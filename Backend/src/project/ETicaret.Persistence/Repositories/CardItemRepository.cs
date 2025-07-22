using Core.Infrastructure.Persistence.Repositories;
using ETicaret.Application.Services.Repositories;
using ETicaret.Domain.Entities;
using ETicaret.Persistence.Contexts;

namespace ETicaret.Persistence.Repositories;

// CartItem için Repository
public class CardItemRepository : EfRepositoryBase<CardItem, Guid, BaseDBContexts>, ICardItemRepository
{
    public CardItemRepository(BaseDBContexts context) : base(context){}
    
}
