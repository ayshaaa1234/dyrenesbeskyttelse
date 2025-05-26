using System;

namespace ClassLibrary.Features.Memberships.Exceptions
{
    /// <summary>
    /// Repræsenterer fejl, der opstår under behandling af medlemskaber eller medlemskabsprodukter.
    /// </summary>
    public class MembershipException : Exception
    {
        /// <summary>
        /// Initialiserer en ny instans af <see cref="MembershipException"/> klassen med en standardmeddelelse.
        /// </summary>
        public MembershipException() : base("Der opstod en fejl i Membership håndteringen.")
        {
        }

        /// <summary>
        /// Initialiserer en ny instans af <see cref="MembershipException"/> klassen med en specificeret fejlmeddelelse.
        /// </summary>
        /// <param name="message">Meddelelsen der beskriver fejlen.</param>
        public MembershipException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initialiserer en ny instans af <see cref="MembershipException"/> klassen med en specificeret fejlmeddelelse
        /// og en reference til den indre undtagelse, der er årsagen til denne undtagelse.
        /// </summary>
        /// <param name="message">Meddelelsen der beskriver fejlen.</param>
        /// <param name="innerException">Undtagelsen der er årsagen til den aktuelle undtagelse, eller en null-reference hvis ingen indre undtagelse er specificeret.</param>
        public MembershipException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
} 