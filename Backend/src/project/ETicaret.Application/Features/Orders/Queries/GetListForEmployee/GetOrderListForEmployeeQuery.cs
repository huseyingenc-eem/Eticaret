using AutoMapper;
using ETicaret.Application.Services.Repositories;
using ETicaret.Domain.Entities;
using ETicaret.Domain.Enums;
using MediatR;
using System.Linq.Expressions;
using Core.Persistence.Paging;
using Microsoft.EntityFrameworkCore;

namespace ETicaret.Application.Features.Orders.Queries.GetListForEmployee;

public class GetOrderListForEmployeeQuery : IRequest<IPaginate<GetOrderListForEmployeeResponseDto>>
{
    public OrderStatus? StatusFilter { get; set; }
    public int PageIndex { get; set; } = 0;
    public int PageSize { get; set; } = 10;

    public class GetOrderListForEmployeeQueryHandler : IRequestHandler<GetOrderListForEmployeeQuery, IPaginate<GetOrderListForEmployeeResponseDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetOrderListForEmployeeQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IPaginate<GetOrderListForEmployeeResponseDto>> Handle(GetOrderListForEmployeeQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<Order, bool>>? filterExpression = null;
            if (request.StatusFilter.HasValue)
            {
                filterExpression = o => o.Status == request.StatusFilter.Value;
            }
            IPaginate<Order> ordersPage = await _unitOfWork.OrderRepository.GetListAsync(
                filter: filterExpression,
                orderBy: q => q.OrderByDescending(o => o.OrderDate),
                include: q => q.Include(o => o.User),
                index: request.PageIndex,
                size: request.PageSize,
                enableTracking: false,
                cancellationToken: cancellationToken
            );

            IPaginate<GetOrderListForEmployeeResponseDto> responsePage = _mapper.Map<IPaginate<GetOrderListForEmployeeResponseDto>>(ordersPage);

            return responsePage;
        }
    }
}
