using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace AISupportTicketSystem.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();
        
        services.AddValidatorsFromAssembly(assembly);

        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssemblies(assembly);
        });

        services.AddAutoMapper(assembly);
        
        return services;
    }
}