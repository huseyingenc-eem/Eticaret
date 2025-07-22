namespace Core.Domain.Entities;

/// <summary>
/// Bir varlığın geçici olarak silinebilir (soft-deletable) olduğunu belirten sözleşme.
/// </summary>
public interface ISoftDeletable
{
    /// <summary>
    /// Varlığın silindi olarak işaretlendiği zaman damgasını alır veya ayarlar.
    /// Silinmemişse null'dır.
    /// </summary>
    DateTime? DeletedTime { get; }
}