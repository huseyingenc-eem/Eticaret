using Core.Application.Abstractions.Repositories;
using ETicaret.Domain.Entities;

namespace ETicaret.Application.Services.Repositories;

/// <summary>
/// Review varlığı için özelleştirilmiş repository arayüzü.
/// Genel IRepository'den miras alarak, Review'a özgü iş mantığını destekleyen ek metotlar sağlar.
/// </summary>
public interface IReviewRepository : IRepository<Review, Guid> {}