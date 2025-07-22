using AutoMapper;
using ETicaret.Application.Features.Addresses.Commands.Create;
using ETicaret.Application.Features.Addresses.Commands.Delete;
using ETicaret.Application.Features.Addresses.Commands.Update;
using ETicaret.Application.Features.Addresses.Queries.GetById;
using ETicaret.Application.Features.Addresses.Queries.GetList;
using ETicaret.Application.Features.Addresses.Queries.GetListByUserId;
using ETicaret.Domain.Entities;

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

        // Create
        CreateMap<Address, CreateAddressResponseDto>().ReverseMap();

        // Update
        CreateMap<Address, UpdateAddressResponseDto>().ReverseMap();

        //Delete
        CreateMap<Address, DeleteAddressResponseDto>().ReverseMap();
        CreateMap<Address, GetListAddressResponseDto>();

        CreateMap<Address, GetByIdAddressResponseDto>();
        CreateMap<Address, GetListByUserIdAddressResponseDto>();


    }
}
