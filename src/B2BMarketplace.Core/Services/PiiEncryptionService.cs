using B2BMarketplace.Core.Entities;
using B2BMarketplace.Core.Interfaces.Services;
using System.Security.Cryptography;
using System.Text;

namespace B2BMarketplace.Core.Services
{
    /// <summary>
    /// Basic implementation of PII encryption service (AC-NF-01)
    /// Note: This is a simplified implementation. A production implementation would use proper key management, etc.
    /// </summary>
    public class PiiEncryptionService : IPiiEncryptionService
    {
        // In a real application, this key would be securely stored and managed
        private readonly byte[] _key;
        private readonly byte[] _iv;

        public PiiEncryptionService()
        {
            // For demo purposes, using a fixed key and IV
            // In production, these should be securely managed
            _key = Encoding.UTF8.GetBytes("B2BMarketplaceKey12345678901234"); // 32 bytes for AES-256
            _iv = Encoding.UTF8.GetBytes("B2B12345"); // 16 bytes for AES
        }

        public string Encrypt(string data)
        {
            if (string.IsNullOrEmpty(data))
                return data;

            using (var aesAlg = Aes.Create())
            {
                aesAlg.Key = _key;
                aesAlg.IV = _iv;

                var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (var msEncrypt = new MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (var swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(data);
                        }
                    }
                    return Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
        }

        public string Decrypt(string encryptedData)
        {
            if (string.IsNullOrEmpty(encryptedData))
                return encryptedData;

            using (var aesAlg = Aes.Create())
            {
                aesAlg.Key = _key;
                aesAlg.IV = _iv;

                var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (var msDecrypt = new MemoryStream(Convert.FromBase64String(encryptedData)))
                {
                    using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (var srDecrypt = new StreamReader(csDecrypt))
                        {
                            return srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
        }

        public AuditLog EncryptAuditLogPii(AuditLog auditLog)
        {
            if (auditLog == null) 
            {
                throw new ArgumentNullException(nameof(auditLog), "AuditLog cannot be null");
            }

            // For now, we'll just encrypt specific sensitive fields
            // In a real implementation, you might want to encrypt more fields based on configuration
            if (!string.IsNullOrEmpty(auditLog.Details))
            {
                auditLog.Details = Encrypt(auditLog.Details);
            }

            if (!string.IsNullOrEmpty(auditLog.UserName))
            {
                auditLog.UserName = Encrypt(auditLog.UserName);
            }

            if (!string.IsNullOrEmpty(auditLog.UserRole))
            {
                auditLog.UserRole = Encrypt(auditLog.UserRole);
            }

            return auditLog;
        }

        public AuditLog DecryptAuditLogPii(AuditLog auditLog)
        {
            if (auditLog == null) 
            {
                throw new ArgumentNullException(nameof(auditLog), "AuditLog cannot be null");
            }

            // Decrypt the same fields that were encrypted
            if (!string.IsNullOrEmpty(auditLog.Details))
            {
                auditLog.Details = Decrypt(auditLog.Details);
            }

            if (!string.IsNullOrEmpty(auditLog.UserName))
            {
                auditLog.UserName = Decrypt(auditLog.UserName);
            }

            if (!string.IsNullOrEmpty(auditLog.UserRole))
            {
                auditLog.UserRole = Decrypt(auditLog.UserRole);
            }

            return auditLog;
        }
    }
}