using AISupportTicketSystem.Application.DTOs.Auth;
using FluentValidation;

namespace AISupportTicketSystem.Application.Validators.Auth;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format")
            .MaximumLength(256).WithMessage("Email length should not exceed 256 characters");
        
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(8).WithMessage("Password length should be at least 8 characters")
            .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter")
            .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter")
            .Matches("[0-9]").WithMessage("Password must contain at least one digit");
        
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required")
            .MaximumLength(100).WithMessage("First name length should not exceed 100 characters")
            .Matches("^[a-zA-ZğüşıöçĞÜŞİÖÇ\\s]+$").WithMessage("First name can only contain letters");
        
        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required")
            .MaximumLength(100).WithMessage("Last name length should not exceed 100 characters")
            .Matches("^[a-zA-ZğüşıöçĞÜŞİÖÇ\\s]+$").WithMessage("Last name can only contain letters");
    }
}