using Core.Persistence.Repositories;
using ETicaret.Domain.Entities;

namespace ETicaret.Application.Services.Repositories;

public interface ICardItemRepository : IAsyncRepository<CardItem, Guid>, IRepository<CardItem, Guid>
{
}