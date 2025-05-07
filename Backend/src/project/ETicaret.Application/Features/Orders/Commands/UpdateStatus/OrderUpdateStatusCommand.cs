using AutoMapper;
using ETicaret.Application.Services.Repositories;
using ETicaret.Domain.Entities;
using ETicaret.Domain.Enums;
using MediatR;
using Core.CrossCuttingConcerns.Exceptions;
using FluentValidation;

namespace ETicaret.Application.Features.Orders.Commands.UpdateStatus;

public class OrderUpdateStatusCommand : IRequest<OrderUpdateStatusResponse>
{
    public int OrderId { get; set; }
    public OrderStatus NewStatus { get; set; }

    public class OrderUpdateStatusCommandHandler : IRequestHandler<OrderUpdateStatusCommand, OrderUpdateStatusResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public OrderStatus NewStatus { get; set; }

        public OrderUpdateStatusCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<OrderUpdateStatusResponse> Handle(OrderUpdateStatusCommand request, CancellationToken cancellationToken)
        {
            Order? orderToUpdate = await _unitOfWork.OrderRepository.GetAsync(
                filter: o => o.Id == request.OrderId,
                cancellationToken: cancellationToken);

            // Sipariş bulunamazsa hata fırlat
            if (orderToUpdate == null)
            {
                throw new NotFoundException($"Güncellenecek sipariş bulunamadı (ID: {request.OrderId}).");
            }
            if (!IsValidStatusTransition(orderToUpdate.Status, request.NewStatus))
            {
                throw new BusinessException($"Sipariş durumu '{orderToUpdate.Status}' iken '{request.NewStatus}' yapılamaz.");
            }

            orderToUpdate.Status = request.NewStatus;
            orderToUpdate.UpdateTime = DateTime.UtcNow;

            await _unitOfWork.CompleteAsync(cancellationToken);

            OrderUpdateStatusResponse response = _mapper.Map<OrderUpdateStatusResponse>(orderToUpdate);
            response.CurrentStatus = orderToUpdate.Status.ToString();

            return response;
        }
         private bool IsValidStatusTransition(OrderStatus from, OrderStatus to)
        {
            if (from == OrderStatus.Delivered && to == OrderStatus.Cancelled) return false;
            if (from == OrderStatus.Cancelled) return false; 
            return true;
        }
        
    }
}
