using AISupportTicketSystem.Application.DTOs.Tickets;
using AISupportTicketSystem.Application.Interfaces.Repositories;
using AutoMapper;
using MediatR;

namespace AISupportTicketSystem.Application.Features.Tickets.Queries.GetTicketById;

public class GetTicketByIdQueryHandler : IRequestHandler<GetTicketByIdQuery, TicketDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetTicketByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<TicketDto?> Handle(GetTicketByIdQuery request, CancellationToken cancellationToken)
    {
        var ticket = await _unitOfWork.Tickets.GetByIdWithDetailsAsync(request.Id);
        
        return ticket == null ? null : _mapper.Map<TicketDto>(ticket);
    }
}