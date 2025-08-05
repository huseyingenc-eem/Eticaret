namespace ETicaret.Application.Features.Discounts.Constants;

/// <summary>
/// Discount feature'ı için sabit değerler.
/// </summary>
public static class DiscountConstants
{
    /// <summary>
    /// Discount cache group anahtarı.
    /// </summary>
    public const string DiscountsCacheGroup = "Discounts";

    /// <summary>
    /// Error codes for Discount operations.
    /// </summary>
    public static class ErrorCodes
    {
        public const string DuplicateDiscountCode = "DUPLICATE_DISCOUNT_CODE";
        public const string InvalidDiscountValue = "INVALID_DISCOUNT_VALUE";
        public const string InvalidDateRange = "INVALID_DATE_RANGE";
        public const string DiscountNotFound = "DISCOUNT_NOT_FOUND";
        public const string DiscountExpired = "DISCOUNT_EXPIRED";
        public const string DiscountNotActive = "DISCOUNT_NOT_ACTIVE";
        public const string DiscountAlreadyUsed = "DISCOUNT_ALREADY_USED";
        public const string DiscountUsageExceeded = "DISCOUNT_USAGE_EXCEEDED";
        public const string MinimumPurchaseNotMet = "MINIMUM_PURCHASE_NOT_MET";
        public const string DiscountInUse = "DISCOUNT_IN_USE";
    }

    /// <summary>
    /// Discount validation constants.
    /// </summary>
    public static class Validation
    {
        public const int MaxDiscountCodeLength = 50;
        public const int MaxNameLength = 150;
        public const int MaxDescriptionLength = 500;
        public const decimal MaxPercentageValue = 100;
        public const decimal MinDiscountValue = 0;
        public const int MaxUsesLimit = 999999;
    }
}