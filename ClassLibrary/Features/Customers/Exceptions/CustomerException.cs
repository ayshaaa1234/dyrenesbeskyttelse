using System;

namespace ClassLibrary.Features.Customers.Exceptions
{
    /// <summary>
    /// Undtagelse der kastes ved Customer-relaterede fejl
    /// </summary>
    public class CustomerException : Exception
    {
        /// <summary>
        /// Initialiserer en ny instans af <see cref="CustomerException"/> klassen med en standardfejlmeddelelse.
        /// </summary>
        public CustomerException() : base("Der opstod en fejl i Customer håndteringen.")
        {
        }

        /// <summary>
        /// Initialiserer en ny instans af <see cref="CustomerException"/> klassen med en specificeret fejlmeddelelse.
        /// </summary>
        /// <param name="message">Meddelelsen der beskriver fejlen.</param>
        public CustomerException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initialiserer en ny instans af <see cref="CustomerException"/> klassen med en specificeret fejlmeddelelse
        /// og en reference til den indre undtagelse, der er årsagen til denne undtagelse.
        /// </summary>
        /// <param name="message">Meddelelsen der beskriver fejlen.</param>
        /// <param name="innerException">Undtagelsen der er årsagen til den aktuelle undtagelse, eller en null-reference hvis ingen indre undtagelse er specificeret.</param>
        public CustomerException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
} 