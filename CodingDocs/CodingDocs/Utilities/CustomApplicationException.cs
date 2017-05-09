using System;

namespace CodingDocs.Utilities
{
    public class CustomApplicationException : Exception
    {
        public CustomApplicationException()
        {
        }

        public CustomApplicationException(string message) : base(message)
        {
        }

        public CustomApplicationException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}