using System;
using System.Threading.Tasks;

namespace Eff.Core
{
    public class ActionEffect : Effect<ValueTuple>
    {
        private readonly Action action;

        public ActionEffect(Action action, string memberName, string sourceFilePath, int sourceLineNumber)
            : base(memberName, sourceFilePath, sourceLineNumber)
        {
            this.action = action;
        }

        public Action Action => action;

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