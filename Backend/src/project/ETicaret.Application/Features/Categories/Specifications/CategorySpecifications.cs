using Core.Application.Abstractions.Specifications;
using ETicaret.Domain.Entities;
using static Core.Application.Abstractions.Specifications.CommonSpecifications;

namespace ETicaret.Application.Features.Categories.Specifications;

/// <summary>
/// Category entity'si için spesifikasyonlar.
/// </summary>
public static class CategorySpecifications
{
    /// <summary>
    /// ID'ye göre kategori getirme - Generic specification kullanır
    /// </summary>
    public class ById : ByIdSpecification<Category, int>
    {
        public ById(int id) : base(id) { }
    }

    /// <summary>
    /// İsme göre kategori arama
    /// </summary>
    public class ByName : Specification<Category>
    {
        public ByName(string name)
            : base(c => c.Name.Trim().ToLower() == name.Trim().ToLower())
        {
        }
    }

    /// <summary>
    /// Aktif kategoriler - Generic specification kullanır
    /// </summary>
    public class Active : ActiveEntitiesSpecification<Category>
    {
        public Active()
        {
            AddOrderBy(c => c.Name);
        }
    }

    /// <summary>
    /// Ana kategoriler (parent'ı olmayan)
    /// </summary>
    public class Parents : Specification<Category>
    {
        public Parents()
            : base(category => category.ParentId == null)
        {
            AddOrderBy(c => c.Name);
        }
    }

    /// <summary>
    /// Belirli bir kategorinin alt kategorileri
    /// </summary>
    public class Children : Specification<Category>
    {
        public Children(int parentId, bool onlyActive = false)
            : base(c => c.ParentId == parentId && (!onlyActive || c.IsActive))
        {
            AddOrderBy(c => c.Name);
        }
    }

    /// <summary>
    /// İsim kontrolü (güncelleme sırasında ID hariç)
    /// </summary>
    public class ByNameExcludingId : Specification<Category>
    {
        public ByNameExcludingId(int categoryId, string name)
            : base(c => c.Name.Trim().ToLower() == name.Trim().ToLower() && c.Id != categoryId)
        {
        }
    }
}