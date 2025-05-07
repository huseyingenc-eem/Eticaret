using AutoMapper;
using Core.Application.Pipelines.Authorization;
using ETicaret.Application.Services.Repositories;
using ETicaret.Domain.Entities;
using ETicaret.Domain.Enums;
using MediatR;
using System.Linq.Expressions;

namespace ETicaret.Application.Features.Orders.Queries.GetListForEmployee;


public class GetOrderListForEmployeeQuery : IRequest<List<GetOrderListForEmployeeResponseDto>>
{
    public OrderStatus? StatusFilter { get; set; }

    public class GetOrderListForEmployeeQueryHandler : IRequestHandler<GetOrderListForEmployeeQuery, List<GetOrderListForEmployeeResponseDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetOrderListForEmployeeQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<GetOrderListForEmployeeResponseDto>> Handle(GetOrderListForEmployeeQuery request, CancellationToken cancellationToken)
        {

            Expression<Func<Order, bool>>? filterExpression = null;

            if (request.StatusFilter.HasValue)
            {
                filterExpression = o => o.Status == request.StatusFilter.Value;
            }

            List<Order> orders = await _unitOfWork.OrderRepository.GetListAsync(
                filter: filterExpression, 
                orderBy: q => q.OrderByDescending(o => o.OrderDate),
                include: true,
                enableTracking: false,
                cancellationToken: cancellationToken
            );

            List<GetOrderListForEmployeeResponseDto> response = _mapper.Map<List<GetOrderListForEmployeeResponseDto>>(orders);

            return response;
        }
    }
}
