namespace ETicaret.Application.Features.Suppliers.Constants;

/// <summary>
/// Supplier feature'ı için sabit değerler.
/// </summary>
public static class SupplierConstants
{
    /// <summary>
    /// Supplier cache group anahtarı.
    /// </summary>
    public const string SuppliersCacheGroup = "Suppliers";

    /// <summary>
    /// Error codes for Supplier operations.
    /// </summary>
    public static class ErrorCodes
    {
        public const string DuplicateCompanyName = "DUPLICATE_COMPANY_NAME";
        public const string DuplicateContactEmail = "DUPLICATE_CONTACT_EMAIL";
        public const string DuplicatePhoneNumber = "DUPLICATE_PHONE_NUMBER";
        public const string SupplierNotFound = "SUPPLIER_NOT_FOUND";
        public const string SupplierHasActiveProducts = "SUPPLIER_HAS_ACTIVE_PRODUCTS";
    }
}