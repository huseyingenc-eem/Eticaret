using Core.Application.Behaviors.Caching;
using Core.Application.Common.Results;
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
    public int PageIndex { get; set; } = 0;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }
    public string? RoleFilter { get; set; }
    public string? CityFilter { get; set; }
    public bool OnlyActiveUsers { get; set; } = false;
    public string SortBy { get; set; } = "Name";
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

public class GetListUserRolesQueryHandler : IRequestHandler<GetListUserRolesQuery, PagedResult<GetListUserRolesResponseDto>>
{
    private readonly UserManager<User> _userManager;

    public GetListUserRolesQueryHandler(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public async Task<PagedResult<GetListUserRolesResponseDto>> Handle(GetListUserRolesQuery request, CancellationToken cancellationToken)
    {
        // 1. Base query oluştur ve filtreleri uygula
        IQueryable<User> query = BuildBaseQuery(request);

        // 2. Toplam kayıt sayısını al (sayfalama için)
        int totalCount = await query.CountAsync(cancellationToken);

        // 3. Sıralama ve sayfalama uygula
        query = ApplySorting(query, request.SortBy, request.SortDirection);
        query = ApplyPagination(query, request.PageIndex, request.PageSize);

        // 4. Kullanıcıları getir
        var users = await query.ToListAsync(cancellationToken);

        // 5. Her kullanıcı için rolleri getir (N+1 durumu ama sayfalama sayesinde sorun yok)
        var userRoleDtos = new List<GetListUserRolesResponseDto>();

        foreach (var user in users)
        {
            var userRoles = await _userManager.GetRolesAsync(user);

            userRoleDtos.Add(new GetListUserRolesResponseDto
            {
                UserId = user.Id,
                FullName = $"{user.FirstName} {user.LastName}".Trim(),
                Email = user.Email!,
                IsActive = user.EmailConfirmed,
                RoleNames = string.Join(", ", userRoles)
            });
        }

        return new PagedResult<GetListUserRolesResponseDto>(
            userRoleDtos,
            totalCount,
            request.PageIndex,
            request.PageSize
        );
    }

    #region Yardımcı Metotlar (Helper Methods)

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

    private static IQueryable<User> ApplySorting(IQueryable<User> query, string sortBy, string sortDirection)
    {
        bool isDescending = sortDirection.Equals("desc", StringComparison.OrdinalIgnoreCase);

        return sortBy.ToLower() switch
        {
            "email" => isDescending ? query.OrderByDescending(u => u.Email) : query.OrderBy(u => u.Email),
            "createddate" => isDescending ? query.OrderByDescending(u => u.Id) : query.OrderBy(u => u.Id),
            "city" => isDescending ? query.OrderByDescending(u => u.City) : query.OrderBy(u => u.City),
            _ => isDescending ? query.OrderByDescending(u => u.FirstName + " " + u.LastName) : query.OrderBy(u => u.FirstName + " " + u.LastName)
        };
    }

    private static IQueryable<User> ApplyPagination(IQueryable<User> query, int pageIndex, int pageSize)
    {
        return query.Skip(pageIndex * pageSize).Take(pageSize);
    }

    #endregion
}

#endregion