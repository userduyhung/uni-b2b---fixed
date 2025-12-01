using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using B2BMarketplace.Infrastructure.Data;

namespace B2BMarketplace.Api.IntegrationTests;

public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove the existing DbContext registration
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType ==
                    typeof(DbContextOptions<ApplicationDbContext>));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            // Add a database context (ApplicationDbContext) using an in-memory database for testing
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseInMemoryDatabase("InMemoryDbForTesting");
            });

            // Build the service provider
            var sp = services.BuildServiceProvider();

            // Create a scope to obtain a reference to the database context
            using var scope = sp.CreateScope();
            var scopedServices = scope.ServiceProvider;
            var db = scopedServices.GetRequiredService<ApplicationDbContext>();

            // Ensure the database is created
            db.Database.EnsureCreated();

            // Force the creation of all DbSets by accessing them
            var auditLogsExist = db.AuditLogs.Any();
            Console.WriteLine($"AuditLogs table check: {auditLogsExist}");
            
            var auditLogsCount = db.AuditLogs.Count();
            Console.WriteLine($"AuditLogs count: {auditLogsCount}");

            // Also check other important tables
            var usersCount = db.Users.Count();
            Console.WriteLine($"Users count: {usersCount}");

            // Seed the database
            DataSeeder.SeedAsync(db).Wait();
        });
    }
}