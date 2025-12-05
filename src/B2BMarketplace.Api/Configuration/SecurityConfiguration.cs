using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Builder;
using System.Text.Json;

namespace B2BMarketplace.Api.Configuration;

/// <summary>
/// Configuration class for security enhancements
/// </summary>
public static class SecurityConfiguration
{
    /// <summary>

    /// Configures security-related middleware

    /// </summary>

    /// <param name="app">The web application</param>

    public static void ConfigureSecurityMiddleware(WebApplication app)

    {

        var enableHsts = app.Configuration.GetValue<bool>("Security:EnableHsts", true);

        var hstsMaxAge = app.Configuration.GetValue<int>("Security:HstsMaxAge", 365);

        

        // Add security headers using middleware

        app.Use(async (context, next) =>

        {

            context.Response.Headers.Append("X-Content-Type-Options", "nosniff");

            context.Response.Headers.Append("X-Frame-Options", "DENY");

            context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");

            context.Response.Headers.Append("Referrer-Policy", "no-referrer");

            

            // Add HSTS header if enabled in configuration

            if (enableHsts)

            {

                var hstsValue = $"max-age={hstsMaxAge * 24 * 60 * 60}; includeSubDomains";

                context.Response.Headers.Append("Strict-Transport-Security", hstsValue);

            }

            

            await next();

        });



        // Use HSTS in production when HTTPS is available

        if (enableHsts)

        {

            app.UseHsts();

        }

    }

    /// <summary>
    /// Configures security-related services
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">The configuration</param>
    public static void ConfigureSecurityServices(IServiceCollection services, IConfiguration configuration)
    {
        // Configure CORS for security
        services.AddCors(options =>
        {
            options.AddPolicy("DevelopmentCors", policy =>
            {
                policy.SetIsOriginAllowed(origin => true) // Allow any origin
                      .AllowAnyMethod()
                      .AllowAnyHeader()
                      .AllowCredentials();
            });

            options.AddPolicy("ProductionCors", policy =>
            {
                policy.WithOrigins(
                        "https://fsourcing.vercel.app",  // ✅ Frontend Vercel
                        "https://localhost:5001", 
                        "https://yourdomain.com", 
                        "http://localhost:5000",
                        "https://*.github.dev") // Configure your allowed origins
                      .SetIsOriginAllowedToAllowWildcardSubdomains()
                      .WithMethods("GET", "POST", "PUT", "DELETE")
                      .WithHeaders("Content-Type", "Authorization")
                      .AllowCredentials();
            });
        });

        // Configure anti-forgery for security
        services.AddAntiforgery(options =>
        {
            options.HeaderName = "X-CSRF-TOKEN";
            options.Cookie.Name = "XSRF-COOKIE";
            options.Cookie.HttpOnly = true;
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            options.Cookie.SameSite = SameSiteMode.Strict;
        });
    }

    /// <summary>
    /// Configures security-related exception handling
    /// </summary>
    /// <param name="app">The web application</param>
    public static void ConfigureSecurityExceptionHandling(WebApplication app)
    {
        // Use exception handling middleware with security considerations
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler(errorApp =>
            {
                errorApp.Run(async context =>
                {
                    context.Response.StatusCode = 500;
                    context.Response.ContentType = "application/json";

                    // Don't expose detailed error information in production
                    var errorResponse = new
                    {
                        error = "An internal error occurred",
                        timestamp = DateTime.UtcNow,
                        requestId = context.TraceIdentifier
                    };

                    await context.Response.WriteAsJsonAsync(errorResponse);
                });
            });
        }
    }
}