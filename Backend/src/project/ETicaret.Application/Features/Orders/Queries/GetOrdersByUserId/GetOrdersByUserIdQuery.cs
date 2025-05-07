using AutoMapper;
using ETicaret.Application.Services.Repositories;
using ETicaret.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Core.CrossCuttingConcerns.Exceptions;

namespace ETicaret.Application.Features.Orders.Queries.GetListByUserId;

// 1. Query Sınıfı (Değişiklik Yok)
public class GetOrdersByUserIdQuery : IRequest<List<GetOrdersByUserIdResponseDto>>
{
    // 2. Handler Sınıfı (Güncellenmiş Repository Çağrısı)
    public class GetOrdersByUserIdQueryHandler : IRequestHandler<GetOrdersByUserIdQuery, List<GetOrdersByUserIdResponseDto>>
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

        public async Task<List<GetOrdersByUserIdResponseDto>> Handle(GetOrdersByUserIdQuery request, CancellationToken cancellationToken)
        {
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                throw new AuthorizationException("Siparişlerinizi görmek için giriş yapmalısınız.");
            }

            List<Order> orders = await _unitOfWork.OrderRepository.GetListAsync(
                filter: o => o.UserId == userId,
                orderBy: q => q.OrderByDescending(o => o.OrderDate),
                include: true,
                enableTracking: false,
                cancellationToken: cancellationToken
            );

            List<GetOrdersByUserIdResponseDto> response = _mapper.Map<List<GetOrdersByUserIdResponseDto>>(orders);

            return response;
        }
    }
}
