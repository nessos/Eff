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

        bool CaptureState { get; }
        (string name, object value)[] Parameters { get; }
        (string name, object value)[] LocalVariables { get; }

        void SetState((string name, object value)[] parameters, 
                      (string name, object value)[] localVariables);

        
        void SetException(Exception ex);

        bool HasResult { get; }
        Exception Exception { get; }
        object Result { get; }

        Eff<TResult> Await<TResult>(Func<Eff<TResult>> continuation);
    }

    

    public interface IEffect<TResult> : IEffect
    {
        TResult GetResult();
        IEffect<TResult> GetAwaiter();
        void SetResult(TResult result);
    }
}
