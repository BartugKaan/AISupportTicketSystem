using FluentValidation;

namespace AISupportTicketSystem.Application.Features.Tickets.Commands.AddMessage;

public class AddMessageCommandValidator : AbstractValidator<AddMessageCommand>
{
    public AddMessageCommandValidator()
    {
        RuleFor(x => x.TicketId)
            .NotEmpty().WithMessage("Ticket ID is required");

        RuleFor(x => x.SenderId)
            .NotEmpty().WithMessage("Sender ID is required");

        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Message content is required")
            .MinimumLength(1).WithMessage("Message cannot be empty")
            .MaximumLength(8000).WithMessage("Message cannot exceed 8000 characters");
    }
}