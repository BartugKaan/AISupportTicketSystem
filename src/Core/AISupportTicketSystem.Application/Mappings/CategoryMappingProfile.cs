using AISupportTicketSystem.Application.DTOs.Categories;
using AISupportTicketSystem.Domain.Entities;
using AutoMapper;

namespace AISupportTicketSystem.Application.Mappings;

public class CategoryMappingProfile : Profile
{
    public CategoryMappingProfile()
    {
        CreateMap<Category, CategoryDto>()
            .ForMember(dest => dest.SubCategories, opt => opt.MapFrom(src => src.SubCategories));
    }
}