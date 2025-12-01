using System;

namespace B2BMarketplace.Core.Exceptions
{
    /// <summary>
    /// Exception thrown when there are issues with service tier operations
    /// </summary>
    public class ServiceTierException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the ServiceTierException class
        /// </summary>
        public ServiceTierException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the ServiceTierException class with a specified error message
        /// </summary>
        /// <param name="message">The message that describes the error</param>
        public ServiceTierException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the ServiceTierException class with a specified error message and a reference to the inner exception that is the cause of this exception
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified</param>
        public ServiceTierException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    /// <summary>
    /// Exception thrown when a service tier is not found
    /// </summary>
    public class ServiceTierNotFoundException : ServiceTierException
    {
        /// <summary>
        /// Gets the ID of the service tier that was not found
        /// </summary>
        public Guid? ServiceTierId { get; }

        /// <summary>
        /// Gets the name of the service tier that was not found
        /// </summary>
        public string? ServiceTierName { get; }

        /// <summary>
        /// Initializes a new instance of the ServiceTierNotFoundException class
        /// </summary>
        public ServiceTierNotFoundException() : base("Service tier not found")
        {
        }

        /// <summary>
        /// Initializes a new instance of the ServiceTierNotFoundException class with a specified service tier ID
        /// </summary>
        /// <param name="serviceTierId">The ID of the service tier that was not found</param>
        public ServiceTierNotFoundException(Guid serviceTierId) : base($"Service tier with ID {serviceTierId} not found")
        {
            ServiceTierId = serviceTierId;
        }

        /// <summary>
        /// Initializes a new instance of the ServiceTierNotFoundException class with a specified service tier name
        /// </summary>
        /// <param name="serviceTierName">The name of the service tier that was not found</param>
        public ServiceTierNotFoundException(string serviceTierName) : base($"Service tier with name '{serviceTierName}' not found")
        {
            ServiceTierName = serviceTierName;
        }
    }

    /// <summary>
    /// Exception thrown when there are issues with service tier feature operations
    /// </summary>
    public class ServiceTierFeatureException : ServiceTierException
    {
        /// <summary>
        /// Gets the ID of the service tier feature that caused the exception
        /// </summary>
        public Guid? FeatureId { get; }

        /// <summary>
        /// Initializes a new instance of the ServiceTierFeatureException class
        /// </summary>
        public ServiceTierFeatureException() : base("Service tier feature operation failed")
        {
        }

        /// <summary>
        /// Initializes a new instance of the ServiceTierFeatureException class with a specified error message
        /// </summary>
        /// <param name="message">The message that describes the error</param>
        public ServiceTierFeatureException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the ServiceTierFeatureException class with a specified error message and feature ID
        /// </summary>
        /// <param name="message">The message that describes the error</param>
        /// <param name="featureId">The ID of the service tier feature that caused the exception</param>
        public ServiceTierFeatureException(string message, Guid featureId) : base(message)
        {
            FeatureId = featureId;
        }
    }
}