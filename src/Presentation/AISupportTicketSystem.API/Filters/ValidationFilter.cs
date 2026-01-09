using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AISupportTicketSystem.API.Filters;

public class ValidationFilter : IAsyncActionFilter
{
    
    private readonly IServiceProvider _serviceProvider;

    public ValidationFilter(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        foreach (var argument in context.ActionArguments.Values)
        {
            if(argument == null) continue;
            
            var argumentType = argument.GetType();
            var validationType = typeof(IValidator<>).MakeGenericType(argumentType);
            var validator = _serviceProvider.GetService(validationType) as IValidator;
            
            if(validator == null) continue;

            var validationContext = new ValidationContext<object>(argument);
            var validationResult = await validator.ValidateAsync(validationContext);

            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .GroupBy(x => x.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

                var response = new
                {
                    status = 422,
                    message = "Validation Failed",
                    errors,
                    timestamp = DateTime.UtcNow
                };
                context.Result = new UnprocessableEntityObjectResult(response);
                return;
            }
        }

        await next();
    }
}