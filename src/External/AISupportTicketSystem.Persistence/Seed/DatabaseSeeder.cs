using AISupportTicketSystem.Domain.Entities;
using AISupportTicketSystem.Persistence.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AISupportTicketSystem.Persistence.Seed;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<ApplicationDbContext>>();


        try
        {
            await context.Database.MigrateAsync();

            await SeedRolesAsync(roleManager, logger);
            await SeedCategoriesAsync(context, logger);
            await SeedUsersAsync(userManager, logger);
            
            logger.LogInformation("Database seeding completed successfully");

        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occured while seeding the database");
            throw;
        }
    }

    private static async Task SeedRolesAsync(RoleManager<IdentityRole<Guid>> roleManager, ILogger logger)
    {
        var roles = new[] { "Admin", "Supervisor", "Agent", "Customer" };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole<Guid>
                {
                    Id = Guid.NewGuid(),
                    Name = role,
                    NormalizedName = role.ToUpperInvariant()
                });
                logger.LogInformation("Created role: {Role}", role);
            }
        }
    }

    private static async Task SeedCategoriesAsync(ApplicationDbContext context, ILogger logger)
    {
        if(await context.Categories.AnyAsync()) return;

        var categories = new List<Category>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Technical Support",
                Description = "Technical issues and troubleshooting",
                Icon = "🔧",
                DisplayOrder = 1,
                IsActive =  true,
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                Id =  Guid.NewGuid(),
                Name = "Billing",
                Description = "Payment and invoice related issues",
                Icon = "💳",
                DisplayOrder = 2,
                IsActive =   true,
                CreatedAt =  DateTime.UtcNow
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Account",
                Description = "Account management and settings",
                Icon = "👤",
                DisplayOrder = 3,
                IsActive = true,
                CreatedAt =   DateTime.UtcNow
            },
            new()
            {
                Id =  Guid.NewGuid(),
                Name = "Feature Request",
                Description = "Suggestions and feature request",
                Icon = "💡",
                DisplayOrder = 4,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "General Inquiry",
                Description = "General questions and information",
                Icon = "❓",
                DisplayOrder = 5,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            }
        };

        await context.Categories.AddRangeAsync(categories);
        await context.SaveChangesAsync();
        logger.LogInformation("{Count} Categories seeded successfully", categories.Count);
    }

    private static async Task SeedUsersAsync(UserManager<ApplicationUser> userManager, ILogger logger)
    {
        await CreateUserIfNotExists(
            userManager,
            new ApplicationUser
            {
                UserName = "admin@ticketsystem.com",
                Email = "admin@ticketsystem.com",
                FirstName = "System",
                LastName = "Admin",
                EmailConfirmed = true,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            "Admin123!",
            "Admin",
            logger);

        // Supervisor user
        await CreateUserIfNotExists(
            userManager,
            new ApplicationUser
            {
                UserName = "supervisor@ticketsystem.com",
                Email = "supervisor@ticketsystem.com",
                FirstName = "John",
                LastName = "Supervisor",
                EmailConfirmed = true,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            "Supervisor123!",
            "Supervisor",
            logger);

        // Agent user
        await CreateUserIfNotExists(
            userManager,
            new ApplicationUser
            {
                UserName = "agent@ticketsystem.com",
                Email = "agent@ticketsystem.com",
                FirstName = "Jane",
                LastName = "Agent",
                EmailConfirmed = true,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            "Agent123!",
            "Agent",
            logger);

        // Customer user
        await CreateUserIfNotExists(
            userManager,
            new ApplicationUser
            {
                UserName = "customer@ticketsystem.com",
                Email = "customer@ticketsystem.com",
                FirstName = "Bob",
                LastName = "Customer",
                EmailConfirmed = true,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            "Customer123!",
            "Customer",
            logger);
    }
    
    private static async Task CreateUserIfNotExists(
        UserManager<ApplicationUser> userManager,
        ApplicationUser user,
        string password,
        string role,
        ILogger logger)
    {
        var existingUser = await userManager.FindByEmailAsync(user.Email!);
        
        if (existingUser == null)
        {
            var result = await userManager.CreateAsync(user, password);
            
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, role);
                logger.LogInformation("Created user: {Email} with role: {Role}", user.Email, role);
            }
            else
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                logger.LogWarning("Failed to create user {Email}: {Errors}", user.Email, errors);
            }
        }
    }
}