namespace AISupportTicketSystem.Application.DTOs.Auth;

public record LoginRequest(
    string Email,
    string Password);