using System;
using System.Threading.Tasks;

namespace Eff.Core
{
    public class ActionEffect : Effect<ValueTuple>
    {
        private readonly Action action;

        public ActionEffect(Action action, string memberName, string sourceFilePath, int sourceLineNumber, bool captureState)
            : base(memberName, sourceFilePath, sourceLineNumber, captureState)
        {
            this.action = action;
        }

        public Action Action => action;

        public override ValueTask<ValueTuple> Accept(IEffectHandler handler)
        {
            return handler.Handle(this);
        }
    }

    
}