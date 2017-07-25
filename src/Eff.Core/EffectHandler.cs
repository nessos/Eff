#pragma warning disable 1998

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Eff.Core
{
    public abstract class EffectHandler : IEffectHandler
    {

        public EffectHandler()
        {
            
        }

        public abstract ValueTask<ValueTuple> Handle<TResult>(IEffect<TResult> effect);
        

       
        public virtual async ValueTask<ValueTuple> Handle<TResult>(TaskEffect<TResult> effect)
        {
            var result = await effect.Task;
            effect.SetResult(result);
            
            return ValueTuple.Create();
        }

        public virtual async ValueTask<ValueTuple> Handle<TResult>(EffEffect<TResult> effect)
        {
            var result = await effect.Eff.Run(this);
            effect.SetResult(result);

            return ValueTuple.Create();
        }

        public virtual async ValueTask<TResult> Handle<TResult>(SetResult<TResult> setResult)
        {
            return setResult.Result;
        }

        public virtual async ValueTask<ValueTuple> Handle<TResult>(SetException<TResult> setException)
        {
            System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(setException.Exception).Throw();

            return ValueTuple.Create();
        }

        public virtual async ValueTask<Eff<TResult>> Handle<TResult>(Delay<TResult> delay)
        {
            return delay.Func();
        }


        public virtual async ValueTask<Eff<TResult>> Handle<TResult>(Await<TResult> awaitEff)
        {
            var effect = awaitEff.Effect;

            // Initialize State Values
            if (effect.CaptureState)
            {
                var parameters = Utils.GetParametersValues(awaitEff.State);
                var localVariables = Utils.GetLocalVariablesValues(awaitEff.State);
                effect.SetState(parameters, localVariables);
            }

            // Execute Effect
            try
            {
                await effect.Accept(this);
                if (!effect.IsCompleted)
                    throw new EffException($"Effect {effect.GetType().Name} is not completed.");
            }
            catch (AggregateException ex)
            {
                effect.SetException(ex.InnerException);
            }
            catch (Exception ex)
            {
                effect.SetException(ex);
            }

            var eff = awaitEff.Continuation();
            return eff;
        }
    }


}
