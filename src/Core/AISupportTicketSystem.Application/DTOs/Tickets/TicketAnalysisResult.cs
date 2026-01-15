using AISupportTicketSystem.Domain.Enums;

namespace AISupportTicketSystem.Application.DTOs.Tickets;

public record TicketAnalysisResult(
    string SuggestedCategory,
    TicketPriority Priority,
    SentimentType Sentiment,
    double SentimentScore,
    List<string> SuggestedTags);