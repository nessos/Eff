using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Eff.Core
{
    [AsyncMethodBuilder(typeof(EffMethodBuilder<>))]
    public abstract class Eff<TResult>
    {
        public abstract ValueTask<Eff<TResult>> Handle(IEffMethodHandler<TResult> effHandler, IEffectHandler effectHandler);
    }

    public class Await<TSource, TResult> : Eff<TResult>
    {
        private readonly IEffect<TSource> effect;
        private readonly Func<TSource, Eff<TResult>> success;
        private readonly Func<Exception, Eff<TResult>> failure;

        public Await(IEffect<TSource> effect, 
                        Func<TSource, Eff<TResult>> success,
                        Func<Exception, Eff<TResult>> failure)
        {
            this.effect = effect;
            this.success = success;
            this.failure = failure;
        }

        public override ValueTask<Eff<TResult>> Handle(IEffMethodHandler<TResult> effHandler, IEffectHandler effectHandler)
        {
            return effHandler.Handle(this, effectHandler);
        }
    }

    public class SetResult<TResult> : Eff<TResult>
    {
        private readonly TResult result;

        public SetResult(TResult result)
        {
            this.result = result;
        }

        public override ValueTask<Eff<TResult>> Handle(IEffMethodHandler<TResult> effHandler, IEffectHandler effectHandler)
        {
            throw new NotImplementedException();
        }
    }

    public class SetException<TResult> : Eff<TResult>
    {
        private readonly Exception exception;

        public SetException(Exception exception)
        {
            this.exception = exception;
        }

        public override ValueTask<Eff<TResult>> Handle(IEffMethodHandler<TResult> effHandler, IEffectHandler effectHandler)
        {
            throw new NotImplementedException();
        }
    }


}
