namespace AISupportTicketSystem.Application.Exceptions;

public class ValidationException : Exception
{
    public Dictionary<string, string[]> Errors { get;}

    public ValidationException() : base("One or more validation errors occured.")
    {
        Errors = new Dictionary<string, string[]>();
    }

    public ValidationException(Dictionary<string, string[]> errors) : this()
    {
        Errors = errors;
    }
}