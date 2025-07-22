using AutoMapper;
using Core.Application.Abstractions.Repositories;
using Core.Application.Abstractions.Services;
using MediatR;

namespace Core.Application.Services;

/// <summary>
/// Uygulama servisleri için ortak bağımlılıkları ve işlevleri sağlayan temel sınıf.
/// Bu sınıf, somut servislerdeki constructor kalabalığını azaltır.
/// </summary>
public abstract class ApplicationService
{
    // protected: Bu sınıftan türeyen alt sınıflar bu alanlara erişebilir.
    protected readonly IUnitOfWork UnitOfWork;
    protected readonly IMapper Mapper;
    protected readonly IMediator Mediator;
    protected readonly ILoggerService Logger;

    protected ApplicationService(IUnitOfWork unitOfWork, IMapper mapper, IMediator mediator, ILoggerService logger)
    {
        UnitOfWork = unitOfWork;
        Mapper = mapper;
        Mediator = mediator;
        Logger = logger;
    }
}