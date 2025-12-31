using System.Security.Claims;
using AISupportTicketSystem.Domain.Entities;

namespace AISupportTicketSystem.Application.Interfaces;

public interface IJwtService
{
    string GenerateAccessToken(ApplicationUser user, IList<string> roles);
    string GenerateRefreshToken();
    ClaimsPrincipal? ValidateToken(string token);
    DateTime GetAccessTokenExpiry();
}