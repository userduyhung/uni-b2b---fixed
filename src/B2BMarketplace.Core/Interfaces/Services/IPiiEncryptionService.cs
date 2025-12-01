using B2BMarketplace.Core.Entities;

namespace B2BMarketplace.Core.Interfaces.Services
{
    /// <summary>
    /// Interface for PII encryption services to protect sensitive data in audit logs (AC-NF-01)
    /// </summary>
    public interface IPiiEncryptionService
    {
        /// <summary>
        /// Encrypts sensitive information
        /// </summary>
        /// <param name="data">Data to encrypt</param>
        /// <returns>Encrypted data</returns>
        string Encrypt(string data);

        /// <summary>
        /// Decrypts sensitive information
        /// </summary>
        /// <param name="encryptedData">Encrypted data to decrypt</param>
        /// <returns>Decrypted data</returns>
        string Decrypt(string encryptedData);

        /// <summary>
        /// Encrypts PII fields in an audit log
        /// </summary>
        /// <param name="auditLog">Audit log to encrypt PII for</param>
        /// <returns>Audit log with encrypted PII</returns>
        AuditLog EncryptAuditLogPii(AuditLog auditLog);

        /// <summary>
        /// Decrypts PII fields in an audit log
        /// </summary>
        /// <param name="auditLog">Audit log with encrypted PII</param>
        /// <returns>Audit log with decrypted PII</returns>
        AuditLog DecryptAuditLogPii(AuditLog auditLog);
    }
}