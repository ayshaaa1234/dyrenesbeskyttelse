using System;

namespace ClassLibrary.Features.AnimalManagement.Exceptions
{
    /// <summary>
    /// Undtagelse der kastes ved AnimalManagement-relaterede fejl
    /// </summary>
    public class AnimalManagementException : Exception
    {
        /// <summary>
        /// Opretter en ny AnimalManagementException
        /// </summary>
        public AnimalManagementException() : base("Der opstod en fejl i Animal Management.")
        {
        }

        /// <summary>
        /// Opretter en ny AnimalManagementException med en specifik besked
        /// </summary>
        public AnimalManagementException(string message) : base(message)
        {
        }

        /// <summary>
        /// Opretter en ny AnimalManagementException med en specifik besked og en indre undtagelse
        /// </summary>
        public AnimalManagementException(string message, Exception innerException) 
            : base(message, innerException)
        {
        }
    }
} 