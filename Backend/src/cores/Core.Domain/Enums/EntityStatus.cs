namespace Core.Domain.Enums;

/// <summary>
/// Bir varlığın yaşam döngüsündeki temel durumlarını belirtir.
/// </summary>
public enum EntityStatus
{
    /// <summary>
    /// Varlık aktif ve kullanımda.
    /// </summary>
    Active = 1,

    /// <summary>
    /// Varlık pasif, kullanım dışı ancak sistemde mevcut.
    /// </summary>
    Inactive = 2,

    /// <summary>
    /// Varlık silinmiş olarak işaretlenmiş (soft-delete).
    /// </summary>
    Deleted = 3
}