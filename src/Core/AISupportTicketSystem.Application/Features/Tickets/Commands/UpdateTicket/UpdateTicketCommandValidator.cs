using FluentValidation;

namespace AISupportTicketSystem.Application.Features.Tickets.Commands.UpdateTicket;

public class UpdateTicketCommandValidator : AbstractValidator<UpdateTicketCommand>
{
    public UpdateTicketCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Ticket ID is required");

        RuleFor(x => x.Title)
            .MinimumLength(5).When(x => !string.IsNullOrWhiteSpace(x.Title))
            .WithMessage("Title must be at least 5 characters")
            .MaximumLength(200).When(x => !string.IsNullOrWhiteSpace(x.Title))
            .WithMessage("Title cannot exceed 200 characters");

        RuleFor(x => x.Description)
            .MinimumLength(10).When(x => !string.IsNullOrWhiteSpace(x.Description))
            .WithMessage("Description must be at least 10 characters")
            .MaximumLength(4000).When(x => !string.IsNullOrWhiteSpace(x.Description))
            .WithMessage("Description cannot exceed 4000 characters");

        RuleFor(x => x.Priority)
            .IsInEnum().When(x => x.Priority.HasValue)
            .WithMessage("Invalid priority value");
    }
}