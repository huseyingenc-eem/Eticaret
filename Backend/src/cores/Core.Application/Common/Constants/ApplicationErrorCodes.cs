using Microsoft.Extensions.Configuration;

namespace Core.Application.Common.Constants;

/// <summary>
/// Uygulama genelinde kullanılacak hata kodlarını dinamik olarak üreten sınıf.
/// Şirket/uygulama prefixi ve mesajları configuration'dan alır.
/// </summary>
public static class ApplicationErrorCodes
{
    private static string? _appPrefix;
    private static IConfiguration? _configuration;

    /// <summary>
    /// Error code sistemini başlatır. Startup'ta çağrılmalıdır.
    /// </summary>
    /// <param name="configuration">Configuration servisi</param>
    public static void Initialize(IConfiguration configuration)
    {
        _configuration = configuration;
        _appPrefix = configuration["ErrorCodes:AppPrefix"] ?? "MYAPP";
    }

    /// <summary>
    /// Uygulama prefixi (örn: AMAZON, TRENDYOL, ETICARET)
    /// </summary>
    public static string AppPrefix => _appPrefix ?? "MYAPP";

    #region Core Error Codes

    /// <summary>
    /// Validation hataları için error code
    /// </summary>
    public static string ValidationError => BuildErrorCode("VALIDATION", "ERROR");

    /// <summary>
    /// Genel işlenmemiş hatalar için error code
    /// </summary>
    public static string UnhandledException => BuildErrorCode("GENERAL", "UNHANDLED");

    /// <summary>
    /// Yetkilendirme hataları için error code
    /// </summary>
    public static string AuthGeneral => BuildErrorCode("AUTHORIZATION", "FAILURE");

    /// <summary>
    /// Kaynak bulunamadı hataları için error code
    /// </summary>
    public static string NotFoundGeneral => BuildErrorCode("NOTFOUND", "FAILURE");

    /// <summary>
    /// İş kuralı ihlali hataları için error code
    /// </summary>
    public static string BusinessRuleViolation => BuildErrorCode("BUSINESS", "RULE_VIOLATION");

    /// <summary>
    /// Redis/Cache hataları için error code
    /// </summary>
    public static string CacheError => BuildErrorCode("CACHE", "ERROR");

    /// <summary>
    /// Veritabanı bağlantı hataları için error code
    /// </summary>
    public static string DatabaseError => BuildErrorCode("DATABASE", "ERROR");

    #endregion

    #region Domain Specific Error Codes

    /// <summary>
    /// OperationClaim domain'i için error code'lar
    /// </summary>
    public static class OperationClaim
    {
        public static string OperationNameRequired => BuildErrorCode("OPERATION_CLAIM", "NAME_REQUIRED");
        public static string OperationNameNotUnique => BuildErrorCode("OPERATION_CLAIM", "NAME_NOT_UNIQUE");
        public static string FeatureNameRequired => BuildErrorCode("OPERATION_CLAIM", "FEATURE_NAME_REQUIRED");
        public static string RequiredRolesRequired => BuildErrorCode("OPERATION_CLAIM", "REQUIRED_ROLES_REQUIRED");
        public static string RequiredRolesInvalid => BuildErrorCode("OPERATION_CLAIM", "REQUIRED_ROLES_INVALID");
        public static string RequiredRolesContainsEmpty => BuildErrorCode("OPERATION_CLAIM", "REQUIRED_ROLES_CONTAINS_EMPTY");
        public static string NotFound => BuildErrorCode("OPERATION_CLAIM", "NOT_FOUND");
    }

    /// <summary>
    /// Address domain'i için error code'lar
    /// </summary>
    public static class Address
    {
        public static string UserNotFound => BuildErrorCode("ADDRESS", "USER_NOT_FOUND");
        public static string DuplicateTitle => BuildErrorCode("ADDRESS", "DUPLICATE_TITLE");
        public static string DefaultTypeRequired => BuildErrorCode("ADDRESS", "DEFAULT_TYPE_REQUIRED");
        public static string LimitExceeded => BuildErrorCode("ADDRESS", "LIMIT_EXCEEDED");
        public static string NotFound => BuildErrorCode("ADDRESS", "NOT_FOUND");
        public static string Unauthorized => BuildErrorCode("ADDRESS", "UNAUTHORIZED");
    }

    /// <summary>
    /// Category domain'i için error code'lar
    /// </summary>
    public static class Category
    {
        public static string DuplicateName => BuildErrorCode("CATEGORY", "DUPLICATE_NAME");
        public static string HasChildCategories => BuildErrorCode("CATEGORY", "HAS_CHILD_CATEGORIES");
        public static string NotFound => BuildErrorCode("CATEGORY", "NOT_FOUND");
        public static string CircularReference => BuildErrorCode("CATEGORY", "CIRCULAR_REFERENCE");
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Error code oluşturan helper metod
    /// </summary>
    /// <param name="domain">Domain adı (örn: OPERATION_CLAIM, ADDRESS)</param>
    /// <param name="errorType">Hata tipi (örn: NOT_FOUND, VALIDATION_ERROR)</param>
    /// <returns>Formatlanmış error code</returns>
    private static string BuildErrorCode(string domain, string errorType)
    {
        return $"{AppPrefix}.{domain}.{errorType}";
    }

    /// <summary>
    /// Custom error code oluşturmak için public metod
    /// </summary>
    /// <param name="domain">Domain adı</param>
    /// <param name="errorType">Hata tipi</param>
    /// <returns>Formatlanmış error code</returns>
    public static string CreateErrorCode(string domain, string errorType)
    {
        return BuildErrorCode(domain, errorType);
    }

    #endregion
}