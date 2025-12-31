namespace AISupportTicketSystem.Application.DTOs.Auth;

public record AuthResponse(
    bool Success,
    string? AccessToken,
    string? RefreshToken,
    DateTime? ExpiresAt,
    UserDto? User,
    string? error);