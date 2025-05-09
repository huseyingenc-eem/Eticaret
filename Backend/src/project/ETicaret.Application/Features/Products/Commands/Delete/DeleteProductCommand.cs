using ETicaret.Application.Services.RedisServices;
using ETicaret.Application.Services.Repositories;
using MediatR;
using Core.CrossCuttingConcerns.Exceptions;

namespace ETicaret.Application.Features.Products.Commands.Delete;

public class DeleteProductCommand : IRequest<DeleteProductResponseDto>
{
    public Guid Id { get; set; }

    public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, DeleteProductResponseDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRedisService _redisService;
        
        public DeleteProductCommandHandler(IUnitOfWork unitOfWork, IRedisService redisService) 
        {
            _unitOfWork = unitOfWork; 
            _redisService = redisService;
        }

        public async Task<DeleteProductResponseDto> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            var product = await _unitOfWork.ProductRepository.GetAsync(filter: x => x.Id == request.Id, cancellationToken: cancellationToken);

            if (product == null)
                throw new NotFoundException($"Product with Id {request.Id} not found.");

            await _unitOfWork.ProductRepository.DeleteAsync(product, cancellationToken: cancellationToken); 
            await _unitOfWork.CompleteAsync(cancellationToken); 

            // Cache temizleme
            await _redisService.RemoveDataAsync("products");
            await _redisService.RemoveDataAsync($"product:{request.Id}");

            return new DeleteProductResponseDto { Id = request.Id, Message = "Ürün başarıyla silindi." };
        }
    }
}