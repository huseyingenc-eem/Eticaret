namespace Core.Application.Common.Results;

/// <summary>
/// Bir işlemin sonucunu temsil eden ve veri taşımayan temel sınıf.
/// </summary>
public class Result : IResult
{
    public bool IsSuccess { get; }
    public string Message { get; }
    public IEnumerable<string>? Errors { get; }

    // Bu constructor, dışarıdan doğrudan çağrılmasın diye protected.
    // Başarı veya başarısızlık durumları için static factory metotları kullanılmalıdır.
    protected Result(bool isSuccess, string message, IEnumerable<string>? errors = null)
    {
        IsSuccess = isSuccess;
        Message = message;
        Errors = errors ?? Enumerable.Empty<string>();
    }

    /// <summary>
    /// Başarılı bir işlem sonucunu temsil eder.
    /// </summary>
    /// <param name="message">Başarı mesajı.</param>
    public static Result Success(string message) => new(true, message);

    /// <summary>
    /// Başarısız bir işlem sonucunu temsil eder.
    /// </summary>
    /// <param name="message">Hata mesajı.</param>
    /// <param name="errors">Detaylı hata listesi (opsiyonel).</param>
    public static Result Failure(string message, IEnumerable<string>? errors = null) => new(false, message, errors);
}

/// <summary>
/// Bir işlemin sonucunu ve işlem sonucunda dönen veriyi temsil eden jenerik sınıf.
/// </summary>
/// <typeparam name="T">Dönen verinin türü.</typeparam>
public class Result<T> : Result, IResult
{
    public T? Data { get; }

    protected Result(bool isSuccess, string message, T? data, IEnumerable<string>? errors = null)
        : base(isSuccess, message, errors)
    {
        Data = data;
    }

    /// <summary>
    /// Veri içeren başarılı bir işlem sonucunu temsil eder.
    /// </summary>
    /// <param name="data">İşlem sonucu dönen veri.</param>
    /// <param name="message">Başarı mesajı.</param>
    public static Result<T> Success(T data, string message) => new(true, message, data);

    /// <summary>
    /// Veri içermeyen, başarısız bir işlem sonucunu temsil eder.
    /// </summary>
    /// <param name="message">Hata mesajı.</param>
    /// <param name="errors">Detaylı hata listesi (opsiyonel).</param>
    public static new Result<T> Failure(string message, IEnumerable<string>? errors = null) => new(false, message, default, errors);
}