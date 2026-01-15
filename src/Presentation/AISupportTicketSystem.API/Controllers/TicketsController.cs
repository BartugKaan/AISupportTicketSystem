using System.Security.Claims;
using AISupportTicketSystem.Application.DTOs.Common;
using AISupportTicketSystem.Application.DTOs.Tickets;
using AISupportTicketSystem.Application.Features.Tickets.Commands.AssignTicket;
using AISupportTicketSystem.Application.Features.Tickets.Commands.ChangeStatus;
using AISupportTicketSystem.Application.Features.Tickets.Commands.CreateTicket;
using AISupportTicketSystem.Application.Features.Tickets.Commands.DeleteTicket;
using AISupportTicketSystem.Application.Features.Tickets.Commands.UpdateTicket;
using AISupportTicketSystem.Application.Features.Tickets.Queries.GetAllTickets;
using AISupportTicketSystem.Application.Features.Tickets.Queries.GetMyTickets;
using AISupportTicketSystem.Application.Features.Tickets.Queries.GetTicketById;
using AISupportTicketSystem.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

namespace AISupportTicketSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TicketsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<TicketsController> _logger;

    public TicketsController(IMediator mediator, ILogger<TicketsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    private Guid GetCurrentUserId()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.Parse(userId!);
    }

    private bool IsInRole(string role)
    {
        return User.IsInRole(role);
    }

    /// <summary>
    /// Get all tickets with filtering and pagination (Admin, Supervisor, Agent)
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<TicketListDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] TicketStatus? status = null,
        [FromQuery] TicketPriority? priority = null,
        [FromQuery] Guid? customerId = null,
        [FromQuery] Guid? agentId = null,
        [FromQuery] string? searchTerm = null,
        [FromQuery] string? sortBy = null,
        [FromQuery] bool sortDescending = true)
    {
        var query = new GetAllTicketsQuery(
            pageNumber,
            pageSize,
            status,
            priority,
            customerId,
            agentId,
            searchTerm,
            sortBy,
            sortDescending);
        
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get tickets by Id 
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(TicketDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var ticket = await _mediator.Send(new GetTicketByIdQuery(id));
        
        if(ticket == null) return NotFound(new {message = "Ticket not found."});

        if (IsInRole("Customer") && ticket.CustomerId != GetCurrentUserId()) return Forbid();
        
        return Ok(ticket);
    }

    /// <summary>
    /// Get my tickets (Customer: own tickets, Agent: assigned ticket)
    /// </summary>
    [HttpGet("my-tickets")]
    [ProducesResponseType(typeof(IReadOnlyList<TicketListDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyTickets()
    {
        var userId = GetCurrentUserId();
        var isAgent = IsInRole("Agent") || IsInRole("Supervisor") || IsInRole("Admin");

        var query = new GetMyTicketsQuery(userId, isAgent);
        var result = await _mediator.Send(query);
        
        return Ok(result);
    }

    /// <summary>
    /// Create a new ticket (Customer Only)
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(TicketDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateTicketDto dto)
    {
        var command = new CreateTicketCommand(
            dto.Title,
            dto.Description,
            dto.Priority,
            dto.Source,
            dto.CategoryId,
            GetCurrentUserId());

        var result = await _mediator.Send(command);
        
        _logger.LogInformation("Ticket created: {TicketId}", result.TicketNumber);

        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }


    /// <summary>
    /// Update a ticket
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(TicketDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTicketDto dto)
    {
        var existingTicket = await _mediator.Send(new GetTicketByIdQuery(id));

        if (existingTicket == null) return NotFound(new { message = "Ticket not Found" });

        if (IsInRole("Customer") && existingTicket.CustomerId != GetCurrentUserId()) return Forbid();

        var command = new UpdateTicketCommand(
            id,
            dto.Title,
            dto.Description,
            dto.Priority,
            dto.CategoryId
        );

        var result = await _mediator.Send(command);
        _logger.LogInformation("Ticket Updated : {TicketNumber}", result.TicketNumber);
        return Ok(result);
    }

    /// <summary>
    /// Delete a Ticket (Admin Only)
    /// </summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _mediator.Send(new DeleteTicketCommand(id));
        return NoContent();
    }

    /// <summary>
    /// Assing Ticket to an agent(Supervisor, Admin)
    /// </summary>
    [HttpPatch("{id:guid}/assign")]
    [Authorize(Roles = "Admin,Supervisor")]
    [ProducesResponseType(typeof(TicketDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Assign(Guid id, [FromBody] AssignTicketDto dto)
    {
        var command = new AssignTicketCommand(id, dto.AgentId);
        var result = await _mediator.Send(command);
        
        _logger.LogInformation("Ticket {Id} assigned to agent {AgentId}", id, dto.AgentId);
        return Ok(result);
    }

    /// <summary>
    /// Change ticket status (Agent, Supervisor, Admin)
    /// </summary>
    [HttpPatch("{id:guid}/status")]
    [Authorize(Roles = "Admin,Supervisor,Agent")]
    [ProducesResponseType(typeof(TicketDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ChangeStatus(Guid id, [FromBody] ChangeStatusDto dto)
    {
        var command = new ChangeStatusCommand(id, dto.Status);
        var result = await _mediator.Send(command);
        
        _logger.LogInformation("Ticket {Id} status changed to {Status}", id, dto.Status);
        return Ok(result);
    }
}