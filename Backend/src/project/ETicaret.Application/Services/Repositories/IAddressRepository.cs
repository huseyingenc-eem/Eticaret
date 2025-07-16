using Core.Application.Abstractions.Repositories;
using ETicaret.Domain.Entities;

namespace ETicaret.Application.Services.Repositories;

/// <summary>
/// Address entity'si için veri erişim operasyonlarını tanımlayan interface.
/// Temel CRUD operasyonları için IAsyncRepository ve IRepository'den kalıtım alır.
/// </summary>
public interface IAddressRepository : IRepository<Address, Guid>
{
}