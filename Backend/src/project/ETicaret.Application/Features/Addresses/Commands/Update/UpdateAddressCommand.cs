using AutoMapper;
using Core.Application.Abstractions.Messaging; // <-- IAuthenticatedRequest için
using Core.Application.Abstractions.Repositories;
using Core.Application.Behaviors.Transactional;
using Core.Application.Common.Exceptions;
using ETicaret.Application.Features.Addresses.Specifications; // <-- Yeni spesifikasyonumuz
using ETicaret.Domain.Entities;
using MediatR;

namespace ETicaret.Application.Features.Addresses.Commands.Update;

/// <summary>
/// Mevcut bir adresi güncelleme işlemini temsil eden komut.
/// ITransactionalRequest: Bu işlemin tamamının bir transaction içinde çalışmasını sağlar.
/// IAuthenticatedRequest: Bu komuta, isteği yapan kullanıcının kimliğinin (UserId) otomatik atanmasını sağlar.
/// </summary>
public class UpdateAddressCommand : IRequest<UpdateAddressResponseDto>, ITransactionalRequest, IAuthenticatedRequest
{
    #region Komut Parametreleri
    public Guid Id { get; set; } // Hangi adresin güncelleneceği
    public string UserId { get; set; } // Middleware tarafından otomatik doldurulacak
    public string AddressTitle { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string District { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
    public string AddressLine { get; set; }
    public string? PostalCode { get; set; }
    public bool IsDefaultBilling { get; set; }
    public bool IsDefaultShipping { get; set; }
    #endregion

    /// <summary>
    /// UpdateAddressCommand isteğini işleyen Handler.
    /// </summary>
    public class UpdateAddressCommandHandler : IRequestHandler<UpdateAddressCommand, UpdateAddressResponseDto>
    {
        private readonly IMapper _mapper;
        private readonly IRepository<Address, Guid> _addressRepository;

        public UpdateAddressCommandHandler(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            // Bağımlılığımızı IUnitOfWork üzerinden alıyoruz.
            _addressRepository = unitOfWork.GetRepository<Address, Guid>();
        }

        public async Task<UpdateAddressResponseDto> Handle(UpdateAddressCommand request, CancellationToken cancellationToken)
        {
            // 1. Yeni spesifikasyonumuzu kullanarak güncellenecek adresi veritabanından alıyoruz.
            var spec = new AddressSpecifications.ById(request.Id);
            Address? addressToUpdate = await _addressRepository.GetAsync(spec, cancellationToken);

            // 2. Varlık Kontrolü: Adres bulunamazsa, özel bir exception fırlat.
            if (addressToUpdate == null)
            {
                throw new NotFoundException($"ID'si {request.Id} olan adres bulunamadı.");
            }

            // 3. Yetki Kontrolü: Adresin sahibi, isteği yapan kullanıcı mı?
            // request.UserId, AssignUserIdMiddleware tarafından otomatik olarak doldurulur.
            if (addressToUpdate.UserId != request.UserId)
            {
                throw new AuthorizationException("Bu adresi güncelleme yetkiniz bulunmamaktadır.");
            }

            // 4. AutoMapper ile request'ten gelen verileri, veritabanından çektiğimiz entity'e aktar.
            _mapper.Map(request, addressToUpdate);

            // 5. Varlığın güncellenmek üzere işaretlenmesini sağla.
            await _addressRepository.UpdateAsync(addressToUpdate, cancellationToken);

            // 6. CompleteAsync çağrısını kaldırıyoruz.
            // Bu komut ITransactionalRequest'i uyguladığı için, TransactionBehavior
            // bu handler başarılı olduğunda kaydetme (Complete) ve onaylama (Commit)
            // işlemlerini bizim için otomatik yapacaktır.

            UpdateAddressResponseDto response = _mapper.Map<UpdateAddressResponseDto>(addressToUpdate);
            response.Message = "Adres başarıyla güncellendi.";

            return response;
        }
    }
}