using FluentValidation;

namespace AISupportTicketSystem.Application.Features.Tickets.Commands.CreateTicket;

public class CreateTicketCommandValidator : AbstractValidator<CreateTicketCommand>
{
    public CreateTicketCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MinimumLength(5).WithMessage("Title must be at least 5 characters")
            .MaximumLength(200).WithMessage("Title cannot exceed 200 characters");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required")
            .MinimumLength(10).WithMessage("Description must be at least 10 characters")
            .MaximumLength(4000).WithMessage("Description cannot exceed 4000 characters");

        RuleFor(x => x.CustomerId)
            .NotEmpty().WithMessage("Customer ID is required");

        RuleFor(x => x.Priority)
            .IsInEnum().When(x => x.Priority.HasValue)
            .WithMessage("Invalid priority value");

        RuleFor(x => x.Source)
            .IsInEnum().When(x => x.Source.HasValue)
            .WithMessage("Invalid source value");
    }
}