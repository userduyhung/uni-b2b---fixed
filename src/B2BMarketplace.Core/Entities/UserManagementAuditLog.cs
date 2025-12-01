using System.ComponentModel.DataAnnotations;

namespace B2BMarketplace.Core.Entities
{
    /// <summary>
    /// Represents an audit log entry for user management actions performed by admins
    /// </summary>
    public class UserManagementAuditLog
    {
        private DateTime _timestamp;
        private DateTime _createdAt;
        private bool _isCreated = false; // Flag to track if object has been persisted

        /// <summary>
        /// Unique identifier for the audit log entry
        /// </summary>
        public Guid Id { get; private set; }

        /// <summary>
        /// ID of the user that was managed
        /// </summary>
        public Guid UserId { get; private set; }

        /// <summary>
        /// ID of the admin who performed the action
        /// </summary>
        public Guid AdminId { get; private set; }

        /// <summary>
        /// The action performed (Lock or Unlock)
        /// </summary>
        [Required]
        public string Action { get; private set; } = string.Empty;

        /// <summary>
        /// Timestamp when the action was performed - made truly immutable (AC-03)
        /// </summary>
        [ConcurrencyCheck]
        public DateTime Timestamp
        {
            get => _timestamp;
            set
            {
                // Only allow setting the timestamp if the object hasn't been created yet
                // This helps ensure immutability once persisted
                if (!_isCreated)
                {
                    _timestamp = value;
                }
            }
        }

        /// <summary>
        /// Creation timestamp used for immutability verification (AC-03)
        /// </summary>
        [ConcurrencyCheck]
        public DateTime CreatedAt
        {
            get => _createdAt;
            private set => _createdAt = value;
        }

        /// <summary>
        /// Reason for the action (e.g., reason for locking a user)
        /// </summary>
        public string? Reason { get; private set; }

        /// <summary>
        /// IP Address of the admin who performed the action (AC-01)
        /// </summary>
        public string? IpAddress { get; private set; }

        /// <summary>
        /// User Agent of the admin who performed the action (AC-01)
        /// </summary>
        public string? UserAgent { get; private set; }

        /// <summary>
        /// Entity name that was affected (e.g., User, Product, etc.) (AC-01)
        /// </summary>
        public string? EntityName { get; private set; }

        /// <summary>
        /// ID of the entity that was affected (AC-01)
        /// </summary>
        public int? EntityId { get; private set; }

        /// <summary>
        /// Details about the action performed
        /// </summary>
        public string? Details { get; private set; }

        /// <summary>
        /// JSON representation of entity state before the action (AC-02)
        /// </summary>
        public string? BeforeValues { get; private set; }

        /// <summary>
        /// JSON representation of entity state after the action (AC-02)
        /// </summary>
        public string? AfterValues { get; private set; }

        /// <summary>
        /// Navigation property to the user that was managed
        /// </summary>
        public User? User { get; set; }

        /// <summary>
        /// Constructor to initialize default values
        /// </summary>
        public UserManagementAuditLog()
        {
            Id = Guid.NewGuid();
            _timestamp = DateTime.UtcNow;
            _createdAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Method to initialize audit log with required information for AC-01
        /// </summary>
        /// <param name="userId">ID of user that was managed</param>
        /// <param name="adminId">ID of admin performing the action</param>
        /// <param name="action">Action performed</param>
        /// <param name="reason">Reason for the action</param>
        /// <param name="ipAddress">IP Address of the admin</param>
        /// <param name="userAgent">User Agent of the admin</param>
        /// <param name="entityName">Name of the entity affected</param>
        /// <param name="entityId">ID of the entity affected</param>
        /// <param name="details">Additional details about the action</param>
        /// <param name="beforeValues">JSON representation of entity state before the action (AC-02)</param>
        /// <param name="afterValues">JSON representation of entity state after the action (AC-02)</param>
        public void InitializeAuditLog(
            Guid userId,
            Guid adminId,
            string action,
            string? reason = null,
            string? ipAddress = null,
            string? userAgent = null,
            string? entityName = null,
            int? entityId = null,
            string? details = null,
            string? beforeValues = null,
            string? afterValues = null)
        {
            // Prevent modification after the audit log has been marked as created
            if (_isCreated)
            {
                throw new InvalidOperationException("Cannot initialize audit log after it has been persisted. This violates AC-03 immutability requirement.");
            }

            Id = Guid.NewGuid(); // Generate new ID for new audit log
            UserId = userId;
            AdminId = adminId;
            Action = action ?? throw new ArgumentNullException(nameof(action));
            Reason = reason;
            IpAddress = ipAddress;
            UserAgent = userAgent;
            EntityName = entityName;
            EntityId = entityId;
            Details = details;
            BeforeValues = beforeValues;  // Added for AC-02
            AfterValues = afterValues;    // Added for AC-02
            _timestamp = DateTime.UtcNow;
            _createdAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Marks the audit log as created/persisted to prevent modification of immutable properties (AC-03)
        /// </summary>
        public void MarkAsCreated()
        {
            _isCreated = true;
        }

        /// <summary>
        /// Validates if this audit log can be modified, preventing violations of AC-03
        /// </summary>
        /// <returns>True if modifications are allowed (before persistence), false otherwise</returns>
        public bool CanBeModified()
        {
            return !_isCreated;
        }
    }
}