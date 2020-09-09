using System;

namespace Gestor
{
    public class CustomException : Exception
    {
        public CustomException(string message)
            : base(message) { }
    }
}
