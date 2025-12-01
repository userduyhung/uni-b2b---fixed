using Microsoft.OpenApi.Models;

using Microsoft.EntityFrameworkCore;

using Microsoft.EntityFrameworkCore.InMemory;

using Microsoft.AspNetCore.Authentication.JwtBearer;

using Microsoft.IdentityModel.Tokens;

using System.Text;
using B2BMarketplace.Api.Configuration;
using B2BMarketplace.Api.Services; // For background services
// Remove ambiguous using statements and specify full namespaces where needed
using B2BMarketplace.Core.Interfaces.Repositories;
using B2BMarketplace.Core.Interfaces.Services;
using B2BMarketplace.Core.Services;
using B2BMarketplace.Infrastructure.Data;
using B2BMarketplace.Infrastructure.Data.Repositories;
using B2BMarketplace.Infrastructure.Repositories;
using B2BMarketplace.Infrastructure.Services;
using B2BMarketplace.Core.Services.Admin;
using B2BMarketplace.Core.Services.Premium;
using B2BMarketplace.Core.Interfaces.Services.Admin;
using B2BMarketplace.Core.Interfaces.Services.Premium;

var builder = WebApplication.CreateBuilder(args);

// Configure logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase; // Use camelCase for API responses
        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull; // Ignore null properties
        options.JsonSerializerOptions.WriteIndented = false; // Don't write indented JSON
        options.JsonSerializerOptions.IgnoreReadOnlyProperties = false; // Don't ignore read-only properties
        options.JsonSerializerOptions.IncludeFields = true; // Include fields in serialization
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles; // Handle circular references
    });

// Add Entity Framework: prefer SQL Server when a DefaultConnection is configured
var defaultConn = builder.Configuration.GetConnectionString("DefaultConnection");

if (!string.IsNullOrWhiteSpace(defaultConn))
{
    // Use SQL Server (e.g., local SQL Server 2022)
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(defaultConn));
}
else if (builder.Environment.IsDevelopment())
{
    // Development fallback: In-memory (keeps existing dev behavior)
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseInMemoryDatabase("B2BMarketplaceTestDb"));
}
else
{
    // Production fallback: SQLite file
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlite("Data Source=b2bmarketplace.db"));
}

// Register services
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IBuyerProfileRepository, BuyerProfileRepository>();
builder.Services.AddScoped<ISellerProfileRepository, SellerProfileRepository>();
builder.Services.AddScoped<ICertificationRepository, CertificationRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IRFQRepository, RFQRepository>();
builder.Services.AddScoped<IQuoteRepository, QuoteRepository>();
builder.Services.AddScoped<IRFQRecipientRepository, RFQRecipientRepository>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<INotificationPreferencesRepository, NotificationPreferencesRepository>();
builder.Services.AddScoped<IAuditRepository, AuditRepository>();
builder.Services.AddScoped<IContentModerationRepository, ContentModerationRepository>();
builder.Services.AddScoped<IModerationAuditLogRepository, ModerationAuditLogRepository>();
builder.Services.AddScoped<IPasswordService, PasswordService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAdminUserService, AdminUserService>();
builder.Services.AddScoped<IProfileService, ProfileService>();
builder.Services.AddScoped<ICertificationService, CertificationService>();
builder.Services.AddScoped<IVerificationService, VerificationService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IRFQService, RFQService>();
builder.Services.AddScoped<IQuoteService, QuoteService>();
builder.Services.AddScoped<IContentModerationService, ContentModerationService>();
builder.Services.AddScoped<IAnalyticsService, AnalyticsService>();
builder.Services.AddScoped<IPasswordResetService, PasswordResetService>();
builder.Services.AddScoped<IPasswordResetTokenRepository, PasswordResetTokenRepository>();
builder.Services.AddScoped<B2BMarketplace.Core.Interfaces.Services.IEmailService, B2BMarketplace.Infrastructure.Services.EmailService>();
builder.Services.AddScoped<IFavoriteRepository, FavoriteRepository>();
builder.Services.AddScoped<IFavoriteService, FavoriteService>();
builder.Services.AddScoped<ISearchRepository, SearchRepository>();
builder.Services.AddScoped<ISearchService, SearchService>();
builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<IPremiumSubscriptionRepository, PremiumSubscriptionRepository>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IPremiumSubscriptionService, PremiumSubscriptionService>();
builder.Services.AddScoped<IAuditLogService, AuditLogService>();
builder.Services.AddScoped<IStaticContentService, StaticContentService>();
builder.Services.AddScoped<IContractTemplateService, ContractTemplateService>();
builder.Services.AddScoped<IServiceTierRepository, B2BMarketplace.Infrastructure.Repositories.ServiceTierRepository>();
builder.Services.AddScoped<IServiceTierService, ServiceTierService>();

// Register content management services
builder.Services.AddScoped<IContentCategoryRepository, ContentCategoryRepository>();
builder.Services.AddScoped<IContentItemRepository, ContentItemRepository>();
builder.Services.AddScoped<IContentTagRepository, ContentTagRepository>();
builder.Services.AddScoped<IContentCategoryService, ContentCategoryService>();
builder.Services.AddScoped<IContentItemService, ContentItemService>();
builder.Services.AddScoped<IContentTagService, ContentTagService>();

// Register background services
builder.Services.AddHostedService<ScheduledContentProcessingService>();
builder.Services.AddHostedService<PaymentConfirmationBackgroundService>();

// Register new repositories for missing features
builder.Services.AddScoped<IContractTemplateRepository, ContractTemplateRepository>();
builder.Services.AddScoped<IContractInstanceRepository, ContractInstanceRepository>();
builder.Services.AddScoped<IReviewReplyRepository, ReviewReplyRepository>();
builder.Services.AddScoped<IPaymentInvoiceRepository, PaymentInvoiceRepository>();
builder.Services.AddScoped<IStaticContentRepository, StaticContentRepository>();

// Register repositories for admin category management and premium subscriptions
builder.Services.AddScoped<IProductCategoryRepository, ProductCategoryRepository>();
builder.Services.AddScoped<ICategoryConfigurationRepository, CategoryConfigurationRepository>();

// Register services for admin category management and premium subscriptions
builder.Services.AddScoped<IProductCategoryService, ProductCategoryService>();
builder.Services.AddScoped<ICategoryConfigurationService, CategoryConfigurationService>();
builder.Services.AddScoped<ICategorySellerProfileService, CategorySellerProfileService>();
builder.Services.AddScoped<IPaymentConfirmationService, B2BMarketplace.Core.Services.PaymentConfirmationService>();

// Register services for admin category and certification management
builder.Services.AddScoped<IAdminCategoryService, AdminCategoryService>();
builder.Services.AddScoped<IAdminCertificationService, AdminCertificationService>();

// Register services for premium status assignment and management
builder.Services.AddScoped<IPremiumAssignmentService, PremiumAssignmentService>();
builder.Services.AddScoped<IPremiumManagementService, PremiumManagementService>();

// Register email service
builder.Services.AddScoped<B2BMarketplace.Core.Interfaces.Services.IEmailService, B2BMarketplace.Infrastructure.Services.EmailService>();

// Register cart-related services
builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<ICartService, B2BMarketplace.Infrastructure.Services.CartService>();

// Register address-related services
builder.Services.AddScoped<IAddressRepository, AddressRepository>();
builder.Services.AddScoped<IAddressService, AddressService>();

// Register payment method-related services
builder.Services.AddScoped<IPaymentMethodRepository, PaymentMethodRepository>();
builder.Services.AddScoped<IPaymentMethodService, PaymentMethodService>();

// Register order-related services
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderService, B2BMarketplace.Infrastructure.Services.OrderService>();

// Register claims utility service
builder.Services.AddScoped<IClaimsUtilityService, ClaimsUtilityService>();

// Configure performance services
PerformanceConfiguration.ConfigurePerformanceServices(builder.Services);

// Configure security services
SecurityConfiguration.ConfigureSecurityServices(builder.Services, builder.Configuration);

// Configure JWT authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    // In a real application, these would be loaded from configuration
    var secretKey = "your-super-secret-key-that-is-at-least-32-characters-long";
    var issuer = "B2BMarketplace";
    var audience = "B2BMarketplaceUsers";

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey)),
        ValidateIssuer = true,
        ValidIssuer = issuer,
        ValidateAudience = true,
        ValidAudience = audience,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero,
        // Map JWT claim types to ASP.NET Core identity claim types
        RoleClaimType = System.Security.Claims.ClaimTypes.Role,
        NameClaimType = System.Security.Claims.ClaimTypes.NameIdentifier
    };

    // Add events for debugging
    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
            logger.LogError($"❌ JWT Authentication Failed: {context.Exception.Message}");
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
            logger.LogInformation("✅ JWT Token Validated Successfully");
            var claims = context.Principal?.Claims;
            if (claims != null)
            {
                foreach (var claim in claims)
                {
                    logger.LogInformation($"  Claim: {claim.Type} = {claim.Value}");
                }
            }
            return Task.CompletedTask;
        },
        OnMessageReceived = context =>
        {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
            var token = context.Request.Headers["Authorization"].FirstOrDefault();
            logger.LogInformation($"📩 JWT Token Received: {(string.IsNullOrEmpty(token) ? "NONE" : "EXISTS - " + token.Substring(0, Math.Min(50, token.Length)))}");
            return Task.CompletedTask;
        },
        OnChallenge = context =>
        {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
            logger.LogWarning($"⚠️ JWT Challenge: {context.Error}, {context.ErrorDescription}");
            return Task.CompletedTask;
        }
    };
});

// Configure authorization
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("SellerOnly", policy => policy.RequireRole("Seller"));
    options.AddPolicy("BuyerOnly", policy => policy.RequireRole("Buyer"));
    options.AddPolicy("AdminOrSeller", policy => policy.RequireRole("Admin", "Seller"));
    options.AddPolicy("AdminOrBuyer", policy => policy.RequireRole("Admin", "Buyer"));
});

// Configure Swagger with proper settings
builder.Services.AddEndpointsApiExplorer();

// Get Swagger configuration from appsettings
var swaggerConfig = builder.Configuration.GetSection("Swagger");
var title = swaggerConfig["Title"] ?? "B2B Marketplace API";
var version = swaggerConfig["Version"] ?? "v1";
var description = swaggerConfig["Description"] ?? "API for B2B Marketplace Platform";

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = title,
        Version = version,
        Description = description
    });

    // Add security schemes
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    // Add security requirement - this makes Swagger send the token with every request
    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });

    // Add server URLs - use relative path to work with tunnels
    // This allows Swagger to work with both localhost and forwarded ports
    // Swagger will automatically use the current URL (tunnel or localhost)

    // Handle circular references in Swagger schema generation
    c.CustomSchemaIds(type => type.FullName?.Replace("+", ".") ?? type.Name);

    // Enable annotations
    c.EnableAnnotations();

    // Add role information to API endpoints
    c.OperationFilter<RoleOperationFilter>();

    // Resolve conflicting actions by using the first one encountered
    c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
});

var app = builder.Build();

// Seed data first
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await B2BMarketplace.Infrastructure.Data.DataSeeder.SeedAsync(context);
}

// Configure security exception handling
SecurityConfiguration.ConfigureSecurityExceptionHandling(app);

// CRITICAL: Use CORS before any other middleware that might generate responses
if (app.Environment.IsDevelopment())
{
    app.UseCors("DevelopmentCors");
}
else
{
    app.UseCors("ProductionCors");
}

// Configure the HTTP request pipeline.
// Always enable Swagger for testing
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "B2B Marketplace API v1");
    c.RoutePrefix = string.Empty;
    // Configure Swagger UI to work with both HTTP and HTTPS
    c.ConfigObject.DeepLinking = false;
    c.DocumentTitle = "B2B Marketplace API";
    c.ConfigObject.DisplayOperationId = true;
    c.ConfigObject.DisplayRequestDuration = true;
    c.ConfigObject.TryItOutEnabled = true;
});

// Skip HTTPS redirection in Development to avoid issues with port forwarding
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

// Configure security middleware (but skip HSTS in development)
if (!app.Environment.IsDevelopment())
{
    SecurityConfiguration.ConfigureSecurityMiddleware(app);
}

// Configure performance middleware
PerformanceConfiguration.ConfigurePerformanceMiddleware(app);

// Use routing
app.UseRouting();

// Use authentication
app.UseAuthentication();

// Use authorization
app.UseAuthorization();

// Map controllers
app.MapControllers();

// --- THÊM ĐOẠN NÀY ĐỂ TỰ ĐỘNG CHUYỂN HƯỚNG ---
// app.MapGet("/", async context =>
// {
//     context.Response.Redirect("/swagger");
//     await Task.CompletedTask;
// });
if (app.Environment.IsDevelopment())
{
    var url = "http://localhost:5000";
    try
    {
        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
        {
            FileName = url,
            UseShellExecute = true
        });
    }
    catch { }
}
// ---------------------------------------------

app.Run();

// Make the Program class public for testing
public partial class Program { }
