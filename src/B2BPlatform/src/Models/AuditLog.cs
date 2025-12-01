using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace B2BPlatform.Models
{
    [Table("AuditLogs")]
    public class AuditLog
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Action { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string EntityName { get; set; }
        
        [Required]
        public int EntityId { get; set; }
        
        [MaxLength(50)]
        public string? UserId { get; set; }
        
        [MaxLength(50)]
        public string? UserName { get; set; }
        
        [MaxLength(100)]
        public string? UserRole { get; set; }
        
        public string? Details { get; set; }
        
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        
        [MaxLength(50)]
        public string? IPAddress { get; set; }
        
        [MaxLength(50)]
        public string? UserAgent { get; set; }
        
        [Required]
        [MaxLength(20)]
        public string OperationType { get; set; } // CREATE, UPDATE, DELETE, READ
        
        // Properties to ensure immutability of audit logs
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        [StringLength(255)]
        public string? CreatedBy { get; set; }
        
        // Prevent updates to audit logs after creation
        [ConcurrencyCheck]
        public DateTime CreatedAtCheck { get; set; } = DateTime.UtcNow;
    }
}