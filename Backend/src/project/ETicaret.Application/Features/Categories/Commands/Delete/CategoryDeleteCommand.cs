using AutoMapper;
using ETicaret.Application.Services.Repositories;
using MediatR;

namespace ETicaret.Application.Features.Categories.Commands.Update;

public class CategoryDeleteCommand : IRequest<string>
{
    public int Id { get; set; }

    public class CategoryDeleteCommandHandler : IRequestHandler<CategoryDeleteCommand, string>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CategoryDeleteCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<string> Handle(CategoryDeleteCommand request, CancellationToken cancellationToken)
        {
            var category = await _unitOfWork.CategoryRepository.GetAsync(
                 filter: x => x.Id == request.Id,
                 include: false,
                 enableTracking: true,
                 cancellationToken: cancellationToken
             );

            if (category == null)
                return $"Silinecek kategori bulunamadı (ID: {request.Id}).";

            await _unitOfWork.CategoryRepository.DeleteAsync(category, cancellationToken);
            await _unitOfWork.CompleteAsync(cancellationToken);
            return "Kategori başarıyla silindi.";
        }
    }
}