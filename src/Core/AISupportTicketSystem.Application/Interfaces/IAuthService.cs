using AISupportTicketSystem.Application.DTOs.Auth;

namespace AISupportTicketSystem.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResponse> LoginAsync(LoginRequest request);
    Task<AuthResponse> RegisterAsync(RegisterRequest request);
    Task<AuthResponse> RefreshTokenAsync(RefreshTokenRequest request);
    Task LogoutAsync(string userId, string token);
    Task<bool> IsTokenBlacklistedAsync(string token);
}