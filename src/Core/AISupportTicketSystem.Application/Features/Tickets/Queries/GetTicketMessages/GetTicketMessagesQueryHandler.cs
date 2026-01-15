using AISupportTicketSystem.Application.DTOs.Tickets;
using AISupportTicketSystem.Application.Interfaces.Repositories;
using AutoMapper;
using MediatR;

namespace AISupportTicketSystem.Application.Features.Tickets.Queries.GetTicketMessages;

public class GetTicketMessagesQueryHandler : IRequestHandler<GetTicketMessagesQuery, IReadOnlyList<TicketMessageDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetTicketMessagesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<TicketMessageDto>> Handle(GetTicketMessagesQuery request, CancellationToken cancellationToken)
    {
        var message = await _unitOfWork.TicketMessages.GetByTicketIdAsync(
            request.TicketId,
            request.IncludeInternal);

        return _mapper.Map<IReadOnlyList<TicketMessageDto>>(message);
    }
}