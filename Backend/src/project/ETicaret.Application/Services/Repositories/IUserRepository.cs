using ETicaret.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace ETicaret.Application.Abstractions.Repositories;

/// <summary>
/// UserManager'dan türeyen ve sadece özel business fonksiyonlar ekleyen interface.
/// UserManager'ın tüm metotları otomatik olarak miras alınır.
/// </summary>
public interface IUserRepository
{
    #region Sadece Özel Business Fonksiyonlar

    #endregion
}