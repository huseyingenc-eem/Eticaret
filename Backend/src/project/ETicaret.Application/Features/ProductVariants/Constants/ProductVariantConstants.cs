namespace ETicaret.Application.Features.ProductVariants.Constants;

/// <summary>
/// ProductVariant feature'ı için sabit değerler.
/// </summary>
public static class ProductVariantConstants
{
    public const string ProductVariantsCacheGroup = "ProductVariants";
    public static class ErrorCodes
    {
        public const string ProductVariantNotFound = "PRODUCT_VARIANT_NOT_FOUND";
        public const string DuplicateSku = "DUPLICATE_SKU";
        public const string ProductNotFound = "PRODUCT_NOT_FOUND";
        public const string InvalidPrice = "INVALID_PRICE";
        public const string InvalidStock = "INVALID_STOCK";
        public const string VariantInUse = "VARIANT_IN_USE";
        public const string ProductInactive = "PRODUCT_INACTIVE";
        public const string SkuRequired = "SKU_REQUIRED";
        public const string PriceMustBePositive = "PRICE_MUST_BE_POSITIVE";
    }

    public static class Validation
    {
        public const int MaxSkuLength = 100;
        public const int MinSkuLength = 3;
        public const decimal MinPrice = 0.01m;
        public const decimal MaxPrice = 999999.99m;
        public const int MinStock = 0;
        public const int MaxStock = 100000;
        public const int MaxAttributeDescriptionLength = 500;
        public const int MaxVariantImageUrlLength = 255;
    }
}