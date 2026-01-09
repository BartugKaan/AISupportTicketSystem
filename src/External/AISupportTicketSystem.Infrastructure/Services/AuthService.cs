using AISupportTicketSystem.Application.DTOs.Auth;
using AISupportTicketSystem.Application.Interfaces;
using AISupportTicketSystem.Domain.Entities;
using AISupportTicketSystem.Infrastructure.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace AISupportTicketSystem.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IJwtService _jwtService;
    private readonly ITokenBlacklistService _tokenBlacklistService;
    private readonly JwtSettings _jwtSettings;

    public AuthService(UserManager<ApplicationUser> userManager, IJwtService jwtService, ITokenBlacklistService tokenBlacklistService, IOptions<JwtSettings> jwtSettings)
    {
        _userManager = userManager;
        _jwtService = jwtService;
        _tokenBlacklistService = tokenBlacklistService;
        _jwtSettings = jwtSettings.Value;
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user == null || !user.IsActive)
        {
            return new AuthResponse(false, null, null, null, null, "Invalid Email or Password");
        }
        
        var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password);

        if (!isPasswordValid)
        {
            return new AuthResponse(false, null, null, null, null, "Invalid Email or Password");
        }
        
        user.LastLoginAt = DateTime.UtcNow;
        await _userManager.UpdateAsync(user);

        return await GenerateAuthResponseAsync(user);
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        var existingUser = await _userManager.FindByEmailAsync(request.Email);

        if (existingUser != null)
        {
            return  new AuthResponse(false, null, null, null, null, "Email already exists");
        }

        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            EmailConfirmed = true,
            IsActive = true,
            CreatedAt =  DateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(x => x.Description));
            return new AuthResponse(false, null, null, null, null, errors);
        }
        await _userManager.AddToRoleAsync(user, "Customer");
        return await GenerateAuthResponseAsync(user);
    }

    public async Task<AuthResponse> RefreshTokenAsync(RefreshTokenRequest request)
    {
        var principal = _jwtService.ValidateToken(request.AccessToken);

        if (principal == null)
        {
            return new AuthResponse(false, null, null, null, null, "Invalid access Token");
        }

        var userId = principal.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            return new AuthResponse(false, null, null, null, null, "Invalid access token");
        }

        var user = await _userManager.FindByIdAsync(userId);

        if (user == null || !user.IsActive)
        {
            return new AuthResponse(false, null, null, null, null, "User not found or inactive");
        }
        
        var tokenExpiry = TimeSpan.FromMinutes(_jwtSettings.ExpiryInMınutes);
        await _tokenBlacklistService.BlacklistTokenAsync(request.AccessToken, tokenExpiry);
        
        return await GenerateAuthResponseAsync(user);
    }

    public async Task LogoutAsync(string userId, string token)
    {
        var tokenExpiry = TimeSpan.FromMinutes(_jwtSettings.ExpiryInMınutes);
        await _tokenBlacklistService.BlacklistTokenAsync(token, tokenExpiry);

        await Task.CompletedTask;
    }

    public async Task<bool> IsTokenBlacklistedAsync(string token)
    {
        return await _tokenBlacklistService.IsBlacklistedAsync(token);
    }

    private async Task<AuthResponse> GenerateAuthResponseAsync(ApplicationUser user)
    {
        var roles = await _userManager.GetRolesAsync(user);

        var accessToken = _jwtService.GenerateAccessToken(user, roles);
        var refreshToken = _jwtService.GenerateRefreshToken();
        var expires = _jwtService.GetAccessTokenExpiry();

        var userDto = new UserDto(
            user.Id,
            user.Email!,
            user.FirstName,
            user.LastName,
            roles
            );
        return new AuthResponse(true, accessToken, refreshToken, expires, userDto, null);
    }
}