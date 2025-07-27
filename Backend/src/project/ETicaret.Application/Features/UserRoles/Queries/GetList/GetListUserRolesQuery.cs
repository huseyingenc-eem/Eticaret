using AutoMapper;
using Core.Application.Abstractions.Paging;
using Core.Application.Behaviors.Caching;
using Core.Application.Common.Results;
using ETicaret.Application.Features.UserRoles.Rules;
using ETicaret.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ETicaret.Application.Features.UserRoles.Queries.GetList;

#region Sorgu Sınıfı (Query Class)

/// <summary>
/// Tüm kullanıcıları ve onlara atanmış rolleri sayfalanmış bir şekilde getiren sorgu.
/// ICachableRequest: Bu sorgunun sonucunun önbelleğe alınmasını sağlar.
/// Filtreleme, sayfalama ve sıralama desteği ile yüksek performans sunar.
/// </summary>
public class GetListUserRolesQuery : IRequest<PagedResult<GetListUserRolesResponseDto>>, ICachableRequest
{
    #region Özellikler (Properties)

    /// <summary>
    /// Sayfa indeksi (0'dan başlar).
    /// </summary>
    public int PageIndex { get; set; } = 0;

    /// <summary>
    /// Sayfa boyutu (varsayılan: 10, maksimum: 100).
    /// </summary>
    public int PageSize { get; set; } = 10;

    /// <summary>
    /// Kullanıcı adı, email veya isim-soyisim ile arama.
    /// </summary>
    public string? SearchTerm { get; set; }

    /// <summary>
    /// Belirli bir role sahip kullanıcıları filtreleme.
    /// </summary>
    public string? RoleFilter { get; set; }

    /// <summary>
    /// Şehir bazında filtreleme.
    /// </summary>
    public string? CityFilter { get; set; }

    /// <summary>
    /// Sadece aktif kullanıcıları gösterme (email onaylanmış).
    /// </summary>
    public bool OnlyActiveUsers { get; set; } = false;

    /// <summary>
    /// Sıralama kriteri (Name, Email, CreatedDate).
    /// </summary>
    public string SortBy { get; set; } = "Name";

    /// <summary>
    /// Sıralama yönü (asc, desc).
    /// </summary>
    public string SortDirection { get; set; } = "asc";

    #endregion

    #region Önbellek Ayarları (Cache Settings)

    public bool BypassCache { get; set; }

    public string CacheKey => $"user-roles-list_page_{PageIndex}_size_{PageSize}_search_{SearchTerm}_role_{RoleFilter}_city_{CityFilter}_active_{OnlyActiveUsers}_sort_{SortBy}_{SortDirection}";

    public string? CacheGroupKey => "UserRoles";

    public TimeSpan? SlidingExpiration => TimeSpan.FromMinutes(15);

    public TimeSpan? AbsoluteExpirationRelativeToNow => TimeSpan.FromHours(1);

    #endregion
}

#endregion

#region Sorgu İşleyici (Query Handler)

/// <summary>
/// GetListUserRolesQuery sorgusunu işleyen handler sınıfı.
/// Kullanıcı-rol listesini sayfalama, filtreleme ve önbellekleme ile getirir.
/// </summary>
public class GetListUserRolesQueryHandler : IRequestHandler<GetListUserRolesQuery, PagedResult<GetListUserRolesResponseDto>>
{
    #region Alan Tanımlamaları (Field Declarations)

    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IMapper _mapper;
    private readonly UserRolesBusinessRules _userRolesBusinessRules;

    #endregion

    #region Yapıcı Metot (Constructor)

    /// <summary>
    /// GetListUserRolesQueryHandler sınıfının yeni bir örneğini oluşturur.
    /// </summary>
    /// <param name="userManager">ASP.NET Identity kullanıcı yöneticisi.</param>
    /// <param name="roleManager">ASP.NET Identity rol yöneticisi.</param>
    /// <param name="mapper">Entity ve DTO dönüşümleri için AutoMapper.</param>
    /// <param name="userRolesBusinessRules">UserRoles iş kuralları servisi.</param>
    public GetListUserRolesQueryHandler(
        UserManager<User> userManager,
        RoleManager<IdentityRole> roleManager,
        IMapper mapper,
        UserRolesBusinessRules userRolesBusinessRules)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _mapper = mapper;
        _userRolesBusinessRules = userRolesBusinessRules;
    }

    #endregion

    #region İşleme Metotları (Handler Methods)

    /// <summary>
    /// Kullanıcı-rol listesi sorgusunu işler.
    /// </summary>
    /// <param name="request">Liste getirme sorgusu.</param>
    /// <param name="cancellationToken">İptal token'ı.</param>
    /// <returns>Sayfalanmış kullanıcı-rol listesi.</returns>
    public async Task<PagedResult<GetListUserRolesResponseDto>> Handle(GetListUserRolesQuery request, CancellationToken cancellationToken)
    {
        // 1. İstek parametrelerini doğrula
        ValidateRequest(request);

        // 2. Temel kullanıcı sorgusunu oluştur
        IQueryable<User> query = BuildBaseQuery(request);

        // 3. Toplam kayıt sayısını al (filtreleme sonrası)
        int totalCount = await query.CountAsync(cancellationToken);

        // 4. Sıralama uygula
        query = ApplySorting(query, request.SortBy, request.SortDirection);

        // 5. Sayfalama uygula
        query = ApplyPagination(query, request.PageIndex, request.PageSize);

        // 6. Kullanıcıları listele
        var users = await query.ToListAsync(cancellationToken);

        // 7. Her kullanıcı için rol bilgilerini al ve DTO'ya dönüştür
        var userRoleDtos = await MapUsersToResponseDtosAsync(users, cancellationToken);

        // 8. Sayfalanmış sonuç oluştur ve döndür
        return new PagedResult<GetListUserRolesResponseDto>(
            userRoleDtos,
            totalCount,
            request.PageIndex,
            request.PageSize
        );
    }

    #endregion

    #region Yardımcı Metotlar (Helper Methods)

    /// <summary>
    /// İstek parametrelerinin geçerliliğini kontrol eder.
    /// </summary>
    /// <param name="request">Doğrulanacak istek.</param>
    private void ValidateRequest(GetListUserRolesQuery request)
    {
        UserRolesBusinessRules.ValidatePaginationParameters(request.PageIndex, request.PageSize);

        var validSortFields = new[] { "Name", "Email", "CreatedDate", "City" };
        if (!validSortFields.Contains(request.SortBy, StringComparer.OrdinalIgnoreCase))
        {
            request.SortBy = "Name"; // Varsayılan değer
        }

        var validSortDirections = new[] { "asc", "desc" };
        if (!validSortDirections.Contains(request.SortDirection, StringComparer.OrdinalIgnoreCase))
        {
            request.SortDirection = "asc"; // Varsayılan değer
        }
    }

    /// <summary>
    /// Temel kullanıcı sorgusunu oluşturur ve filtreleri uygular.
    /// </summary>
    /// <param name="request">Sorgu parametreleri.</param>
    /// <returns>Filtrelenmiş kullanıcı sorgusu.</returns>
    private IQueryable<User> BuildBaseQuery(GetListUserRolesQuery request)
    {
        IQueryable<User> query = _userManager.Users;

        // Arama terimi filtresi
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            string searchLower = request.SearchTerm.ToLower();
            query = query.Where(u =>
                u.UserName!.ToLower().Contains(searchLower) ||
                u.Email!.ToLower().Contains(searchLower) ||
                u.FirstName.ToLower().Contains(searchLower) ||
                u.LastName.ToLower().Contains(searchLower) ||
                (u.FirstName + " " + u.LastName).ToLower().Contains(searchLower)
            );
        }

        // Şehir filtresi
        if (!string.IsNullOrWhiteSpace(request.CityFilter))
        {
            query = query.Where(u => u.City != null && u.City.ToLower().Contains(request.CityFilter.ToLower()));
        }

        // Aktif kullanıcı filtresi
        if (request.OnlyActiveUsers)
        {
            query = query.Where(u => u.EmailConfirmed);
        }

        return query;
    }

    /// <summary>
    /// Sorguya sıralama uygular.
    /// </summary>
    /// <param name="query">Sıralanacak sorgu.</param>
    /// <param name="sortBy">Sıralama alanı.</param>
    /// <param name="sortDirection">Sıralama yönü.</param>
    /// <returns>Sıralanmış sorgu.</returns>
    private static IQueryable<User> ApplySorting(IQueryable<User> query, string sortBy, string sortDirection)
    {
        bool isDescending = sortDirection.Equals("desc", StringComparison.OrdinalIgnoreCase);

        return sortBy.ToLower() switch
        {
            "email" => isDescending ? query.OrderByDescending(u => u.Email) : query.OrderBy(u => u.Email),
            "createddate" => isDescending ? query.OrderByDescending(u => u.Id) : query.OrderBy(u => u.Id), // Id genellikle tarih sırasını yansıtır
            "city" => isDescending ? query.OrderByDescending(u => u.City) : query.OrderBy(u => u.City),
            _ => isDescending ? query.OrderByDescending(u => u.FirstName + " " + u.LastName) : query.OrderBy(u => u.FirstName + " " + u.LastName)
        };
    }

    /// <summary>
    /// Sorguya sayfalama uygular.
    /// </summary>
    /// <param name="query">Sayfalanacak sorgu.</param>
    /// <param name="pageIndex">Sayfa indeksi.</param>
    /// <param name="pageSize">Sayfa boyutu.</param>
    /// <returns>Sayfalanmış sorgu.</returns>
    private static IQueryable<User> ApplyPagination(IQueryable<User> query, int pageIndex, int pageSize)
    {
        return query.Skip(pageIndex * pageSize).Take(pageSize);
    }

    /// <summary>
    /// Kullanıcıları yanıt DTO'larına dönüştürür ve rol bilgilerini ekler.
    /// </summary>
    /// <param name="users">Dönüştürülecek kullanıcı listesi.</param>
    /// <param name="cancellationToken">İptal token'ı.</param>
    /// <returns>Yanıt DTO'larının listesi.</returns>
    private async Task<List<GetListUserRolesResponseDto>> MapUsersToResponseDtosAsync(
        List<User> users,
        CancellationToken cancellationToken)
    {
        var result = new List<GetListUserRolesResponseDto>();

        foreach (var user in users)
        {
            // Her kullanıcı için rolleri al
            var userRoles = await _userManager.GetRolesAsync(user);

            // Rol detaylarını al
            var roleDetails = new List<UserRoleDetailDto>();
            foreach (var roleName in userRoles)
            {
                var role = await _roleManager.FindByNameAsync(roleName);
                if (role != null)
                {
                    roleDetails.Add(new UserRoleDetailDto
                    {
                        RoleId = role.Id,
                        RoleName = role.Name!
                    });
                }
            }

            // DTO oluştur
            var dto = new GetListUserRolesResponseDto
            {
                UserId = user.Id,
                UserName = user.UserName!,
                Email = user.Email!,
                FirstName = user.FirstName,
                LastName = user.LastName,
                FullName = $"{user.FirstName} {user.LastName}".Trim(),
                City = user.City,
                EmailConfirmed = user.EmailConfirmed,
                Roles = roleDetails,
                RoleCount = roleDetails.Count,
                RoleNames = string.Join(", ", userRoles)
            };

            result.Add(dto);
        }

        return result;
    }

    #endregion
}

#endregion