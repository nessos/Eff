using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Eff.Core
{
    public interface IEffect : ICriticalNotifyCompletion
    {
        bool IsCompleted { get; }

        string CallerMemberName { get; }
        string CallerFilePath { get; }
        int CallerLineNumber { get; }

        object State { get; }
        void SetState(object state);

        
        void SetException(Exception ex);

        bool HasResult { get; }
        Exception Exception { get; }
        object Result { get; }

        ValueTask<ValueTuple> Accept(IEffectHandler handler);

    }

    

    public interface IEffect<TResult> : IEffect
    {
        TResult GetResult();
        IEffect<TResult> GetAwaiter();
        void SetResult(TResult result);
    }
}
