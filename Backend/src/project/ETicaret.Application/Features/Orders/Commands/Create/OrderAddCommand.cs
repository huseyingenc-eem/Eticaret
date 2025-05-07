using AutoMapper;
using Core.CrossCuttingConcerns.Exceptions;
using ETicaret.Application.Services.Repositories;
using ETicaret.Domain.Entities;
using ETicaret.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace ETicaret.Application.Features.Orders.Commands.Create;

public class OrderAddCommand : IRequest<OrderAddResponseDto>
{
    // Handler içinde HttpContext'ten alınacaksa UserId'ye gerek yok.
    public int ShippingAddressId { get; set; }
    public int BillingAddressId { get; set; }
    public List<OrderItemDto> OrderItems { get; set; } = new();

    // İç DTO
    public class OrderItemDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }

    public class OrderAddCommandHandler : IRequestHandler<OrderAddCommand, OrderAddResponseDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public OrderAddCommandHandler(IUnitOfWork unitOfWork,IMapper mapper,IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<OrderAddResponseDto> Handle(OrderAddCommand request, CancellationToken cancellationToken)
        {
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                throw new AuthorizationException("Sipariş oluşturmak için giriş yapmalısınız.");

            // 2. Adres Kontrolleri (UoW üzerinden)
            var shippingAddress = await _unitOfWork.AddressRepository.GetAsync(a => a.Id == request.ShippingAddressId && a.UserId == userId, cancellationToken: cancellationToken);
            if (shippingAddress == null) throw new BusinessException($"Teslimat adresi bulunamadı veya kullanıcıya ait değil (ID: {request.ShippingAddressId}).");

            var billingAddress = await _unitOfWork.AddressRepository.GetAsync(a => a.Id == request.BillingAddressId && a.UserId == userId, cancellationToken: cancellationToken);
            if (billingAddress == null) throw new BusinessException($"Fatura adresi bulunamadı veya kullanıcıya ait değil (ID: {request.BillingAddressId}).");

            // 3. Ürün ve Stok Kontrolleri & Fiyat Hesaplama (UoW üzerinden)
            decimal totalAmount = 0;
            var orderItemsToCreate = new List<OrderItem>();
            var fetchedProducts = new Dictionary<int, Product>(); // Context tarafından izlenen ürünleri tut

            foreach (var itemDto in request.OrderItems)
            {
                if (!fetchedProducts.TryGetValue(itemDto.ProductId, out var product))
                {
                    // UoW üzerinden ProductRepository kullan
                    product = await _unitOfWork.ProductRepository.GetAsync(p => p.Id == itemDto.ProductId && p.IsActive, cancellationToken: cancellationToken);
                    if (product == null) throw new BusinessException($"Ürün bulunamadı veya aktif değil (ID: {itemDto.ProductId}).");
                    fetchedProducts.Add(itemDto.ProductId, product); 
                }

                var totalRequestedQuantityForProduct = request.OrderItems.Where(i => i.ProductId == itemDto.ProductId).Sum(i => i.Quantity);
                if (product.Stock < totalRequestedQuantityForProduct)
                    throw new BusinessException($"Yetersiz stok: {product.Name} (Mevcut Stok: {product.Stock}, Toplam İstenen: {totalRequestedQuantityForProduct}).");

                var orderItem = new OrderItem { ProductId = product.Id, Quantity = itemDto.Quantity, Price = product.Price };
                orderItemsToCreate.Add(orderItem);
                totalAmount += product.Price * itemDto.Quantity;
            }

            foreach (var productEntry in fetchedProducts)
            {
                var totalQuantity = request.OrderItems.Where(i => i.ProductId == productEntry.Key).Sum(i => i.Quantity);
                productEntry.Value.Stock -= totalQuantity; 
            }

            // 5. Order Entity'sini Oluştur
            var newOrder = new Order
            {
                UserId = userId,
                ShippingAddressId = request.ShippingAddressId,
                BillingAddressId = request.BillingAddressId,
                OrderDate = DateTime.UtcNow,
                TotalAmount = totalAmount,
                Status = OrderStatus.Pending,
                OrderItems = orderItemsToCreate
            };

            await _unitOfWork.OrderRepository.AddAsync(newOrder);
            await _unitOfWork.CompleteAsync(cancellationToken);

            // 8. Response Döndür
            // CompleteAsync sonrası newOrder'ın Id alanı dolmuş olmalı
            var response = _mapper.Map<OrderAddResponseDto>(newOrder);
            response.OrderStatus = newOrder.Status.ToString();
            return response;
        }
    
    }
}
