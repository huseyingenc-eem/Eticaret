using AutoMapper;
using ETicaret.Application.Services.RedisServices;
using ETicaret.Application.Services.Repositories;
using MediatR;
using Core.Shared.Exceptions;

namespace ETicaret.Application.Features.Products.Commands.Update;

public class UpdateProductCommand : IRequest<UpdateProductResponseDto>
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public int CategoryID { get; set; }
    public int SupplierID { get; set; }
    public string? Description { get; set; }
    public string? SKU { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsActive { get; set; }

    public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, UpdateProductResponseDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IRedisService _redisService;

        public UpdateProductCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IRedisService redisService) // Constructor güncellendi
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _redisService = redisService;
        }

        public async Task<UpdateProductResponseDto> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            var productToUpdate = await _unitOfWork.ProductRepository.GetAsync(p => p.Id == request.Id, cancellationToken: cancellationToken);

            if (productToUpdate == null)
                throw new NotFoundException($"{request.Id} kimliğine sahip ürün bulunamadı.");

            _mapper.Map(request, productToUpdate);

            await _unitOfWork.ProductRepository.UpdateAsync(productToUpdate, cancellationToken);
            await _unitOfWork.CompleteAsync(cancellationToken);

            await _redisService.RemoveDataAsync("products");
            await _redisService.RemoveDataAsync($"product:{request.Id}");

            UpdateProductResponseDto response = _mapper.Map<UpdateProductResponseDto>(productToUpdate);
            response.Message = "Ürün başarıyla güncellendi.";
            return response;
        }
    }
}