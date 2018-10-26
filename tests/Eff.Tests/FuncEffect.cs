using Eff.Core;
using System;
using System.Threading.Tasks;

namespace Eff.Tests
{
    public class FuncEffect<TResult> : Effect<TResult>
    {
        private readonly Func<TResult> func;

        public FuncEffect(Func<TResult> func, string memberName, string sourceFilePath, int sourceLineNumber, bool captureState)
            : base(memberName, sourceFilePath, sourceLineNumber)
        {
            this.func = func;
        }

        public Func<TResult> Func => func;
    }

    
}