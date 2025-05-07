using AutoMapper;
using ETicaret.Application.Services.Repositories;
using ETicaret.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Core.CrossCuttingConcerns.Exceptions;

namespace ETicaret.Application.Features.Orders.Queries.GetById;

public class GetOrderByIdQuery : IRequest<GetOrderByIdResponseDto>
{
    public int OrderId { get; set; }

    public GetOrderByIdQuery(int orderId)
    {
        OrderId = orderId;
    }
    public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, GetOrderByIdResponseDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GetOrderByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<GetOrderByIdResponseDto> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
        {
            Order? order = await _unitOfWork.OrderRepository.GetAsync(
                filter: o => o.Id == request.OrderId,
                include: true,
                enableTracking: false,
                cancellationToken: cancellationToken
            );

            if (order == null)
            {
                throw new NotFoundException($"Sipariş bulunamadı (ID: {request.OrderId}).");
            }

            var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (order.UserId != userId /* && !isAdmin */)
            {
                throw new AuthorizationException("Bu siparişi görüntüleme yetkiniz yok.");
            }

            GetOrderByIdResponseDto response = _mapper.Map<GetOrderByIdResponseDto>(order);

            return response;
        }
    }
}
