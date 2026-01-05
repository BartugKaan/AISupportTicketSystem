namespace AISupportTicketSystem.Infrastructure.Settings;

public class JwtSettings
{
    public const string SectionName = "JwtSettings";

    public string SecretKey { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int ExpiryInMınutes { get; set; }
    public int RefreshTokenExpiryInDays { get; set; } = 7;
}