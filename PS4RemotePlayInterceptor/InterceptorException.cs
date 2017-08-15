using System;

namespace PS4RemotePlayInterceptor
{
    public class InterceptorException : Exception
    {
        public InterceptorException()
        {
        }

        public InterceptorException(string message) : base(message)
        {
        }

        public InterceptorException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
