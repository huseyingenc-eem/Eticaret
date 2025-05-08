using AutoMapper;
using ETicaret.Application.Services.Repositories;
using ETicaret.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Core.CrossCuttingConcerns.Exceptions;
using Core.Persistence.Paging;

namespace ETicaret.Application.Features.Orders.Queries.GetOrdersByUserId;

public class GetOrdersByUserIdQuery : IRequest<IPaginate<GetOrdersByUserIdResponseDto>>
{
    public int PageIndex { get; set; } = 0;
    public int PageSize { get; set; } = 20;

    public class GetOrdersByUserIdQueryHandler : IRequestHandler<GetOrdersByUserIdQuery, IPaginate<GetOrdersByUserIdResponseDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GetOrdersByUserIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IPaginate<GetOrdersByUserIdResponseDto>> Handle(GetOrdersByUserIdQuery request, CancellationToken cancellationToken)
        {
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                throw new AuthorizationException("Siparişlerinizi görmek için giriş yapmalısınız.");
            }

            // Sayfalama destekli GetListAsync metodu çağrıldı (IPaginate<Order> döndüren)
            IPaginate<Order> ordersPage = await _unitOfWork.OrderRepository.GetListAsync(
                filter: o => o.UserId == userId,
                orderBy: q => q.OrderByDescending(o => o.OrderDate),
                include: null, // İlişkili veri gerekmiyorsa null.
                index: request.PageIndex,
                size: request.PageSize,
                enableTracking: false,
                cancellationToken: cancellationToken
            );

            IPaginate<GetOrdersByUserIdResponseDto> responsePage = _mapper.Map<IPaginate<GetOrdersByUserIdResponseDto>>(ordersPage);

            return responsePage;
        }
    }
}
