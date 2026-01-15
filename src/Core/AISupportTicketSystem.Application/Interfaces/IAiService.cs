using AISupportTicketSystem.Application.DTOs.Tickets;

namespace AISupportTicketSystem.Application.Interfaces;

public interface IAiService
{
    Task<TicketAnalysisResult> AnalyzeTicketAsync(string title, string description);
    Task<string> GenerateSuggestedResponseAsync(string title, string description, string? categoryName);
    Task<float[]> GenerateEmbeddingAsync(string text);
}