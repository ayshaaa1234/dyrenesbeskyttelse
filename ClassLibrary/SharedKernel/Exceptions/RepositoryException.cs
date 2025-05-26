using System;

namespace ClassLibrary.SharedKernel.Exceptions
{
    /// <summary>
    /// Exception der kastes ved fejl i repository operationer
    /// </summary>
    public class RepositoryException : Exception
    {
        public RepositoryException() : base() { }

        public RepositoryException(string message) : base(message) { }

        public RepositoryException(string message, Exception innerException) 
            : base(message, innerException) { }
    }
} 