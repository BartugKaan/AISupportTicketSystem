using AISupportTicketSystem.Application.DTOs.Categories;
using AISupportTicketSystem.Application.Interfaces.Repositories;
using AutoMapper;
using MediatR;

namespace AISupportTicketSystem.Application.Features.Categories.Queries.GetAllCategories;

public class GetAllCategoriesQueryHandler : IRequestHandler<GetAllCategoriesQuery, IReadOnlyList<CategoryDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllCategoriesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<CategoryDto>> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
    {
        var categories = request.IncludeInactive
            ? await _unitOfWork.Categories.GetCategoriesWithSubCategoriesAsync()
            : await _unitOfWork.Categories.GetActiveCategoriesAsync();
        
        return _mapper.Map<IReadOnlyList<CategoryDto>>(categories);
    }
}