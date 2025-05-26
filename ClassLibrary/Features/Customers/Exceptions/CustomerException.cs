using System;

namespace ClassLibrary.Features.Customers.Exceptions
{
    /// <summary>
    /// Undtagelse der kastes ved Customer-relaterede fejl
    /// </summary>
    public class CustomerException : Exception
    {
        public CustomerException() : base("Der opstod en fejl i Customer håndteringen.")
        {
        }

        public CustomerException(string message) : base(message)
        {
        }

        public CustomerException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
} 