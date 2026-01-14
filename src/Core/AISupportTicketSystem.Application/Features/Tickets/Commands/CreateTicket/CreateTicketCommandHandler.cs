using AISupportTicketSystem.Application.DTOs.Tickets;
using AISupportTicketSystem.Application.Interfaces.Repositories;
using AISupportTicketSystem.Domain.Entities;
using AISupportTicketSystem.Domain.Enums;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AISupportTicketSystem.Application.Features.Tickets.Commands.CreateTicket;

public class CreateTicketCommandHandler : IRequestHandler<CreateTicketCommand, TicketDto>
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateTicketCommandHandler> _logger;

    public CreateTicketCommandHandler(IMapper mapper, IUnitOfWork unitOfWork, ILogger<CreateTicketCommandHandler> logger)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<TicketDto> Handle(CreateTicketCommand request, CancellationToken cancellationToken)
    {
        var ticketNumber = await _unitOfWork.Tickets.GenerateTicketNumberAsync();

        var ticket = new Ticket
        {
            Id = Guid.NewGuid(),
            TicketNumber = ticketNumber,
            Title = request.Title,
            Description = request.Description,
            Status = TicketStatus.New,
            Priority = request.Priority ?? TicketPriority.Medium,
            Source = request.Source ?? TicketSource.Web,
            CustomerId = request.CustomerId,
            CategoryId = request.CategoryId,
            CreatedAt = DateTime.UtcNow,
        };

        await _unitOfWork.Tickets.AddAsync(ticket);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        _logger.LogInformation("Ticket Created: {TicketNumber} by Customer: {CustomerId}", ticketNumber, request.CustomerId);

        var createdTicket = await _unitOfWork.Tickets.GetByIdWithDetailsAsync(ticket.Id);
        
        return _mapper.Map<TicketDto>(createdTicket);
    }
}