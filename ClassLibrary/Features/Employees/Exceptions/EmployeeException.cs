using System;

namespace ClassLibrary.Features.Employees.Exceptions
{
    /// <summary>
    /// Undtagelse der kastes ved Employee-relaterede fejl
    /// </summary>
    public class EmployeeException : Exception
    {
        /// <summary>
        /// Initialiserer en ny instans af <see cref="EmployeeException"/> klassen med en standardmeddelelse.
        /// </summary>
        public EmployeeException() : base("Der opstod en fejl i Employee håndteringen.")
        {
        }

        /// <summary>
        /// Initialiserer en ny instans af <see cref="EmployeeException"/> klassen med en specificeret fejlmeddelelse.
        /// </summary>
        /// <param name="message">Meddelelsen der beskriver fejlen.</param>
        public EmployeeException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initialiserer en ny instans af <see cref="EmployeeException"/> klassen med en specificeret fejlmeddelelse og en reference til den indre undtagelse, der er årsagen til denne undtagelse.
        /// </summary>
        /// <param name="message">Meddelelsen der beskriver fejlen.</param>
        /// <param name="innerException">Undtagelsen der er årsagen til den aktuelle undtagelse, eller en null-reference hvis ingen indre undtagelse er specificeret.</param>
        public EmployeeException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
} 