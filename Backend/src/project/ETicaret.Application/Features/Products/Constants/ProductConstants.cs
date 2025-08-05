namespace ETicaret.Application.Features.Products.Constants;

public static class ProductConstants
{
    public const string ProductsCacheGroup = "Products";

    #region Error Codes
    public static class ErrorCodes
    {
        public const string ProductNotFound = "PRODUCT_NOT_FOUND";
        public const string DuplicateProductName = "DUPLICATE_PRODUCT_NAME";
        public const string InvalidCategory = "INVALID_CATEGORY";
        public const string InvalidSupplier = "INVALID_SUPPLIER";
        public const string ProductHasVariants = "PRODUCT_HAS_VARIANTS";
        public const string ProductInUse = "PRODUCT_IN_USE";
    }
    #endregion

    #region Validation Constants
    public static class Validation
    {
        public const int MaxNameLength = 200;
        public const int MaxDescriptionLength = 1000;
        public const int MinNameLength = 3;
        public const decimal MinPrice = 0.01m;
        public const int MaxStock = 100000;
        public const int MaxSkuLength = 100;
        public const int MaxImageUrlLength = 255;
    }
    #endregion
}