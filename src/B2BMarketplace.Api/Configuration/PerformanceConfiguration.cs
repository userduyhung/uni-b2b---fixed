namespace B2BMarketplace.Api.Configuration;

/// <summary>
/// Configuration class for performance optimizations
/// </summary>
public static class PerformanceConfiguration
{
    /// <summary>
    /// Configures performance-related services
    /// </summary>
    /// <param name="services">The service collection</param>
    public static void ConfigurePerformanceServices(IServiceCollection services)
    {
        // Add response caching for performance
        services.AddResponseCaching(options =>
        {
            options.MaximumBodySize = 1024 * 1024; // 1MB max body size
            options.UseCaseSensitivePaths = false;
            options.SizeLimit = 100 * 1024 * 1024; // 100MB total cache size limit
        });

        // Add memory cache for performance
        services.AddMemoryCache(options =>
        {
            options.SizeLimit = 100 * 1024 * 1024; // 100MB cache size limit
        });

        // Add output caching for performance
        services.AddOutputCache(options =>
        {
            // Set default size limit
            options.DefaultExpirationTimeSpan = TimeSpan.FromSeconds(30);
            options.MaximumBodySize = 64 * 1024 * 1024; // 64MB max body size
            options.SizeLimit = 100 * 1024 * 1024; // 100MB total cache size
        });
    }

    /// <summary>
    /// Configures performance-related middleware
    /// </summary>
    /// <param name="app">The web application</param>
    public static void ConfigurePerformanceMiddleware(WebApplication app)
    {
        // Use response caching for performance
        app.UseResponseCaching();

        // Use output caching for performance
        app.UseOutputCache();
    }
}