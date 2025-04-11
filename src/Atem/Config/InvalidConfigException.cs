using System;

namespace Atem.Config
{
    public class InvalidConfigException : Exception
    {
        public InvalidConfigException(string message, Exception innerException) : base(message, innerException) { }
    }
}