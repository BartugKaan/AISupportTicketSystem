using AISupportTicketSystem.Application.DTOs.Auth;
using FluentValidation;

namespace AISupportTicketSystem.Application.Validators.Auth;

public class RefreshTokenRequestValidator : AbstractValidator<RefreshTokenRequest>
{
    public RefreshTokenRequestValidator()
    {
        RuleFor(x => x.AccessToken)
            .NotEmpty().WithMessage("Access token is required");
        RuleFor(x => x.RefreshToken)
            .NotEmpty().WithMessage("Refresh token is required");
    }
}