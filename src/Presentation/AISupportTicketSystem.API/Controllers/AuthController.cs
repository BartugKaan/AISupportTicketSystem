using System.Security.Claims;
using AISupportTicketSystem.Application.DTOs.Auth;
using AISupportTicketSystem.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AISupportTicketSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        _logger.LogInformation("Login attempt for {Email}", request.Email);

        var response = await _authService.LoginAsync(request);

        if (!response.Success)
        {
            _logger.LogWarning("Login Failed for {Email}: {Error}", request.Email, response.Error);
            return Unauthorized(response);
        }
        
        _logger.LogInformation("Login successful for {Email}", request.Email);
        return Ok(response);
    }

    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        _logger.LogInformation("Registration attempt for {Email}", request.Email);

        var response = await _authService.RegisterAsync(request);

        if (!response.Success)
        {
            _logger.LogWarning("Registration is failed for {Email} : {Error}", request.Email, response.Error);
            return  BadRequest(response);
        }
        
        _logger.LogInformation("Registration is successful for {Email}", request.Email);
        return CreatedAtAction(nameof(Login), response);
    }

    [HttpPost("refreshToken")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        var response = await _authService.RefreshTokenAsync(request);

        if (!response.Success)
        {
            _logger.LogWarning("Token Refresh failed: {Error}",  response.Error);
            return Unauthorized(response);
        }
        
        _logger.LogInformation("Token Refreshed for User {UserId}", response.User?.Id);
        return Ok(response);
    }

    [HttpPost("logout")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Logout()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var token = HttpContext.Request.Headers["Authorization"]
            .ToString()
            .Replace("Bearer ", "");

        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
        {
            return Unauthorized();
        }
        
        await _authService.LogoutAsync(userId, token);
        
        _logger.LogInformation("User {UserId} logged out}", userId);
        return NoContent();
    }

    [HttpGet("me")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult GetCurrentUser()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        var name = User.FindFirst(ClaimTypes.Name)?.Value;
        var roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }
        
        var nameParts = name?.Split(' ') ?? Array.Empty<string>();
        var firstName = nameParts.FirstOrDefault() ?? "";
        var lastName = nameParts.Length > 1 ? string.Join(" ", nameParts.Skip(1)) : "";

        var user = new UserDto(
            Guid.Parse(userId),
            email ?? "",
            firstName,
            lastName, 
            roles);
        
        return Ok(user);
    }
    
    
}