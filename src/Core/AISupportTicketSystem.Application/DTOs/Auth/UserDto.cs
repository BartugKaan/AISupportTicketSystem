namespace AISupportTicketSystem.Application.DTOs.Auth;

public record UserDto(
    Guid Id,
    string Email,
    string FirstName,
    string LastName,
    IList<string> Roles);