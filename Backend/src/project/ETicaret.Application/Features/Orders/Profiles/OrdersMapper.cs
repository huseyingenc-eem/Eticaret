using AutoMapper;
using ETicaret.Application.Features.Orders.Commands.Create;
using ETicaret.Application.Features.Orders.Commands.UpdateStatus;
using ETicaret.Application.Features.Orders.Queries.GetById; 
using ETicaret.Application.Features.Orders.Queries.GetOrdersByUserId; 
using ETicaret.Application.Features.Orders.Queries.GetListForEmployee; 
using ETicaret.Domain.Entities; 
namespace ETicaret.Application.Features.Orders.Profiles;

public class OrdersMapper : Profile
{
    public OrdersMapper()
    {
        // --- Command Response Mappings ---
        CreateMap<Order, OrderAddResponseDto>()
            .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.OrderStatus, opt => opt.MapFrom(src => src.Status.ToString()));

        CreateMap<Order, OrderUpdateStatusResponse>()
            .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.CurrentStatus, opt => opt.MapFrom(src => src.Status.ToString()));

        CreateMap<Order, GetOrdersByUserIdResponseDto>()
            .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

        CreateMap<Order, GetOrderByIdResponseDto>()
            .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.ShippingAddress, opt => opt.MapFrom(src => src.ShippingAddress))
            .ForMember(dest => dest.BillingAddress, opt => opt.MapFrom(src => src.BillingAddress))
            .ForMember(dest => dest.OrderItems, opt => opt.MapFrom(src => src.OrderItems)); 

        CreateMap<Order, GetOrderListForEmployeeResponseDto>()
            .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.CustomerFirstName, opt => opt.MapFrom(src => src.User != null ? src.User.FirstName : null))
            .ForMember(dest => dest.CustomerLastName, opt => opt.MapFrom(src => src.User != null ? src.User.LastName : null))  
            .ForMember(dest => dest.CustomerEmail, opt => opt.MapFrom(src => src.User != null ? src.User.Email : null))
            .ForMember(dest => dest.ShippingCity, opt => opt.MapFrom(src => src.ShippingAddress != null ? src.ShippingAddress.City : null))
            .ForMember(dest => dest.ShippingDistrict, opt => opt.MapFrom(src => src.ShippingAddress != null ? src.ShippingAddress.District : null));

        CreateMap<OrderItem, GetOrderByIdResponseDto.OrderItemDto>()
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product != null ? src.Product.Name : null))
            .ForMember(dest => dest.ProductImageUrl, opt => opt.MapFrom(src => src.Product != null ? src.Product.ImageUrl : null));
                                                                                                    
        CreateMap<Address, GetOrderByIdResponseDto.AddressDto>();
    }
}
