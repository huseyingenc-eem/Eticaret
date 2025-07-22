namespace Core.Application.Common.Exceptions;

/// <summary>
/// Tek bir alan (property) için doğrulama hatalarını temsil eden,
/// değiştirilemez (immutable) bir veri modeli.
/// </summary>
/// <param name="Property">Hatanın ait olduğu alanın adı.</param>
/// <param name="Errors">Bu alana ait hata mesajlarının listesi.</param>
public record ValidationExceptionModel(string? Property, IEnumerable<string> Errors);