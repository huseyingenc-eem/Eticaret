// ETicaret.Application/Services/Repositories/IUnitOfWork.cs
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ETicaret.Application.Services.Repositories;

public interface IUnitOfWork : IAsyncDisposable
{
    IAddressRepository AddressRepository { get; }
    ICategoryRepository CategoryRepository { get; }
    IProductRepository ProductRepository { get; }
    ISupplierRepository SupplierRepository { get; }

    IOrderRepository OrderRepository { get; }
    IOrderItemRepository OrderItemRepository { get; }

    // ... gelecekte eklenebilecek diğer repository'ler ...

    Task<int> CompleteAsync(CancellationToken cancellationToken = default);
}