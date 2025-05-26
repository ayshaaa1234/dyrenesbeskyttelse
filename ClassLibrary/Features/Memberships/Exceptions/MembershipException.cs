using System;

namespace ClassLibrary.Features.Memberships.Exceptions
{
    public class MembershipException : Exception
    {
        public MembershipException() : base("Der opstod en fejl i Membership håndteringen.")
        {
        }

        public MembershipException(string message) : base(message)
        {
        }

        public MembershipException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
} 