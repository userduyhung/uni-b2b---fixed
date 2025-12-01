using B2BPlatform.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace B2BPlatform.Data.Configurations
{
    public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
    {
        public void Configure(EntityTypeBuilder<AuditLog> builder)
        {
            // Primary key
            builder.HasKey(e => e.Id);
            
            // Indexes for performance
            builder.HasIndex(e => e.Timestamp);
            builder.HasIndex(e => e.UserId);
            builder.HasIndex(e => e.EntityName);
            builder.HasIndex(e => e.EntityId);
            builder.HasIndex(e => e.Action);
            builder.HasIndex(e => e.OperationType);
            
            // Constraints to ensure immutability
            builder.Property(e => e.Id).ValueGeneratedOnAdd();
            builder.Property(e => e.CreatedAt).ValueGeneratedOnAdd();
            
            // Prevent updates to these fields after creation
            builder.Property(e => e.CreatedAt).ValueGeneratedOnAdd();
            
            // Make table read-only friendly by design
            builder.ToTable("AuditLogs", table => 
            {
                table.HasCheckConstraint("CK_AuditLog_Immutable", "[CreatedAt] = [CreatedAtCheck]");
            });
        }
    }
}