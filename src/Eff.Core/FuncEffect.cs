using System;
using System.Threading.Tasks;

namespace Eff.Core
{
    public class FuncEffect<TResult> : Effect<TResult>
    {
        private readonly Func<TResult> func;

        public FuncEffect(Func<TResult> func, string memberName, string sourceFilePath, int sourceLineNumber)
            : base(memberName, sourceFilePath, sourceLineNumber)
        {
            this.func = func;
        }

        public Func<TResult> Func => func;

        public override ValueTask<ValueTuple> Accept(IEffectHandler handler)
        {
            return handler.Handle(this);
        }

        public override void OnCompleted(Action continuation)
        {
            throw new NotSupportedException();
        }

        public override void UnsafeOnCompleted(Action continuation)
        {
            throw new NotSupportedException();
        }
    }

    
}