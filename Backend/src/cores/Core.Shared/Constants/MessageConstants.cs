namespace Core.Shared.Constants;

/// <summary>
/// Uygulama genelinde kullanılacak, yeniden kullanılabilir ve dinamik mesaj şablonlarını barındırır.
/// </summary>
public static class MessageConstants
{
    /// <summary>
    /// Başarılı işlemler için gösterilecek mesaj şablonları.
    /// </summary>
    public static class Success
    {
        /// <summary>
        /// Belirtilen türdeki bir varlığın başarıyla eklendiğini bildiren mesajı oluşturur.
        /// </summary>
        /// <param name="entityName">Varlığın kullanıcı dostu adı (örn: "Müşteri", "Sipariş").</param>
        /// <returns>Örn: "Müşteri başarıyla eklendi."</returns>
        public static string EntityAdded(string entityName) => $"{entityName} başarıyla eklendi.";

        /// <summary>
        /// Belirtilen türdeki bir varlığın başarıyla güncellendiğini bildiren mesajı oluşturur.
        /// </summary>
        public static string EntityUpdated(string entityName) => $"{entityName} başarıyla güncellendi.";

        /// <summary>
        /// Belirtilen türdeki bir varlığın başarıyla silindiğini bildiren mesajı oluşturur.
        /// </summary>
        public static string EntityDeleted(string entityName) => $"{entityName} başarıyla silindi.";
    }

    /// <summary>
    /// Hatalı işlemler için gösterilecek mesaj şablonları.
    /// </summary>
    public static class Error
    {
        /// <summary>
        /// Belirtilen türdeki bir varlığın bulunamadığını bildiren mesajı oluşturur.
        /// </summary>
        /// <param name="entityName">Varlığın kullanıcı dostu adı.</param>
        /// <returns>Örn: "Belirtilen sipariş bulunamadı."</returns>
        public static string EntityNotFound(string entityName) => $"Belirtilen {entityName.ToLower()} bulunamadı.";

        /// <summary>
        /// Genel yetkilendirme hatası mesajı.
        /// </summary>
        public const string AuthorizationDenied = "Bu işlemi gerçekleştirmek için yetkiniz bulunmamaktadır.";
    }
}