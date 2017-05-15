using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Eff.Core
{
    public interface IEffectHandler 
    {

        void HandleStart<TStateMachine>(ref TStateMachine stateMachine) where TStateMachine : IAsyncStateMachine;
        void HandleSetResult<Result>(Result result);
        void HandleSetException(Exception exception);

        ValueTask<ValueTuple> Handle<TResult>(IEffect<TResult> effect);
        ValueTask<ValueTuple> Handle<TResult>(TaskEffect<TResult> effect);
        ValueTask<ValueTuple> Handle(TaskEffect effect);
        ValueTask<ValueTuple> Handle<TResult>(EffTaskEffect<TResult> effect);
    }

    
}
