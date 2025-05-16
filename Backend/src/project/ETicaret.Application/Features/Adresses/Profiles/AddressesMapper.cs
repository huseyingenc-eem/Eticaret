using AutoMapper;
using ETicaret.Application.Features.Addresses.Commands.Create;
using ETicaret.Application.Features.Addresses.Commands.Update;
using ETicaret.Application.Features.Addresses.Queries.GetById;
using ETicaret.Application.Features.Addresses.Queries.GetListByUserId;
using ETicaret.Application.Features.Addresses.Queries.GetList;
using ETicaret.Domain.Entities;
using Core.Application.Mappings.Converters;
using Core.Persistence.Paging;

namespace ETicaret.Application.Features.Addresses.Profiles;

/// <summary>
/// Address entity'si ve ilgili DTO/Command'lar için AutoMapper profilini tanımlar.
/// </summary>
public class AddressesMapper : Profile
{
    public AddressesMapper()
    {
        // Commands
        CreateMap<CreateAddressCommand, Address>();
        CreateMap<UpdateAddressCommand, Address>();

        // Responses
        CreateMap<Address, CreateAddressResponseDto>();
        CreateMap<Address, UpdateAddressResponseDto>();
        CreateMap<Address, GetByIdAddressResponseDto>();
        CreateMap<Address, GetListByUserIdAddressResponseDto>();
        CreateMap<Address, GetListAddressResponseDto>();

        // Paging
        CreateMap<IPaginate<Address>, IPaginate<GetListAddressResponseDto>>()
            .ConvertUsing<PaginateTypeConverter<Address, GetListAddressResponseDto>>();

    }
}
