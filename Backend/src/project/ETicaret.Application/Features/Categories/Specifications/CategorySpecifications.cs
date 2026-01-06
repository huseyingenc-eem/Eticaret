using Core.Application.Abstractions.Specifications;
using ETicaret.Domain.Entities;
using static Core.Application.Abstractions.Specifications.CommonSpecifications;

namespace ETicaret.Application.Features.Categories.Specifications;

/// <summary>
/// Sadece Handler ve Query'ler tarafından kullanılan karmaşık specifications
/// </summary>
public static class CategorySpecifications
{
    #region Basic Specifications - Handler'lar için gerekli

    /// <summary>
    /// ID'ye göre kategori getirme - Handler'larda kullanılır
    /// Rules zaten existence kontrolü yaptı, Handler sadece get yapar
    /// </summary>
    public class ById : ByIdSpecification<Category, int>
    {
        public ById(int id) : base(id) { }
    }

    #endregion

    #region Query Specifications

    /// <summary>
    /// Aktif kategorileri getiren spesifikasyon
    /// Query handler'larda kullanılır
    /// </summary>
    public class Active : Specification<Category>
    {
        public Active()
            : base(category => category.IsActive)
        {
            AddOrderBy(c => c.Name);
        }
    }
    public class ByIdWithDetails : Specification<Category>
    {
        public ByIdWithDetails(int id)
            : base(category => category.Id == id)
        {
            // Parent bilgisini dahil et
            AddInclude(c => c.Parent);

            // Children sayısı için gerekli
            AddInclude(c => c.Children);

            // Product sayısı için gerekli
            AddInclude(c => c.Products);
        }
    }
    /// <summary>
    /// Tree yapısı için tüm aktif kategorileri flat liste olarak getiren spesifikasyon
    /// INCLUDE KULLANMAZ - Duplicated problem'i önler
    /// </summary>
    public class TreeData : Specification<Category>
    {
        public TreeData()
            : base(category => category.IsActive)
        {
            // ❌ AddInclude kullanma! Memory'de tree building yapacağız
            AddOrderBy(c => c.ParentId ?? 0); // Ana kategoriler önce
            AddOrderBy(c => c.Name);
        }
    }

    /// <summary>
    /// Ana kategorileri (ParentId'si null olanlar) getiren spesifikasyon
    /// Hiyerarşik listeleme için kullanılır
    /// </summary>
    public class Parents : Specification<Category>
    {
        public Parents(bool? onlyActive = null)
            : base(category => category.ParentId == null && (onlyActive ==null || category.IsActive))
        {
            AddOrderBy(c => c.Name);
        }
    }

    /// <summary>
    /// Belirli bir kategorinin alt kategorilerini getiren spesifikasyon
    /// </summary>
    public class ByParentId : Specification<Category>
    {
        public ByParentId(int parentId, bool? onlyActive = null)
            : base(category => category.ParentId == parentId &&
                              (onlyActive == null || category.IsActive == onlyActive.Value))
        {
            AddOrderBy(c => c.Name);
        }
    }

    #endregion

    #region Admin Specifications

    /// <summary>
    /// Admin paneli için sayfalanmış kategori listesi
    /// Filtreleme ve arama desteği ile
    /// </summary>
    public class AdminPagedAndFiltered : Specification<Category>
    {
        public AdminPagedAndFiltered(int pageIndex, int pageSize, string? nameFilter = null, bool? isActiveFilter = null)
            : base(category =>
                (string.IsNullOrEmpty(nameFilter) || category.Name.Contains(nameFilter)) &&
                (isActiveFilter == null || category.IsActive == isActiveFilter))
        {
            AddInclude(c => c.Parent);
            AddOrderBy(c => c.Name);
            ApplyPaging(pageIndex * pageSize, pageSize);
        }
    }

    #endregion
}