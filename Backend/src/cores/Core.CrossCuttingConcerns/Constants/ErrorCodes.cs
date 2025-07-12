namespace Core.Shared.Constants;

public static class ErrorCodes
{
    // Genel Hatalar
    public const string UnhandledException = "GENERAL_UNHANDLED";
    public const string ValidationError = "VALIDATION_ERROR";
    public const string AuthGeneral = "AUTHORIZATION_FAILURE";
    public const string NotFoundGeneral = "NOTFOUND_FAILURE";
    // İş Mantığı Hataları (Örnekler)
    public const string ProductNameAlreadyExists = "PRODUCT_001";
    public const string ProductNotFound = "PRODUCT_002";
    public const string InsufficientStock = "PRODUCT_003";
    public const string OrderCannotBeCancelled = "ORDER_001";
    public const string UserNotFound = "USER_001";
    public const string InvalidCredentials = "AUTH_001";
    public const string EmailAlreadyInUse = "USER_002";

    // Yetkilendirme Hataları
    public const string UnauthorizedAccess = "AUTH_002";
    public const string ForbiddenAccess = "AUTH_003";

    // Kaynak Bulunamadı Hataları
    public const string GenericNotFound = "NOTFOUND_001"; 

}
