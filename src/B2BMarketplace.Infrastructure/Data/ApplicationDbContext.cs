using B2BMarketplace.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace B2BMarketplace.Infrastructure.Data
{
    /// <summary>
    /// Entity Framework DbContext for the B2B Marketplace application
    /// </summary>
    public class ApplicationDbContext : DbContext
    {
        /// <summary>
        /// Users DbSet
        /// </summary>
        public DbSet<User> Users { get; set; }

        /// <summary>
        /// User management audit logs DbSet
        /// </summary>
        public DbSet<UserManagementAuditLog> UserManagementAuditLogs { get; set; }

        /// <summary>
        /// Buyer profiles DbSet
        /// </summary>
        public DbSet<BuyerProfile> BuyerProfiles { get; set; }

        /// <summary>
        /// Seller profiles DbSet
        /// </summary>
        public DbSet<SellerProfile> SellerProfiles { get; set; }

        /// <summary>
        /// Favorites DbSet
        /// </summary>
        public DbSet<Favorite> Favorites { get; set; }

        /// <summary>
        /// Certifications DbSet
        /// </summary>
        public DbSet<Certification> Certifications { get; set; }

        /// <summary>
        /// Products DbSet
        /// </summary>
        public DbSet<Product> Products { get; set; }

        /// <summary>
        /// RFQs DbSet
        /// </summary>
        public DbSet<RFQ> RFQs { get; set; }

        /// <summary>
        /// RFQ Items DbSet
        /// </summary>
        public DbSet<RFQItem> RFQItems { get; set; }

        /// <summary>
        /// RFQ Recipients DbSet
        /// </summary>
        public DbSet<RFQRecipient> RFQRecipients { get; set; }

        /// <summary>
        /// Quotes DbSet
        /// </summary>
        public DbSet<Quote> Quotes { get; set; }

        /// <summary>
        /// Notifications DbSet
        /// </summary>
        public DbSet<Notification> Notifications { get; set; }

        /// <summary>
        /// Notification Preferences DbSet
        /// </summary>
        public DbSet<NotificationPreferences> NotificationPreferences { get; set; }

        /// <summary>
        /// Content Reports DbSet
        /// </summary>
        public DbSet<ContentReport> ContentReports { get; set; }

        /// <summary>
        /// Moderation Audit Logs DbSet
        /// </summary>
        public DbSet<ModerationAuditLog> ModerationAuditLogs { get; set; }

        /// <summary>
        /// System audit logs DbSet
        /// </summary>
        public DbSet<AuditLog> AuditLogs { get; set; }

        /// <summary>
        /// Password Reset Tokens DbSet
        /// </summary>
        public DbSet<PasswordResetToken> PasswordResetTokens { get; set; }

        /// <summary>
        /// Reviews DbSet
        /// </summary>
        public DbSet<Review> Reviews { get; set; }

        /// <summary>
        /// Review Reports DbSet
        /// </summary>
        public DbSet<ReviewReport> ReviewReports { get; set; }

        /// <summary>
        /// Payments DbSet
        /// </summary>
        public DbSet<Payment> Payments { get; set; }

        /// <summary>
        /// Premium Subscriptions DbSet
        /// </summary>
        public DbSet<PremiumSubscription> PremiumSubscriptions { get; set; }

        /// <summary>
        /// Contract Templates DbSet
        /// </summary>
        public DbSet<ContractTemplate> ContractTemplates { get; set; }

        /// <summary>
        /// Contract Instances DbSet
        /// </summary>
        public DbSet<ContractInstance> ContractInstances { get; set; }

        /// <summary>
        /// Review Replies DbSet
        /// </summary>
        public DbSet<ReviewReply> ReviewReplies { get; set; }

        /// <summary>
        /// Payment Invoices DbSet
        /// </summary>
        public DbSet<PaymentInvoice> PaymentInvoices { get; set; }

        /// <summary>
        /// StaticContent DbSet
        /// </summary>
        public DbSet<StaticContent> StaticContents { get; set; }

        /// <summary>
        /// ContentCategory DbSet
        /// </summary>
        public DbSet<ContentCategory> ContentCategories { get; set; }

        /// <summary>
        /// ContentItem DbSet
        /// </summary>
        public DbSet<ContentItem> ContentItems { get; set; }

        /// <summary>
        /// ContentTag DbSet
        /// </summary>
        public DbSet<ContentTag> ContentTags { get; set; }

        /// <summary>
        /// ContentItemTag DbSet
        /// </summary>
        public DbSet<ContentItemTag> ContentItemTags { get; set; }

        /// <summary>
        /// Service Tiers DbSet
        /// </summary>
        public DbSet<ServiceTier> ServiceTiers { get; set; }

        /// <summary>
        /// Service Tier Features DbSet
        /// </summary>
        public DbSet<ServiceTierFeature> ServiceTierFeatures { get; set; }

        /// <summary>
        /// Service Tier Configurations DbSet
        /// </summary>
        public DbSet<ServiceTierConfiguration> ServiceTierConfigurations { get; set; }

        /// <summary>
        /// Product Categories DbSet
        /// </summary>
        public DbSet<ProductCategory> ProductCategories { get; set; }

        /// <summary>
        /// Category Configurations DbSet
        /// </summary>
        public DbSet<CategoryConfiguration> CategoryConfigurations { get; set; }

        /// <summary>
        /// Carts DbSet
        /// </summary>
        public DbSet<Cart> Carts { get; set; }

        /// <summary>
        /// Cart Items DbSet
        /// </summary>
        public DbSet<CartItem> CartItems { get; set; }

        /// <summary>
        /// Addresses DbSet
        /// </summary>
        public DbSet<Address> Addresses { get; set; }

        /// <summary>
        /// Payment Methods DbSet
        /// </summary>
        public DbSet<PaymentMethod> PaymentMethods { get; set; }

        /// <summary>
        /// Orders DbSet
        /// </summary>
        public DbSet<Order> Orders { get; set; }

        /// <summary>
        /// Order Items DbSet
        /// </summary>
        public DbSet<OrderItem> OrderItems { get; set; }

        /// <summary>
        /// Order Status History DbSet
        /// </summary>
        public DbSet<OrderStatusHistory> OrderStatusHistories { get; set; }

        /// <summary>
        /// Constructor for ApplicationDbContext
        /// </summary>
        /// <param name="options">DbContext options</param>
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        /// <summary>
        /// Configures the model
        /// </summary>
        /// <param name="modelBuilder">Model builder</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure User entity
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.HasIndex(u => u.Email).IsUnique();
                entity.Property(u => u.Email).IsRequired().HasMaxLength(255);
                entity.Property(u => u.PasswordHash).IsRequired();
                entity.Property(u => u.CreatedAt).IsRequired();
                entity.Property(u => u.IsActive).IsRequired();
                entity.Property(u => u.IsLocked).IsRequired();
            });

            // Configure UserManagementAuditLog entity
            modelBuilder.Entity<UserManagementAuditLog>(entity =>
            {
                entity.HasKey(ual => ual.Id);
                entity.Property(ual => ual.Action).IsRequired().HasMaxLength(100);
                entity.Property(ual => ual.Reason).HasMaxLength(500);
                entity.Property(ual => ual.IpAddress).HasMaxLength(50);
                entity.Property(ual => ual.UserAgent).HasMaxLength(200);
                entity.Property(ual => ual.EntityName).HasMaxLength(100);
                entity.Property(ual => ual.Details).HasMaxLength(1000);
                entity.Property(ual => ual.Timestamp).IsRequired();
                entity.Property(ual => ual.CreatedAt).IsRequired();
                entity.HasIndex(ual => ual.Timestamp);
                entity.HasIndex(ual => ual.AdminId);
                entity.HasIndex(ual => ual.UserId);
                entity.HasOne(ual => ual.User)
                    .WithMany()
                    .HasForeignKey(ual => ual.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Additional constraints for AC-03 (immutability)
                entity.Property(ual => ual.Id).ValueGeneratedOnAdd();
            });

            

            // Configure BuyerProfile entity
            modelBuilder.Entity<BuyerProfile>(entity =>
            {
                entity.HasKey(bp => bp.Id);
                entity.HasIndex(bp => bp.UserId).IsUnique();
                entity.Property(bp => bp.Name).IsRequired().HasMaxLength(255);
                entity.Property(bp => bp.CompanyName).HasMaxLength(255);
                entity.Property(bp => bp.Country).HasMaxLength(100);
                entity.Property(bp => bp.Phone).HasMaxLength(20);
                entity.Property(bp => bp.CreatedAt).IsRequired();
                entity.Property(bp => bp.UpdatedAt).IsRequired();
                entity.HasOne(bp => bp.User)
                    .WithOne(u => u.BuyerProfile)
                    .HasForeignKey<BuyerProfile>(bp => bp.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure SellerProfile entity
            modelBuilder.Entity<SellerProfile>(entity =>
            {
                entity.HasKey(sp => sp.Id);
                entity.HasIndex(sp => sp.UserId).IsUnique();
                entity.Property(sp => sp.CompanyName).IsRequired().HasMaxLength(255);
                entity.Property(sp => sp.LegalRepresentative).IsRequired().HasMaxLength(255);
                entity.Property(sp => sp.TaxId).IsRequired().HasMaxLength(50);
                entity.Property(sp => sp.Industry).HasMaxLength(100);
                entity.Property(sp => sp.Country).IsRequired().HasMaxLength(100);
                entity.Property(sp => sp.Description).HasMaxLength(1000);
                entity.Property(sp => sp.BusinessName).HasMaxLength(255);
                entity.Property(sp => sp.CreatedAt).IsRequired();
                entity.Property(sp => sp.UpdatedAt).IsRequired();
                entity.HasOne(sp => sp.User)
                    .WithOne(u => u.SellerProfile)
                    .HasForeignKey<SellerProfile>(sp => sp.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasMany(sp => sp.ReviewReplies)
                    .WithOne(rr => rr.SellerProfile)
                    .HasForeignKey(rr => rr.SellerProfileId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Configure relationship for PrimaryCategory
                entity.HasOne(sp => sp.PrimaryCategory)
                    .WithMany()
                    .HasForeignKey(sp => sp.PrimaryCategoryId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // Configure Certification entity
            modelBuilder.Entity<Certification>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Name).IsRequired().HasMaxLength(255);
                entity.Property(c => c.DocumentPath).HasMaxLength(500);
                entity.Property(c => c.AdminNotes).HasMaxLength(1000);
                entity.Property(c => c.SubmittedAt).IsRequired();
                entity.HasOne(c => c.SellerProfile)
                    .WithMany(sp => sp.Certifications)
                    .HasForeignKey(c => c.SellerProfileId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure Product entity
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Name).IsRequired().HasMaxLength(255);
                entity.Property(p => p.Description).HasMaxLength(2000);
                entity.Property(p => p.ImagePath).HasMaxLength(500);
                entity.Property(p => p.Category).HasMaxLength(100);
                entity.Property(p => p.CreatedAt).IsRequired();
                entity.Property(p => p.UpdatedAt).IsRequired();
                entity.HasOne(p => p.SellerProfile)
                    .WithMany(sp => sp.Products)
                    .HasForeignKey(p => p.SellerProfileId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(p => p.ProductCategory)
                    .WithMany()
                    .HasForeignKey(p => p.CategoryId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // Configure RFQ entity
            modelBuilder.Entity<RFQ>(entity =>
            {
                entity.HasKey(r => r.Id);
                entity.Property(r => r.Title).IsRequired().HasMaxLength(200);
                entity.Property(r => r.Description).IsRequired();
                entity.Property(r => r.CreatedAt).IsRequired();
                entity.HasOne(r => r.BuyerProfile)
                    .WithMany(bp => bp.CreatedRFQs)
                    .HasForeignKey(r => r.BuyerProfileId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure RFQItem entity
            modelBuilder.Entity<RFQItem>(entity =>
            {
                entity.HasKey(ri => ri.Id);
                entity.Property(ri => ri.ProductName).IsRequired().HasMaxLength(200);
                entity.Property(ri => ri.Description).HasMaxLength(1000);
                entity.Property(ri => ri.Unit).HasMaxLength(50);
                entity.HasOne(ri => ri.RFQ)
                    .WithMany(r => r.Items)
                    .HasForeignKey(ri => ri.RFQId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure RFQRecipient entity
            modelBuilder.Entity<RFQRecipient>(entity =>
            {
                entity.HasKey(rr => rr.Id);
                entity.HasOne(rr => rr.RFQ)
                    .WithMany(r => r.Recipients)
                    .HasForeignKey(rr => rr.RFQId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(rr => rr.SellerProfile)
                    .WithMany(sp => sp.RFQRecipients)
                    .HasForeignKey(rr => rr.SellerProfileId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure Quote entity
            modelBuilder.Entity<Quote>(entity =>
            {
                entity.HasKey(q => q.Id);
                entity.Property(q => q.Price).HasColumnType("decimal(18,2)").IsRequired();
                entity.Property(q => q.Conditions).IsRequired();
                entity.Property(q => q.Notes).HasMaxLength(1000);
                entity.Property(q => q.SubmittedAt).IsRequired();
                entity.HasOne(q => q.RFQ)
                    .WithMany(r => r.Quotes)
                    .HasForeignKey(q => q.RFQId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(q => q.SellerProfile)
                    .WithMany(sp => sp.Quotes)
                    .HasForeignKey(q => q.SellerProfileId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure Notification entity
            modelBuilder.Entity<Notification>(entity =>
            {
                entity.HasKey(n => n.Id);
                entity.Property(n => n.Title).IsRequired().HasMaxLength(200);
                entity.Property(n => n.Message).IsRequired();
                entity.Property(n => n.CreatedAt).IsRequired();
                entity.Property(n => n.RelatedEntityType).HasMaxLength(50);
            });

            // Configure NotificationPreferences entity
            modelBuilder.Entity<NotificationPreferences>(entity =>
            {
                entity.HasKey(np => np.Id);
                entity.HasIndex(np => np.UserId).IsUnique();
                entity.Property(np => np.UpdatedAt).IsRequired();
            });

            // Configure ContentReport entity
            modelBuilder.Entity<ContentReport>(entity =>
            {
                entity.HasKey(cr => cr.Id);
                entity.Property(cr => cr.Reason).IsRequired().HasMaxLength(100);
                entity.Property(cr => cr.Description).HasMaxLength(1000);
                entity.Property(cr => cr.ReportedAt).IsRequired();
                entity.HasOne(cr => cr.ReportedBy)
                    .WithMany()
                    .HasForeignKey(cr => cr.ReportedById)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(cr => cr.ResolvedBy)
                    .WithMany()
                    .HasForeignKey(cr => cr.ResolvedById)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            // Configure ModerationAuditLog entity
            modelBuilder.Entity<ModerationAuditLog>(entity =>
            {
                entity.HasKey(mal => mal.Id);
                entity.Property(mal => mal.Notes).HasMaxLength(1000);
                entity.Property(mal => mal.ModeratedAt).IsRequired();
                entity.HasOne(mal => mal.ContentReport)
                    .WithMany()
                    .HasForeignKey(mal => mal.ContentReportId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(mal => mal.ModeratedBy)
                    .WithMany()
                    .HasForeignKey(mal => mal.ModeratedById)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure PasswordResetToken entity
            modelBuilder.Entity<PasswordResetToken>(entity =>
            {
                entity.HasKey(prt => prt.Id);
                entity.HasIndex(prt => prt.Token).IsUnique();
                entity.Property(prt => prt.Token).IsRequired().HasMaxLength(255);
                entity.Property(prt => prt.CreatedAt).IsRequired();
                entity.Property(prt => prt.ExpiresAt).IsRequired();
                entity.Property(prt => prt.Used).IsRequired();
                entity.Property(prt => prt.UpdatedAt);
                entity.HasOne(prt => prt.User)
                    .WithMany()
                    .HasForeignKey(prt => prt.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure Favorite entity
            modelBuilder.Entity<Favorite>(entity =>
            {
                entity.HasKey(f => f.Id);
                entity.Property(f => f.CreatedAt).IsRequired();
                entity.HasOne(f => f.BuyerProfile)
                    .WithMany(bp => bp.Favorites)
                    .HasForeignKey(f => f.BuyerProfileId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(f => f.SellerProfile)
                    .WithMany()
                    .HasForeignKey(f => f.SellerProfileId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure Review entity
            modelBuilder.Entity<Review>(entity =>
            {
                entity.HasKey(r => r.Id);
                entity.Property(r => r.Rating).IsRequired();
                entity.Property(r => r.Comment).HasMaxLength(1000);
                entity.Property(r => r.ReportedReason).HasMaxLength(500);
                entity.Property(r => r.CreatedAt).IsRequired();
                entity.HasOne(r => r.BuyerProfile)
                    .WithMany(bp => bp.ReviewsGiven)
                    .HasForeignKey(r => r.BuyerProfileId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(r => r.SellerProfile)
                    .WithMany(sp => sp.ReviewsReceived)
                    .HasForeignKey(r => r.SellerProfileId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure ReviewReport entity
            modelBuilder.Entity<ReviewReport>(entity =>
            {
                entity.HasKey(rr => rr.Id);
                entity.Property(rr => rr.Reason).IsRequired().HasMaxLength(500);
                entity.Property(rr => rr.CreatedAt).IsRequired();
                entity.Property(rr => rr.ModerationResult).HasMaxLength(100);
                entity.HasOne(rr => rr.Review)
                    .WithMany()
                    .HasForeignKey(rr => rr.ReviewId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure Payment entity
            modelBuilder.Entity<Payment>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Amount).HasColumnType("decimal(18,2)").IsRequired();
                entity.Property(p => p.Currency).IsRequired().HasMaxLength(3);
                entity.Property(p => p.PaymentProvider).IsRequired().HasMaxLength(50);
                entity.Property(p => p.ProviderTransactionId).HasMaxLength(255);
                entity.Property(p => p.PaymentMethod).HasMaxLength(50);
                entity.Property(p => p.Description).HasMaxLength(500);
                entity.Property(p => p.CreatedAt).IsRequired();
                entity.Property(p => p.UpdatedAt).IsRequired();
                entity.HasOne(p => p.SellerProfile)
                    .WithMany()
                    .HasForeignKey(p => p.SellerProfileId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure PremiumSubscription entity
            modelBuilder.Entity<PremiumSubscription>(entity =>
            {
                entity.HasKey(ps => ps.Id);
                entity.Property(ps => ps.PlanType).IsRequired().HasMaxLength(50);
                entity.Property(ps => ps.StartDate).IsRequired();
                entity.Property(ps => ps.CreatedAt).IsRequired();
                entity.Property(ps => ps.UpdatedAt).IsRequired();
                entity.HasOne(ps => ps.SellerProfile)
                    .WithMany(sp => sp.PremiumSubscriptions)
                    .HasForeignKey(ps => ps.SellerProfileId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(ps => ps.Payment)
                    .WithMany()
                    .HasForeignKey(ps => ps.PaymentId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // Configure AuditLog entity
            modelBuilder.Entity<AuditLog>(entity =>
            {
                entity.HasKey(al => al.Id);
                entity.Property(al => al.Action).IsRequired().HasMaxLength(100);
                entity.Property(al => al.EntityName).IsRequired().HasMaxLength(100);
                // UserId is Guid? so no HasMaxLength needed
                entity.Property(al => al.UserName).HasMaxLength(100);
                entity.Property(al => al.UserRole).HasMaxLength(50);
                entity.Property(al => al.IPAddress).HasMaxLength(50);
                entity.Property(al => al.UserAgent).HasMaxLength(100);
                entity.Property(al => al.OperationType).IsRequired().HasMaxLength(20);
                entity.Property(al => al.CreatedAt).IsRequired();
                entity.Property(al => al.CreatedBy).HasMaxLength(255);
                
                // Configure the Details property properly
                entity.Property(al => al.Details).HasMaxLength(1000);
                
                // Configure indexes
                entity.HasIndex(al => al.Timestamp);
                entity.HasIndex(al => al.UserId);
                entity.HasIndex(al => al.EntityName);
                entity.HasIndex(al => al.EntityId);
                entity.HasIndex(al => al.Action);
                entity.HasIndex(al => al.OperationType);
                
                // Explicitly configure the table name to ensure it's created
                entity.ToTable("AuditLogs");
            });

            // Configure ContractTemplate entity
            modelBuilder.Entity<ContractTemplate>(entity =>
            {
                entity.HasKey(ct => ct.Id);
                entity.Property(ct => ct.Name).IsRequired().HasMaxLength(255);
                entity.Property(ct => ct.Title).IsRequired().HasMaxLength(255);
                entity.Property(ct => ct.Content).IsRequired();
                entity.Property(ct => ct.TemplateType).IsRequired().HasMaxLength(50);
                entity.Property(ct => ct.CreatedAt).IsRequired();
                entity.Property(ct => ct.UpdatedAt).IsRequired();
                entity.Property(ct => ct.CustomFields)
                    .HasConversion(
                        v => v == null ? null : System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                        v => v == null ? null : System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(v, (System.Text.Json.JsonSerializerOptions?)null));
                entity.HasOne(ct => ct.CreatedBySellerProfile)
                    .WithMany()
                    .HasForeignKey(ct => ct.CreatedBySellerProfileId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure ContractInstance entity
            modelBuilder.Entity<ContractInstance>(entity =>
            {
                entity.HasKey(ci => ci.Id);
                entity.HasIndex(ci => ci.ContractNumber).IsUnique();
                entity.Property(ci => ci.ContractNumber).IsRequired().HasMaxLength(100);
                entity.Property(ci => ci.Content).IsRequired();
                entity.Property(ci => ci.Status).IsRequired().HasMaxLength(50);
                entity.Property(ci => ci.CreatedAt).IsRequired();
                entity.Property(ci => ci.UpdatedAt).IsRequired();
                entity.HasOne(ci => ci.ContractTemplate)
                    .WithMany(ct => ct.ContractInstances)
                    .HasForeignKey(ci => ci.ContractTemplateId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(ci => ci.BuyerProfile)
                    .WithMany()
                    .HasForeignKey(ci => ci.BuyerProfileId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(ci => ci.SellerProfile)
                    .WithMany()
                    .HasForeignKey(ci => ci.SellerProfileId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(ci => ci.Rfq)
                    .WithMany()
                    .HasForeignKey(ci => ci.RfqId)
                    .OnDelete(DeleteBehavior.SetNull);
                entity.HasOne(ci => ci.Quote)
                    .WithMany()
                    .HasForeignKey(ci => ci.QuoteId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // Configure ReviewReply entity
            modelBuilder.Entity<ReviewReply>(entity =>
            {
                entity.HasKey(rr => rr.Id);
                entity.Property(rr => rr.ReplyContent).IsRequired().HasMaxLength(1000);
                entity.Property(rr => rr.CreatedAt).IsRequired();
                entity.Property(rr => rr.UpdatedAt).IsRequired();
                entity.HasOne(rr => rr.Review)
                    .WithOne(r => r.ReviewReply)
                    .HasForeignKey<ReviewReply>(rr => rr.ReviewId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(rr => rr.SellerProfile)
                    .WithMany()
                    .HasForeignKey(rr => rr.SellerProfileId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure PaymentInvoice entity
            modelBuilder.Entity<PaymentInvoice>(entity =>
            {
                entity.HasKey(pi => pi.Id);
                entity.HasIndex(pi => pi.InvoiceNumber).IsUnique();
                entity.Property(pi => pi.InvoiceNumber).IsRequired().HasMaxLength(100);
                entity.Property(pi => pi.Status).IsRequired().HasMaxLength(50);
                entity.Property(pi => pi.IssuedAt).IsRequired();
                entity.Property(pi => pi.CreatedAt).IsRequired();
                entity.HasOne(pi => pi.Payment)
                    .WithMany()
                    .HasForeignKey(pi => pi.PaymentId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(pi => pi.SellerProfile)
                    .WithMany()
                    .HasForeignKey(pi => pi.SellerProfileId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(pi => pi.BuyerProfile)
                    .WithMany()
                    .HasForeignKey(pi => pi.BuyerProfileId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure StaticContent entity
            modelBuilder.Entity<StaticContent>(entity =>
            {
                entity.HasKey(sc => sc.Id);
                entity.HasIndex(sc => sc.PageSlug).IsUnique();
                entity.Property(sc => sc.PageSlug).IsRequired().HasMaxLength(255);
                entity.Property(sc => sc.Title).IsRequired().HasMaxLength(255);
                entity.Property(sc => sc.Content).IsRequired();
                entity.Property(sc => sc.ContentType).IsRequired().HasMaxLength(50);
                entity.Property(sc => sc.CreatedAt).IsRequired();
                entity.Property(sc => sc.UpdatedAt).IsRequired();
                entity.Property(sc => sc.CreatedBy).IsRequired().HasMaxLength(255);
            });

            // Configure ContentCategory entity
            modelBuilder.Entity<ContentCategory>(entity =>
            {
                entity.HasKey(cc => cc.Id);
                entity.HasIndex(cc => cc.Slug).IsUnique();
                entity.Property(cc => cc.Name).IsRequired().HasMaxLength(100);
                entity.Property(cc => cc.Slug).IsRequired().HasMaxLength(50);
                entity.Property(cc => cc.CreatedAt).IsRequired();
                entity.Property(cc => cc.UpdatedAt).IsRequired();
            });

            // Configure ContentItem entity
            modelBuilder.Entity<ContentItem>(entity =>
            {
                entity.HasKey(ci => ci.Id);
                entity.HasIndex(ci => ci.Slug).IsUnique();
                entity.Property(ci => ci.Title).IsRequired().HasMaxLength(255);
                entity.Property(ci => ci.Content).IsRequired();
                entity.Property(ci => ci.Slug).IsRequired().HasMaxLength(255);
                entity.Property(ci => ci.ContentType).IsRequired().HasMaxLength(50);
                entity.Property(ci => ci.CreatedAt).IsRequired();
                entity.Property(ci => ci.UpdatedAt).IsRequired();
                entity.Property(ci => ci.CreatedBy).IsRequired().HasMaxLength(255);
                entity.HasOne(ci => ci.Category)
                    .WithMany(c => c.ContentItems)
                    .HasForeignKey(ci => ci.CategoryId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // Configure ContentTag entity
            modelBuilder.Entity<ContentTag>(entity =>
            {
                entity.HasKey(ct => ct.Id);
                entity.HasIndex(ct => ct.Slug).IsUnique();
                entity.Property(ct => ct.Name).IsRequired().HasMaxLength(50);
                entity.Property(ct => ct.Slug).IsRequired().HasMaxLength(50);
                entity.Property(ct => ct.CreatedAt).IsRequired();
                entity.Property(ct => ct.UpdatedAt).IsRequired();
            });

            // Configure ContentItemTag entity
            modelBuilder.Entity<ContentItemTag>(entity =>
            {
                entity.HasKey(cit => new { cit.ContentItemId, cit.ContentTagId });
                entity.HasOne(cit => cit.ContentItem)
                    .WithMany(ci => ci.Tags)
                    .HasForeignKey(cit => cit.ContentItemId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(cit => cit.ContentTag)
                    .WithMany(ct => ct.ContentItems)
                    .HasForeignKey(cit => cit.ContentTagId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure ServiceTier entity
            modelBuilder.Entity<ServiceTier>(entity =>
            {
                entity.HasKey(st => st.Id);
                entity.Property(st => st.Name).IsRequired().HasMaxLength(100);
                entity.Property(st => st.Description).HasMaxLength(500);
                entity.Property(st => st.Price).HasColumnType("decimal(18,2)").IsRequired();
                entity.Property(st => st.CreatedAt).IsRequired();
                entity.Property(st => st.UpdatedAt).IsRequired();
            });

            // Configure ServiceTierFeature entity
            modelBuilder.Entity<ServiceTierFeature>(entity =>
            {
                entity.HasKey(stf => stf.Id);
                entity.Property(stf => stf.Name).IsRequired().HasMaxLength(200);
                entity.Property(stf => stf.Description).HasMaxLength(500);
                entity.Property(stf => stf.Value).HasMaxLength(255);
                entity.Property(stf => stf.CreatedAt).IsRequired();
                entity.Property(stf => stf.UpdatedAt).IsRequired();
                entity.HasOne(stf => stf.ServiceTier)
                    .WithMany(st => st.Features)
                    .HasForeignKey(stf => stf.ServiceTierId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure ServiceTierConfiguration entity
            modelBuilder.Entity<ServiceTierConfiguration>(entity =>
            {
                entity.HasKey(stc => stc.Id);
                entity.Property(stc => stc.Key).IsRequired().HasMaxLength(100);
                entity.Property(stc => stc.Description).HasMaxLength(500);
                entity.Property(stc => stc.DataType).HasMaxLength(50);
                entity.Property(stc => stc.CreatedAt).IsRequired();
                entity.Property(stc => stc.UpdatedAt).IsRequired();
                entity.HasOne(stc => stc.ServiceTier)
                    .WithMany()
                    .HasForeignKey(stc => stc.ServiceTierId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure ProductCategory entity
            modelBuilder.Entity<ProductCategory>(entity =>
            {
                entity.HasKey(pc => pc.Id);
                entity.Property(pc => pc.Name).IsRequired().HasMaxLength(100);
                entity.Property(pc => pc.Description).HasMaxLength(500);
                entity.Property(pc => pc.CreatedDate).IsRequired();
                entity.Property(pc => pc.IsActive).IsRequired();
                entity.HasOne(pc => pc.ParentCategory)
                    .WithMany(pc => pc.SubCategories)
                    .HasForeignKey(pc => pc.ParentCategoryId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure CategoryConfiguration entity
            modelBuilder.Entity<CategoryConfiguration>(entity =>
            {
                entity.HasKey(cc => cc.Id);
                entity.Property(cc => cc.RequiredCertifications).HasMaxLength(2000);
                entity.Property(cc => cc.AdditionalFields).HasMaxLength(2000);
                entity.Property(cc => cc.CreatedDate).IsRequired();
                entity.Property(cc => cc.AllowsVerifiedBadge).IsRequired();
                entity.Property(cc => cc.MinCertificationsForBadge).IsRequired();
                entity.HasOne(cc => cc.Category)
                    .WithMany(c => c.CategoryConfigurations)
                    .HasForeignKey(cc => cc.CategoryId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure Cart entity
            modelBuilder.Entity<Cart>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.HasIndex(c => c.UserId).IsUnique(false);
                entity.Property(c => c.CreatedAt).IsRequired();
                entity.Property(c => c.UpdatedAt).IsRequired();
                entity.HasMany(c => c.Items)
                    .WithOne(ci => ci.Cart)
                    .HasForeignKey(ci => ci.CartId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure CartItem entity
            modelBuilder.Entity<CartItem>(entity =>
            {
                entity.HasKey(ci => ci.Id);
                entity.Property(ci => ci.Quantity).IsRequired();
                entity.Property(ci => ci.Price).HasColumnType("decimal(18,2)").IsRequired();
                entity.Property(ci => ci.CreatedAt).IsRequired();
                entity.Property(ci => ci.UpdatedAt).IsRequired();
                entity.HasOne(ci => ci.Cart)
                    .WithMany(c => c.Items)
                    .HasForeignKey(ci => ci.CartId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure Address entity
            modelBuilder.Entity<Address>(entity =>
            {
                entity.HasKey(a => a.Id);
                entity.Property(a => a.RecipientName).IsRequired().HasMaxLength(255);
                entity.Property(a => a.Street).IsRequired().HasMaxLength(500);
                entity.Property(a => a.City).IsRequired().HasMaxLength(100);
                entity.Property(a => a.State).HasMaxLength(100);
                entity.Property(a => a.ZipCode).HasMaxLength(20);
                entity.Property(a => a.Country).IsRequired().HasMaxLength(100);
                entity.Property(a => a.CreatedAt).IsRequired();
                entity.Property(a => a.UpdatedAt).IsRequired();
                entity.HasOne(a => a.User)
                    .WithMany()
                    .HasForeignKey(a => a.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure PaymentMethod entity
            modelBuilder.Entity<PaymentMethod>(entity =>
            {
                entity.HasKey(pm => pm.Id);
                entity.Property(pm => pm.Type).IsRequired().HasMaxLength(50);
                entity.Property(pm => pm.CardNumber).HasMaxLength(20); // Usually only last 4 digits are stored
                entity.Property(pm => pm.ExpiryDate).HasMaxLength(5);
                entity.Property(pm => pm.CVV).HasMaxLength(4); // Usually 3-4 digits
                entity.Property(pm => pm.CardholderName).HasMaxLength(255);
                entity.Property(pm => pm.CreatedAt).IsRequired();
                entity.Property(pm => pm.UpdatedAt).IsRequired();
                entity.HasOne(pm => pm.User)
                    .WithMany()
                    .HasForeignKey(pm => pm.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure Order entity
            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(o => o.Id);
                entity.Property(o => o.Status).IsRequired().HasMaxLength(50);
                entity.Property(o => o.TotalAmount).HasColumnType("decimal(18,2)").IsRequired();
                entity.Property(o => o.Currency).IsRequired().HasMaxLength(3);
                entity.Property(o => o.SpecialInstructions).HasMaxLength(1000);
                entity.Property(o => o.TrackingNumber).HasMaxLength(100);
                entity.Property(o => o.ShippedWith).HasMaxLength(100);
                entity.Property(o => o.ShippingCost).HasColumnType("decimal(18,2)");
                entity.Property(o => o.CreatedAt).IsRequired();
                entity.Property(o => o.UpdatedAt).IsRequired();
                entity.HasOne(o => o.User)
                    .WithMany()
                    .HasForeignKey(o => o.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(o => o.DeliveryAddress)
                    .WithMany()
                    .HasForeignKey(o => o.DeliveryAddressId)
                    .OnDelete(DeleteBehavior.SetNull);
                entity.HasOne(o => o.PaymentMethod)
                    .WithMany()
                    .HasForeignKey(o => o.PaymentMethodId)
                    .OnDelete(DeleteBehavior.SetNull);
                entity.HasMany(o => o.OrderItems)
                    .WithOne(oi => oi.Order)
                    .HasForeignKey(oi => oi.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasMany(o => o.StatusHistory)
                    .WithOne(osh => osh.Order)
                    .HasForeignKey(osh => osh.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure OrderItem entity
            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.HasKey(oi => oi.Id);
                entity.Property(oi => oi.ProductName).IsRequired().HasMaxLength(255);
                entity.Property(oi => oi.Quantity).IsRequired();
                entity.Property(oi => oi.UnitPrice).HasColumnType("decimal(18,2)").IsRequired();
                entity.Property(oi => oi.TotalPrice).HasColumnType("decimal(18,2)").IsRequired();
                entity.Property(oi => oi.CreatedAt).IsRequired();
                entity.HasOne(oi => oi.Order)
                    .WithMany(o => o.OrderItems)
                    .HasForeignKey(oi => oi.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure OrderStatusHistory entity
            modelBuilder.Entity<OrderStatusHistory>(entity =>
            {
                entity.HasKey(osh => osh.Id);
                entity.Property(osh => osh.Status).IsRequired().HasMaxLength(50);
                entity.Property(osh => osh.Notes).HasMaxLength(500);
                entity.Property(osh => osh.ChangedAt).IsRequired();
                entity.HasOne(osh => osh.Order)
                    .WithMany(o => o.StatusHistory)
                    .HasForeignKey(osh => osh.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}