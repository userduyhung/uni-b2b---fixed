using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace B2BMarketplace.Api.Configuration
{
    /// <summary>
    /// Operation filter to add role information and functional descriptions to API endpoints in Swagger UI
    /// </summary>
    public class RoleOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            // Get HTTP method and route for reuse
            var httpMethod = context.ApiDescription.HttpMethod?.ToUpper();
            var routeTemplate = context.ApiDescription.RelativePath?.ToLower() ?? "";

            // Add functional description based on HTTP method and route
            AddFunctionalDescription(operation, context);

            // Get all Authorize attributes from method and controller
            var authAttributes = context.MethodInfo.DeclaringType?.GetCustomAttributes(true)
                .Union(context.MethodInfo.GetCustomAttributes(true))
                .OfType<AuthorizeAttribute>()
                .ToList();

            if (authAttributes == null || !authAttributes.Any())
            {
                return;
            }

            // Extract roles from attributes
            var roles = authAttributes
                .Where(a => !string.IsNullOrEmpty(a.Roles))
                .SelectMany(a => a.Roles?.Split(',').Select(r => r.Trim()) ?? Enumerable.Empty<string>())
                .Distinct()
                .ToList();

            if (roles.Any())
            {
                // Generate short functional summary for the API
                var shortFunction = GetShortFunctionalSummary(httpMethod, routeTemplate);
                
                // Add role information and short function to the summary
                var roleText = $"🔒 Roles: {string.Join(", ", roles)}";
                var summaryText = !string.IsNullOrEmpty(shortFunction) 
                    ? $"{shortFunction} {roleText}" 
                    : roleText;
                
                if (!string.IsNullOrEmpty(operation.Summary))
                {
                    operation.Summary = $"{operation.Summary} {summaryText}";
                }
                else
                {
                    operation.Summary = summaryText;
                }

                // Also add to description for more visibility
                var roleDescription = $"\n\n**🔐 Required Roles:** {string.Join(", ", roles)}";
                operation.Description = (operation.Description ?? "") + roleDescription;
            }
            else if (authAttributes.Any())
            {
                // Has [Authorize] but no specific roles
                var authText = "🔒 [Auth Required]";
                if (!string.IsNullOrEmpty(operation.Summary))
                {
                    operation.Summary = $"{operation.Summary} {authText}";
                }
                else
                {
                    operation.Summary = authText;
                }
            }
        }

        private void AddFunctionalDescription(OpenApiOperation operation, OperationFilterContext context)
        {
            var httpMethod = context.ApiDescription.HttpMethod?.ToUpper();
            var routeTemplate = context.ApiDescription.RelativePath?.ToLower() ?? "";
            var actionName = context.MethodInfo.Name;

            // Generate functional description based on patterns
            var functionality = GenerateFunctionalityDescription(httpMethod, routeTemplate, actionName);

            if (!string.IsNullOrEmpty(functionality))
            {
                var functionalityNote = $"\n\n**📋 Functionality:** {functionality}";
                operation.Description = (operation.Description ?? "") + functionalityNote;
            }

            // Add common features based on HTTP method
            var features = GetCommonFeatures(httpMethod, routeTemplate);
            if (features.Any())
            {
                var featuresText = $"\n\n**✨ Features:**\n{string.Join("\n", features.Select(f => $"- {f}"))}";
                operation.Description = (operation.Description ?? "") + featuresText;
            }
        }

        private string GenerateFunctionalityDescription(string? httpMethod, string routeTemplate, string actionName)
        {
            // Cart operations
            if (routeTemplate.Contains("cart"))
            {
                return httpMethod switch
                {
                    "GET" => "Retrieve current shopping cart items and calculate total amount",
                    "POST" when routeTemplate.Contains("add") => "Add product to shopping cart with specified quantity",
                    "PUT" => "Update quantity or details of items in cart",
                    "DELETE" => "Remove specific item or clear entire shopping cart",
                    _ => "Manage shopping cart operations"
                };
            }

            // Order operations
            if (routeTemplate.Contains("order"))
            {
                return httpMethod switch
                {
                    "GET" when routeTemplate.Contains("{id}") => "Get detailed information of a specific order including items and status",
                    "GET" => "List all orders with filtering, pagination and status tracking",
                    "POST" => "Create new order from cart items and process checkout",
                    "PUT" when routeTemplate.Contains("status") => "Update order status (processing, shipped, delivered, cancelled)",
                    "PUT" when routeTemplate.Contains("cancel") => "Cancel order and process refund if applicable",
                    _ => "Manage order lifecycle and tracking"
                };
            }

            // RFQ (Request for Quote) operations
            if (routeTemplate.Contains("rfq"))
            {
                return httpMethod switch
                {
                    "GET" => "List all RFQs with search, filter by status/category",
                    "POST" => "Create new Request for Quote with product details and requirements",
                    "PUT" when routeTemplate.Contains("respond") => "Seller responds to RFQ with pricing and terms",
                    "PUT" when routeTemplate.Contains("accept") => "Buyer accepts quote and converts to order",
                    "DELETE" => "Cancel or withdraw RFQ request",
                    _ => "Handle quote request and negotiation workflow"
                };
            }

            // Quote operations
            if (routeTemplate.Contains("quote"))
            {
                return httpMethod switch
                {
                    "GET" => "View received quotes with pricing, terms and validity period",
                    "POST" => "Submit quote as seller in response to RFQ",
                    "PUT" when routeTemplate.Contains("accept") => "Accept quote and proceed to order creation",
                    "PUT" when routeTemplate.Contains("reject") => "Reject quote with optional reason",
                    "DELETE" => "Withdraw or cancel submitted quote",
                    _ => "Manage quote submission and acceptance process"
                };
            }

            // Product operations
            if (routeTemplate.Contains("product"))
            {
                return httpMethod switch
                {
                    "GET" when routeTemplate.Contains("{id}") => "Get detailed product info including specs, pricing, stock and reviews",
                    "GET" when routeTemplate.Contains("search") => "Search products by keyword, category, price range with filters",
                    "GET" => "Browse product catalog with pagination and sorting options",
                    "POST" => "Create new product listing with details, images and pricing",
                    "PUT" => "Update product information, stock level or pricing",
                    "DELETE" => "Remove product listing from marketplace",
                    _ => "Manage product catalog and inventory"
                };
            }

            // Payment operations
            if (routeTemplate.Contains("payment"))
            {
                return httpMethod switch
                {
                    "GET" => "View payment history, status and transaction details",
                    "POST" when routeTemplate.Contains("process") => "Process payment using selected payment method",
                    "POST" when routeTemplate.Contains("refund") => "Initiate refund for cancelled or returned order",
                    "PUT" when routeTemplate.Contains("verify") => "Verify payment status and update order accordingly",
                    _ => "Handle payment processing and transaction management"
                };
            }

            // Premium/Subscription operations
            if (routeTemplate.Contains("premium") || routeTemplate.Contains("subscription"))
            {
                return httpMethod switch
                {
                    "GET" => "View current subscription plan, benefits and billing history",
                    "POST" when routeTemplate.Contains("subscribe") => "Subscribe to premium plan with selected tier and payment",
                    "POST" when routeTemplate.Contains("cancel") => "Cancel subscription with refund policy applied",
                    "PUT" when routeTemplate.Contains("upgrade") => "Upgrade to higher tier subscription plan",
                    "PUT" when routeTemplate.Contains("renewal") => "Enable/disable auto-renewal for subscription",
                    _ => "Manage premium subscription and membership benefits"
                };
            }

            // Review operations
            if (routeTemplate.Contains("review"))
            {
                return httpMethod switch
                {
                    "GET" => "Browse product or seller reviews with ratings and filters",
                    "POST" => "Submit review with rating, comment and optional images",
                    "PUT" => "Edit or update existing review content",
                    "DELETE" => "Delete review (only by author or admin)",
                    _ => "Manage product and seller reviews and ratings"
                };
            }

            // Favorite operations
            if (routeTemplate.Contains("favorite"))
            {
                return httpMethod switch
                {
                    "GET" => "List all favorited products for quick access",
                    "POST" => "Add product to favorites list",
                    "DELETE" => "Remove product from favorites",
                    _ => "Manage favorite products wishlist"
                };
            }

            // Profile operations
            if (routeTemplate.Contains("profile"))
            {
                return httpMethod switch
                {
                    "GET" => "View user profile information and settings",
                    "PUT" => "Update profile details, contact info or preferences",
                    "POST" when routeTemplate.Contains("verify") => "Submit verification documents for seller account",
                    _ => "Manage user profile and account settings"
                };
            }

            // Notification operations
            if (routeTemplate.Contains("notification"))
            {
                return httpMethod switch
                {
                    "GET" => "Retrieve notifications with unread count and filters",
                    "PUT" when routeTemplate.Contains("read") => "Mark notifications as read",
                    "PUT" when routeTemplate.Contains("preference") => "Update notification preferences and channels",
                    "DELETE" => "Clear or delete specific notifications",
                    _ => "Manage notifications and alert preferences"
                };
            }

            // Search operations
            if (routeTemplate.Contains("search"))
            {
                return httpMethod switch
                {
                    "GET" => "Search across products, sellers with advanced filters and sorting",
                    "POST" when routeTemplate.Contains("suggest") => "Get search suggestions and autocomplete results",
                    _ => "Perform search operations with various criteria"
                };
            }

            // Admin operations
            if (routeTemplate.Contains("admin"))
            {
                return httpMethod switch
                {
                    "GET" when routeTemplate.Contains("user") => "View and manage user accounts, roles and permissions",
                    "GET" when routeTemplate.Contains("report") => "Generate business reports, analytics and statistics",
                    "PUT" when routeTemplate.Contains("approve") => "Approve pending items (products, sellers, content)",
                    "PUT" when routeTemplate.Contains("reject") => "Reject pending items with reason",
                    "DELETE" when routeTemplate.Contains("user") => "Suspend or delete user accounts",
                    _ => "Perform administrative tasks and platform management"
                };
            }

            // Default based on HTTP method only
            return httpMethod switch
            {
                "GET" => "Retrieve data or resources",
                "POST" => "Create new resource or submit data",
                "PUT" => "Update existing resource",
                "DELETE" => "Remove resource",
                "PATCH" => "Partially update resource",
                _ => ""
            };
        }

        private List<string> GetCommonFeatures(string? httpMethod, string routeTemplate)
        {
            var features = new List<string>();

            // Common features based on route patterns
            if (routeTemplate.Contains("search") || routeTemplate.Contains("filter"))
            {
                features.Add("🔍 Advanced search and filtering");
            }

            if (routeTemplate.Contains("paging") || routeTemplate.Contains("page") || httpMethod == "GET")
            {
                features.Add("📄 Pagination support for large datasets");
            }

            if (routeTemplate.Contains("export"))
            {
                features.Add("📥 Export data to various formats");
            }

            if (routeTemplate.Contains("bulk"))
            {
                features.Add("⚡ Bulk operations support");
            }

            if (routeTemplate.Contains("notify") || routeTemplate.Contains("notification"))
            {
                features.Add("🔔 Real-time notifications");
            }

            if (routeTemplate.Contains("validate") || routeTemplate.Contains("verify"))
            {
                features.Add("✅ Input validation and verification");
            }

            if (routeTemplate.Contains("payment") || routeTemplate.Contains("order"))
            {
                features.Add("🔐 Secure transaction processing");
            }

            if (routeTemplate.Contains("audit") || routeTemplate.Contains("log"))
            {
                features.Add("📊 Activity logging and audit trail");
            }

            return features;
        }

        private string GetShortFunctionalSummary(string? httpMethod, string routeTemplate)
        {
            // Generate short, concise function summaries similar to the image
            
            // Cart operations
            if (routeTemplate.Contains("cart"))
            {
                return httpMethod switch
                {
                    "GET" => "View cart",
                    "POST" when routeTemplate.Contains("add") => "Add to cart",
                    "PUT" => "Update cart items",
                    "DELETE" => "Remove from cart",
                    _ => "Cart management"
                };
            }

            // Order operations
            if (routeTemplate.Contains("order"))
            {
                return httpMethod switch
                {
                    "GET" when routeTemplate.Contains("{id}") => "Get order details",
                    "GET" => "List orders",
                    "POST" => "Create order",
                    "PUT" when routeTemplate.Contains("status") => "Update order status",
                    "PUT" when routeTemplate.Contains("cancel") => "Cancel order",
                    _ => "Order management"
                };
            }

            // RFQ operations
            if (routeTemplate.Contains("rfq"))
            {
                return httpMethod switch
                {
                    "GET" => "View RFQs",
                    "POST" => "Create RFQ",
                    "PUT" when routeTemplate.Contains("respond") => "Respond to RFQ",
                    "PUT" when routeTemplate.Contains("accept") => "Accept quote",
                    "DELETE" => "Cancel RFQ",
                    _ => "RFQ management"
                };
            }

            // Quote operations
            if (routeTemplate.Contains("quote"))
            {
                return httpMethod switch
                {
                    "GET" => "View quotes",
                    "POST" => "Submit quote",
                    "PUT" when routeTemplate.Contains("accept") => "Accept quote",
                    "PUT" when routeTemplate.Contains("reject") => "Reject quote",
                    "DELETE" => "Withdraw quote",
                    _ => "Quote management"
                };
            }

            // Product operations
            if (routeTemplate.Contains("product"))
            {
                return httpMethod switch
                {
                    "GET" when routeTemplate.Contains("{id}") => "Get product details",
                    "GET" when routeTemplate.Contains("search") => "Search products",
                    "GET" => "List products",
                    "POST" => "Create product",
                    "PUT" => "Update product",
                    "DELETE" => "Delete product",
                    _ => "Product management"
                };
            }

            // Payment operations
            if (routeTemplate.Contains("payment"))
            {
                return httpMethod switch
                {
                    "GET" => "View payment history",
                    "POST" when routeTemplate.Contains("process") => "Process payment",
                    "POST" when routeTemplate.Contains("refund") => "Process refund",
                    "PUT" when routeTemplate.Contains("verify") => "Verify payment",
                    _ => "Payment processing"
                };
            }

            // Premium/Subscription operations
            if (routeTemplate.Contains("premium") || routeTemplate.Contains("subscription"))
            {
                if (routeTemplate.Contains("cancel-with-refund"))
                    return "Cancel subscription with refund (24h=100%, 1-3days=50%, >3days=0%)";
                
                if (routeTemplate.Contains("cancel-auto-renewal"))
                    return "Cancel auto-renewal (no refund, subscription continues until end date)";
                
                if (routeTemplate.Contains("enable-auto-renewal"))
                    return "Enable auto-renewal for subscription";
                
                if (routeTemplate.Contains("refund-eligibility"))
                    return "Get refund eligibility details (percentage and amount)";
                
                if (routeTemplate.Contains("purchase-with-wallet"))
                    return "Purchase premium subscription with wallet cash";

                return httpMethod switch
                {
                    "GET" => "View subscription",
                    "POST" when routeTemplate.Contains("subscribe") => "Create premium subscription",
                    "POST" when routeTemplate.Contains("cancel") => "Cancel subscription",
                    "PUT" when routeTemplate.Contains("cancel") => "Cancel subscription",
                    "PUT" when routeTemplate.Contains("upgrade") => "Upgrade subscription",
                    "PUT" when routeTemplate.Contains("renewal") => "Manage auto-renewal",
                    _ => "Subscription management"
                };
            }

            // Review operations
            if (routeTemplate.Contains("review"))
            {
                return httpMethod switch
                {
                    "GET" => "View reviews",
                    "POST" => "Submit review",
                    "PUT" => "Update review",
                    "DELETE" => "Delete review",
                    _ => "Review management"
                };
            }

            // Favorite operations
            if (routeTemplate.Contains("favorite"))
            {
                return httpMethod switch
                {
                    "GET" => "View favorites",
                    "POST" => "Add to favorites",
                    "DELETE" => "Remove from favorites",
                    _ => "Favorites management"
                };
            }

            // Profile operations
            if (routeTemplate.Contains("profile"))
            {
                return httpMethod switch
                {
                    "GET" => "View profile",
                    "PUT" => "Update profile",
                    "POST" when routeTemplate.Contains("verify") => "Submit verification",
                    _ => "Profile management"
                };
            }

            // Notification operations
            if (routeTemplate.Contains("notification"))
            {
                return httpMethod switch
                {
                    "GET" => "Get notifications",
                    "PUT" when routeTemplate.Contains("read") => "Mark as read",
                    "PUT" when routeTemplate.Contains("preference") => "Update preferences",
                    "DELETE" => "Clear notifications",
                    _ => "Notification management"
                };
            }

            // Search operations
            if (routeTemplate.Contains("search"))
            {
                return httpMethod switch
                {
                    "GET" => "Search",
                    "POST" when routeTemplate.Contains("suggest") => "Get suggestions",
                    _ => "Search operations"
                };
            }

            // Admin operations
            if (routeTemplate.Contains("admin"))
            {
                return httpMethod switch
                {
                    "GET" when routeTemplate.Contains("user") => "Manage users",
                    "GET" when routeTemplate.Contains("report") => "View reports",
                    "PUT" when routeTemplate.Contains("approve") => "Approve item",
                    "PUT" when routeTemplate.Contains("reject") => "Reject item",
                    "DELETE" when routeTemplate.Contains("user") => "Delete user",
                    _ => "Admin operations"
                };
            }

            // Default - return empty to use existing summary
            return "";
        }
    }
}
