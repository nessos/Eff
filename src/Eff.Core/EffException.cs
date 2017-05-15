using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

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
