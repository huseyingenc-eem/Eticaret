using AutoMapper;
using Core.Application.Abstractions.Specifications;
using Core.Application.Behaviors.Caching;
using ETicaret.Application.Features.OperationClaims.Specifications;
using ETicaret.Application.Services.Repositories;
using ETicaret.Domain.Entities;
using MediatR;

namespace ETicaret.Application.Features.OperationClaims.Queries.GetList;

#region Get List OperationClaims Query

public class GetListOperationClaimsQuery : IRequest<List<GetListOperationClaimsResponseDto>>, ICachableRequest
{
    public string? OperationNameFilter { get; set; }
    public string? FeatureNameFilter { get; set; }
    public string? RequiredRoleFilter { get; set; }

    public bool BypassCache { get; set; }
    public string CacheKey => $"operation-claims-list_{OperationNameFilter}_{FeatureNameFilter}_{RequiredRoleFilter}";
    public string? CacheGroupKey => "OperationClaims";
    public TimeSpan? SlidingExpiration => TimeSpan.FromMinutes(30);
    public TimeSpan? AbsoluteExpirationRelativeToNow => TimeSpan.FromHours(2);
}

#endregion

#region Get List OperationClaims Query Handler

public class GetListOperationClaimsQueryHandler : IRequestHandler<GetListOperationClaimsQuery, List<GetListOperationClaimsResponseDto>>
{
    private readonly IOperationClaimRepository _operationClaimRepository;
    private readonly IMapper _mapper;

    public GetListOperationClaimsQueryHandler(IOperationClaimRepository operationClaimRepository, IMapper mapper)
    {
        _mapper = mapper;
        _operationClaimRepository = operationClaimRepository;
    }

    public async Task<List<GetListOperationClaimsResponseDto>> Handle(GetListOperationClaimsQuery request, CancellationToken cancellationToken)
    {
        var specification = BuildSpecification(request);

        var operationClaims = await _operationClaimRepository.GetListAsync(specification, cancellationToken);

        return _mapper.Map<List<GetListOperationClaimsResponseDto>>(operationClaims);
    }

    #region Helper Method - Sadece gerçekten karmaşık olanlar için

    private ISpecification<OperationClaim> BuildSpecification(GetListOperationClaimsQuery request)
    {
        if (string.IsNullOrWhiteSpace(request.OperationNameFilter) &&
            string.IsNullOrWhiteSpace(request.FeatureNameFilter) &&
            string.IsNullOrWhiteSpace(request.RequiredRoleFilter))
        {
            return new OperationClaimSpecifications.All();
        }

        if (!string.IsNullOrWhiteSpace(request.OperationNameFilter))
            return new OperationClaimSpecifications.ByOperationName(request.OperationNameFilter);

        if (!string.IsNullOrWhiteSpace(request.FeatureNameFilter))
            return new OperationClaimSpecifications.ByFeatureName(request.FeatureNameFilter);

        if (!string.IsNullOrWhiteSpace(request.RequiredRoleFilter))
            return new OperationClaimSpecifications.ByRequiredRoles(request.RequiredRoleFilter);

        return new OperationClaimSpecifications.All();
    }

    #endregion
}

#endregion