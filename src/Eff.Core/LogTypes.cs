using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eff.Core
{
    public struct ExceptionLog
    {
        public string CallerMemberName;
        public string CallerFilePath;
        public int CallerLineNumber;
        public Exception Exception;
    }

    public struct ResultLog<TResult>
    {
        public string CallerMemberName;
        public string CallerFilePath;
        public int CallerLineNumber;
        public TResult result;
    }
}
