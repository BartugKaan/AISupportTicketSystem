namespace AISupportTicketSystem.Infrastructure.Settings;

public class OpenAISettings
{
    public const string SectionName = "OpenAI";

    public string ApiKey { get; set; } = string.Empty;
    public string Model { get; set; } = "gpt-3.5-turbo";
    public string EmbeddingModel { get; set; } = "text-embedding-3-small";
}