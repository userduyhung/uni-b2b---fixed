using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace B2BMarketplace.Core.Entities
{
    /// <summary>
    /// Represents an audit log entry for system actions
    /// </summary>
    public class AuditLog
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Action { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string EntityName { get; set; } = string.Empty;

        [Required]
        public int EntityId { get; set; }

        public Guid? UserId { get; set; }

        [MaxLength(100)]
        public string? UserName { get; set; }

        [MaxLength(100)]
        public string? UserRole { get; set; }

        public string? Details { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        [MaxLength(50)]
        public string? IPAddress { get; set; }

        [MaxLength(100)]
        public string? UserAgent { get; set; }

        [Required]
        [MaxLength(20)]
        public string OperationType { get; set; } = string.Empty; // CREATE, UPDATE, DELETE, READ

        // Properties to ensure immutability of audit logs
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [MaxLength(255)]
        public string? CreatedBy { get; set; }

        // Prevent updates to audit logs after creation
        [ConcurrencyCheck]
        public DateTime CreatedAtCheck { get; set; } = DateTime.UtcNow;
    }
}