using AutoMapper;
using ETicaret.Application.Features.Addresses.Commands.Create;
using ETicaret.Application.Features.Addresses.Commands.Update;
using ETicaret.Application.Features.Addresses.Queries.GetById;
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
        // Command -> Entity
        CreateMap<AddressAddCommand, Address>();
        CreateMap<AddressUpdateCommand, Address>();

        // Entity -> Command Response DTO
        CreateMap<Address, AddressAddResponseDto>();
        CreateMap<Address, AddressUpdateResponseDto>();

        // Entity -> Query Response DTO
        CreateMap<Address, GetListAddressResponseDto>(); // Liste içindeki tekil item için
        CreateMap<Address, GetByIdAddressResponseDto>();

        // IPaginate<Entity> -> GetListResponse<DTO> (Sayfalama için - Eğer GetListByUserIdQuery IPaginate döndürüyorsa)
        // Eğer GetListByUserIdQuery doğrudan List<> döndürüyorsa bu map'lemeye gerek yok.
        // Şimdilik yorumda bırakalım, çünkü GetListAsync'in liste döndüren versiyonunu kullanıyoruz.
        // CreateMap<IPaginate<Address>, GetListResponse<GetListAddressResponseDto>>().ReverseMap();
    }
}
