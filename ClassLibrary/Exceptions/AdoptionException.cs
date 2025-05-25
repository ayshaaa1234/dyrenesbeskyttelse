using System;

namespace ClassLibrary.Exceptions
{
    /// <summary>
    /// Undtagelse der kastes ved adoptions-relaterede fejl
    /// </summary>
    public class AdoptionException : Exception
    {
        /// <summary>
        /// Opretter en ny AdoptionException
        /// </summary>
        public AdoptionException() : base("Der opstod en fejl under adoptionen")
        {
        }

        /// <summary>
        /// Opretter en ny AdoptionException med en specifik besked
        /// </summary>
        public AdoptionException(string message) : base(message)
        {
        }

        /// <summary>
        /// Opretter en ny AdoptionException med en specifik besked og en indre undtagelse
        /// </summary>
        public AdoptionException(string message, Exception innerException) 
            : base(message, innerException)
        {
        }
    }
} 