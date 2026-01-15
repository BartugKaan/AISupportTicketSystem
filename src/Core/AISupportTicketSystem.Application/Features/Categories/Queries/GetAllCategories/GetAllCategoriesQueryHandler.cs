using AISupportTicketSystem.Application.Constants;
using AISupportTicketSystem.Application.DTOs.Categories;
using AISupportTicketSystem.Application.Interfaces;
using AISupportTicketSystem.Application.Interfaces.Repositories;
using AutoMapper;
using MediatR;

namespace AISupportTicketSystem.Application.Features.Categories.Queries.GetAllCategories;

public class GetAllCategoriesQueryHandler : IRequestHandler<GetAllCategoriesQuery, IReadOnlyList<CategoryDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ICacheService _cacheService;

    public GetAllCategoriesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ICacheService cacheService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _cacheService = cacheService;
    }

    public async Task<IReadOnlyList<CategoryDto>> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
    {
        if (request.IncludeInactive)
        {
            var allCategories = await _unitOfWork.Categories.GetCategoriesWithSubCategoriesAsync();
            return _mapper.Map<IReadOnlyList<CategoryDto>>(allCategories);
        }

        var cacheKey = CacheKeys.CategoriesAll;
        
        var cachedCategories = await _cacheService.GetOrSetAsync(
            cacheKey,
            async () =>
            {
                var categories = await _unitOfWork.Categories.GetActiveCategoriesAsync();
                return _mapper.Map<List<CategoryDto>>(categories);
            },
            TimeSpan.FromHours(1)); 

        return cachedCategories;
    }
}