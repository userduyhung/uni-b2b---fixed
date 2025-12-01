using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace B2BMarketplace.Api.Controllers
{
    /// <summary>
    /// Controller for user management operations
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        /// <summary>
        /// Get all users
        /// </summary>
        /// <returns>List of users</returns>
        [HttpGet]
        public IActionResult GetUsers()
        {
            // This is a placeholder implementation for demonstration
            // In a real implementation, this would query the database
            var users = new[]
            {
                new { Id = Guid.NewGuid(), Email = "testuser1@example.com", Role = "Buyer", CreatedAt = DateTime.UtcNow },
                new { Id = Guid.NewGuid(), Email = "testuser2@example.com", Role = "Seller", CreatedAt = DateTime.UtcNow },
                new { Id = Guid.NewGuid(), Email = "testuser3@example.com", Role = "Admin", CreatedAt = DateTime.UtcNow }
            };

            return Ok(new
            {
                data = new
                {
                    items = users,
                    totalCount = users.Length,
                    page = 1,
                    pageSize = 10,
                    totalPages = 1
                },
                message = "Users retrieved successfully",
                timestamp = DateTime.UtcNow
            });
        }

        /// <summary>
        /// Get user by ID
        /// </summary>
        /// <param name="id">User ID</param>
        /// <returns>User details</returns>
        [HttpGet("{id:guid}")]
        public IActionResult GetUser(Guid id)
        {
            // This is a placeholder implementation for demonstration
            // In a real implementation, this would query the database
            var user = new
            {
                Id = id,
                Email = $"user{id}@example.com",
                Role = "Buyer",
                CreatedAt = DateTime.UtcNow
            };

            return Ok(new
            {
                message = "User retrieved successfully",
                data = user,
                timestamp = DateTime.UtcNow
            });
        }

        /// <summary>
        /// Create a new user
        /// </summary>
        /// <param name="request">User creation request</param>
        /// <returns>Created user details</returns>
        [HttpPost]
        public IActionResult CreateUser([FromBody] CreateUserRequest request)
        {
            // This is a placeholder implementation for demonstration
            var newUser = new
            {
                Id = Guid.NewGuid(),
                Email = request.Email,
                Role = request.Role ?? "Buyer",
                CreatedAt = DateTime.UtcNow
            };

            return CreatedAtAction(nameof(GetUser), new { id = newUser.Id }, new
            {
                message = "User created successfully",
                data = newUser,
                timestamp = DateTime.UtcNow
            });
        }
    }

    /// <summary>
    /// Request model for user creation
    /// </summary>
    public class CreateUserRequest
    {
        /// <summary>
        /// User's email address
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// User's role (Buyer, Seller, Admin)
        /// </summary>
        public string? Role { get; set; }
    }
}
