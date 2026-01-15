using AISupportTicketSystem.Application.DTOs.Tickets;
using AISupportTicketSystem.Domain.Entities;
using AutoMapper;

namespace AISupportTicketSystem.Application.Mappings;

public class TicketMappingProfile : Profile
{
    public TicketMappingProfile()
    {
        // Ticket → TicketDto
        CreateMap<Ticket, TicketDto>()
            .ForMember(dest => dest.CustomerName, 
                opt => opt.MapFrom(src => src.Customer.FullName))
            .ForMember(dest => dest.CustomerEmail, 
                opt => opt.MapFrom(src => src.Customer.Email))
            .ForMember(dest => dest.AssignedAgentName, 
                opt => opt.MapFrom(src => src.AssignedAgent != null ? src.AssignedAgent.FullName : null))
            .ForMember(dest => dest.CategoryName, 
                opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : null));

        // Ticket → TicketListDto
        CreateMap<Ticket, TicketListDto>()
            .ForMember(dest => dest.CustomerName, 
                opt => opt.MapFrom(src => src.Customer.FullName))
            .ForMember(dest => dest.AssignedAgentName, 
                opt => opt.MapFrom(src => src.AssignedAgent != null ? src.AssignedAgent.FullName : null))
            .ForMember(dest => dest.CategoryName, 
                opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : null));

        // CreateTicketDto → Ticket
        CreateMap<CreateTicketDto, Ticket>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.TicketNumber, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.Ignore())
            .ForMember(dest => dest.CustomerId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());
        
        // TicketMessage -> TicketMessageDto
        CreateMap<TicketMessage, TicketMessageDto>()
            .ForMember(dest => dest.SenderName, 
                opt => opt.MapFrom(src => src.Sender.FullName))
            .ForMember(dest => dest.SenderEmail, 
                opt => opt.MapFrom(src => src.Sender.Email));
    }
}