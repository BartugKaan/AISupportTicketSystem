using System.ClientModel;
using System.Text.Json;
using AISupportTicketSystem.Application.DTOs.Tickets;
using AISupportTicketSystem.Application.Interfaces;
using AISupportTicketSystem.Domain.Enums;
using AISupportTicketSystem.Infrastructure.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenAI;
using OpenAI.Chat;
using OpenAI.Embeddings;

namespace AISupportTicketSystem.Infrastructure.Services;

public class OpenAIService : IAiService
{
    private readonly ChatClient _chatClient;
    private readonly EmbeddingClient _embeddingClient;
    private readonly ILogger<OpenAIService> _logger;
    private readonly OpenAISettings _settings;

    public OpenAIService(IOptions<OpenAISettings> settings, ILogger<OpenAIService> logger)
    {
        _settings = settings.Value;
        _logger = logger;

        var client = new OpenAIClient(_settings.ApiKey);
        _chatClient = client.GetChatClient(_settings.Model);
        _embeddingClient = client.GetEmbeddingClient(_settings.EmbeddingModel);
    }

    public async Task<TicketAnalysisResult> AnalyzeTicketAsync(string title, string description)
    {
        try
        {
            var prompt = $$"""
                                     Analyze this customer support ticket and provide a JSON response.

                                     Title: {{title}}
                                     Description: {{description}}

                                     Respond with ONLY a valid JSON object (no markdown, no explanation) in this exact format:
                                     {
                                           "suggestedCategory": "one of: Technical Support, Billing, Account, Feature Request, General Inquiry",
                                           "suggestedPriority": "one of: Low, Medium, High, Critical",
                                           "sentiment": "one of: Positive, Neutral, Negative, Angry",
                                           "sentimentScore": 0.0 to 1.0 (how intense the sentiment is),
                                           "suggestedTags": ["tag1", "tag2", "tag3"]
                                     }
                                      Consider these factors:
                                       - Category: Based on the topic (payment issues = Billing, bugs = Technical Support, etc.)
                                       - Priority: Critical for urgent issues, security problems, or angry customers
                                       - Sentiment: Analyze the emotional tone of the message
                                       - Tags: Extract relevant keywords for searchability
                           """;

            var messages = new List<ChatMessage>
            {
                new SystemChatMessage("You are a customer support ticket analyzer. Always respond with valid JSON only."),
                new UserChatMessage(prompt)
            };

            var response = await _chatClient.CompleteChatAsync(messages);
            var content = response.Value.Content[0].Text;

            _logger.LogDebug("OpenAI Analysis Response: {Response}", content);

            // JSON parse
            var analysisResult = JsonSerializer.Deserialize<AIAnalysisResponse>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (analysisResult == null)
                throw new Exception("Failed to parse AI response");

            return new TicketAnalysisResult(
                analysisResult.SuggestedCategory,
                ParsePriority(analysisResult.SuggestedPriority),
                ParseSentiment(analysisResult.Sentiment),
                analysisResult.SentimentScore,
                analysisResult.SuggestedTags
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error analyzing ticket with OpenAI");
            
            // Fallback değerler
            return new TicketAnalysisResult(
                "General Inquiry",
                TicketPriority.Medium,
                SentimentType.Neutral,
                0.5,
                new List<string>()
            );
        }
    }

    public async Task<string> GenerateSuggestedResponseAsync(string title, string description, string? categoryName)
    {
        try
        {
            var prompt = $"""
                Generate a professional and empathetic customer support response for this ticket.

                Category: {categoryName ?? "General"}
                Title: {title}
                Customer Message: {description}

                Requirements:
                - Be professional, friendly, and empathetic
                - Acknowledge the customer's concern
                - Provide helpful information or next steps
                - Keep it concise (2-3 paragraphs max)
                - Use a warm closing
                - Do NOT include placeholders like [Name] - write as if ready to send
                
                Write the response in the same language as the customer's message.
                """;

            var messages = new List<ChatMessage>
            {
                new SystemChatMessage("You are a helpful and empathetic customer support agent."),
                new UserChatMessage(prompt)
            };

            var response = await _chatClient.CompleteChatAsync(messages);
            var content = response.Value.Content[0].Text;

            _logger.LogDebug("OpenAI Response Suggestion: {Response}", content);

            return content;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating response with OpenAI");
            return "Thank you for contacting us. We have received your request and will get back to you shortly.";
        }
    }

    public async Task<float[]> GenerateEmbeddingAsync(string text)
    {
        try
        {
            var response = await _embeddingClient.GenerateEmbeddingAsync(text);
            var embedding = response.Value.ToFloats();
            
            return embedding.ToArray();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating embedding with OpenAI");
            return Array.Empty<float>();
        }
    }

    private static TicketPriority ParsePriority(string priority) => priority.ToLower() switch
    {
        "low" => TicketPriority.Low,
        "medium" => TicketPriority.Medium,
        "high" => TicketPriority.High,
        "critical" => TicketPriority.Critical,
        _ => TicketPriority.Medium
    };

    private static SentimentType ParseSentiment(string sentiment) => sentiment.ToLower() switch
    {
        "positive" => SentimentType.Positive,
        "neutral" => SentimentType.Neutral,
        "negative" => SentimentType.Negative,
        "angry" => SentimentType.Angry,
        _ => SentimentType.Neutral
    };

    private class AIAnalysisResponse
    {
        public string SuggestedCategory { get; set; } = string.Empty;
        public string SuggestedPriority { get; set; } = string.Empty;
        public string Sentiment { get; set; } = string.Empty;
        public double SentimentScore { get; set; }
        public List<string> SuggestedTags { get; set; } = new();
    }
}