using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eff.Core
{
    public class EffException : Exception
    {
        public EffException(string message) : base(message)
        { }
    }
}
