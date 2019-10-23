using Eff.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Eff.Examples.RecordReplay
{

    public class EffCtx
    {
        public Random Random { get; set; }
    }

    public class DoEffect<T> : Effect<T>
    {

        public Func<EffCtx, T> Func { get; private set; }
        public DoEffect(Func<EffCtx, T> func, string memberName, string sourceFilePath, int sourceLineNumber) : base(memberName, sourceFilePath, sourceLineNumber)
        {
            this.Func = func;
        }
    }


    public static class IO
    {

        public static DoEffect<T> Do<T>(Func<EffCtx, T> func,
                                        [CallerMemberName] string memberName = "",
                                        [CallerFilePath] string sourceFilePath = "",
                                        [CallerLineNumber] int sourceLineNumber = 0)
        {
            return new DoEffect<T>(ctx => func(ctx), memberName, sourceFilePath, sourceLineNumber);
        }

    }
}