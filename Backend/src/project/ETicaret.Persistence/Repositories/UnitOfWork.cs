using Core.Application.Abstractions.Repositories;
using Core.Domain.Entities;
using Core.Infrastructure.Persistence.Repositories;
using ETicaret.Persistence.Contexts;
using Microsoft.EntityFrameworkCore.Storage;
using System.Collections;

namespace ETicaret.Persistence.Repositories;

/// <summary>
/// Unit of Work deseninin modern ve esnek implementasyonu.
/// DbContext'i yönetir, repository'leri dinamik olarak sağlar ve transaction bütünlüğünü korur.
/// </summary>
public sealed class UnitOfWork : IUnitOfWork
{
    private readonly BaseDBContexts _context;
    private Hashtable _repositories;
    private IDbContextTransaction? _transaction;
    private bool _disposed;

    public UnitOfWork(BaseDBContexts context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    #region Repository Erişimi (Dinamik)

    /// <summary>
    /// Belirtilen varlık tipi için repository'nin bir örneğini dinamik olarak döndürür.
    /// Eğer istenen repository daha önce oluşturulduysa, mevcut örnek döndürülür (cache).
    /// Bu, constructor kalabalığını tamamen ortadan kaldırır.
    /// </summary>
    /// <typeparam name="TEntity">Repository'si istenen varlık tipi.</typeparam>
    /// <typeparam name="TId">Varlığın ID tipi.</typeparam>
    /// <returns>IRepository arayüzünün bir örneği.</returns>
    public IRepository<TEntity, TId> GetRepository<TEntity, TId>()
        where TEntity : Entity<TId>
        where TId : IEquatable<TId>
    {
        _repositories ??= new Hashtable();

        var type = typeof(TEntity).Name;

        if (!_repositories.ContainsKey(type))
        {
            var repositoryInstance = new EfRepositoryBase<TEntity, TId, BaseDBContexts>(_context);
            _repositories.Add(type, repositoryInstance);
        }

        return (IRepository<TEntity, TId>)_repositories[type]!;
    }

    #endregion

    #region Transaction Yönetimi

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _transaction ??= await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await _transaction?.CommitAsync(cancellationToken)!;
        }
        catch
        {
            await RollbackTransactionAsync(cancellationToken);
            throw;
        }
        finally
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await _transaction?.RollbackAsync(cancellationToken)!;
        }
        finally
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
    }

    #endregion

    #region Kaydetme İşlemi

    public async Task<int> CompleteAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    #endregion

    #region Dispose Metotları (Sadeleştirilmiş)

    public async ValueTask DisposeAsync()
    {
        if (!_disposed)
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
            }
            await _context.DisposeAsync();
            _disposed = true;
        }
        GC.SuppressFinalize(this);
    }

    #endregion
}