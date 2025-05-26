using System;

namespace ClassLibrary.Features.Employees.Exceptions
{
    /// <summary>
    /// Undtagelse der kastes ved Employee-relaterede fejl
    /// </summary>
    public class EmployeeException : Exception
    {
        public EmployeeException() : base("Der opstod en fejl i Employee håndteringen.")
        {
        }

        public EmployeeException(string message) : base(message)
        {
        }

        public EmployeeException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
} 