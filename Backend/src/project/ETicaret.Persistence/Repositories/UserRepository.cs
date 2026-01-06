using ETicaret.Application.Abstractions.Repositories;
using ETicaret.Domain.Entities;
using ETicaret.Persistence.Contexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ETicaret.Persistence.Repositories;

/// <summary>
/// UserManager'dan türeyen repository. Tüm UserManager fonksiyonları otomatik miras alınır.
/// Sadece özel business fonksiyonlar eklenir.
/// </summary>
public class UserRepository : UserManager<User>, IUserRepository
{
    private readonly BaseDBContexts _context;

    public UserRepository(
        IUserStore<User> store,
        IOptions<IdentityOptions> optionsAccessor,
        IPasswordHasher<User> passwordHasher,
        IEnumerable<IUserValidator<User>> userValidators,
        IEnumerable<IPasswordValidator<User>> passwordValidators,
        ILookupNormalizer keyNormalizer,
        IdentityErrorDescriber errors,
        IServiceProvider services,
        ILogger<UserManager<User>> logger,
        BaseDBContexts context)
        : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
    {
        _context = context;
    }

    #region Özel Business Fonksiyonlar
    #endregion
}