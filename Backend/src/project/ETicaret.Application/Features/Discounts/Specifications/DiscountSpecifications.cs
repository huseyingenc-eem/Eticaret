using Core.Application.Abstractions.Specifications;
using ETicaret.Domain.Entities;
using static Core.Application.Abstractions.Specifications.CommonSpecifications;

namespace ETicaret.Application.Features.Discounts.Specifications;

public static class DiscountSpecifications
{
    #region Basic Specifications - Ortak kullanılan temel specifications

    public class ById : ByIdSpecification<Discount, Guid>
    {
        public ById(Guid id) : base(id) { }
    }

    public class Active : ActiveEntitiesSpecification<Discount>
    {
        public Active()
        {
            AddOrderBy(d => d.Name);
        }
    }

    #endregion

    #region Query Specifications - Query handler'lar için karmaşık sorgular

    public class ActiveAndValid : Specification<Discount>
    {
        public ActiveAndValid(DateTime currentDate)
            : base(d => d.IsActive &&
                       d.StartDate <= currentDate &&
                       (d.EndDate == null || d.EndDate >= currentDate))
        {
            AddOrderBy(d => d.StartDate);
        }
    }

    public class PagedAndFiltered : Specification<Discount>
    {
        public PagedAndFiltered(int pageIndex, int pageSize, string? nameSearch = null, bool onlyActive = true)
            : base(discount =>
                (!onlyActive || discount.IsActive) &&
                (string.IsNullOrEmpty(nameSearch) || discount.Name.Contains(nameSearch)))
        {
            AddOrderByDescending(d => d.CreatedTime);
            ApplyPaging(pageIndex * pageSize, pageSize);
        }
    }

    public class ByType : Specification<Discount>
    {
        public ByType(DiscountType discountType, bool onlyActive = true)
            : base(d => d.DiscountType == discountType && (!onlyActive || d.IsActive))
        {
            AddOrderBy(d => d.DiscountValue);
        }
    }

    public class ExpiringDiscount : Specification<Discount>
    {
        public ExpiringDiscount(DateTime fromDate, DateTime toDate)
            : base(d => d.IsActive &&
                       d.EndDate != null &&
                       d.EndDate >= fromDate &&
                       d.EndDate <= toDate)
        {
            AddOrderBy(d => d.EndDate);
        }
    }

    public class ByDiscountCode : Specification<Discount>
    {
        public ByDiscountCode(string discountCode)
            : base(d => d.DiscountCode != null && d.DiscountCode.ToLower() == discountCode.ToLower()) { }
    }

    #endregion

    #region Advanced Query Specifications - Gelişmiş sorgu ihtiyaçları

    public class WithUsageStatistics : Specification<Discount>
    {
        public WithUsageStatistics(bool onlyActive = true)
            : base(d => !onlyActive || d.IsActive)
        {
            AddInclude(d => d.Usages);
            AddOrderByDescending(d => d.CreatedTime);
        }
    }

    public class RecentlyCreated : Specification<Discount>
    {
        public RecentlyCreated(int days = 7, bool onlyActive = true)
            : base(d => d.CreatedTime >= DateTime.UtcNow.AddDays(-days) && (!onlyActive || d.IsActive))
        {
            AddOrderByDescending(d => d.CreatedTime);
        }
    }

    public class ByMinimumPurchaseRange : Specification<Discount>
    {
        public ByMinimumPurchaseRange(decimal minAmount, decimal maxAmount, bool onlyActive = true)
            : base(d => (!onlyActive || d.IsActive) &&
                       d.MinimumPurchaseAmount >= minAmount &&
                       d.MinimumPurchaseAmount <= maxAmount)
        {
            AddOrderBy(d => d.MinimumPurchaseAmount);
        }
    }

    #endregion
}