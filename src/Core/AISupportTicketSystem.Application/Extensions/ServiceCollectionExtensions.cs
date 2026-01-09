using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace AISupportTicketSystem.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        return services;
    }
}