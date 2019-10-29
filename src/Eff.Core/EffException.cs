using System;
using System.Runtime.Serialization;

namespace Eff.Core
{
    [Serializable]
    public class EffException : Exception
    {
        public EffException(string message) : base(message)
        { }

        public EffException(string message, Exception innerException) : base(message, innerException)
        { }

        public EffException(SerializationInfo info, StreamingContext context) : base(info, context)
        { }
    }
}
