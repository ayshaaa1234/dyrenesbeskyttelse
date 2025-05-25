using System;

namespace ClassLibrary.Exceptions
{
    /// <summary>
    /// Undtagelse der kastes når et dyr ikke findes
    /// </summary>
    public class AnimalNotFoundException : Exception
    {
        /// <summary>
        /// Opretter en ny AnimalNotFoundException
        /// </summary>
        public AnimalNotFoundException() : base("Dyret blev ikke fundet")
        {
        }

        /// <summary>
        /// Opretter en ny AnimalNotFoundException med en specifik besked
        /// </summary>
        public AnimalNotFoundException(string message) : base(message)
        {
        }

        /// <summary>
        /// Opretter en ny AnimalNotFoundException med en specifik besked og en indre undtagelse
        /// </summary>
        public AnimalNotFoundException(string message, Exception innerException) 
            : base(message, innerException)
        {
        }
    }
} 