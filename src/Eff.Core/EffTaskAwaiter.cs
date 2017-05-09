using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Eff.Core
{
    public struct EffTaskAwaiter<TResult> : ICriticalNotifyCompletion
    {
        private readonly EffTask<TResult> effTask;
   
        internal EffTaskAwaiter(EffTask<TResult> effTask)
        {
            this.effTask = effTask;
        }

        public bool IsCompleted => effTask.IsCompleted;
        public TResult GetResult() => effTask.Result;
        

        public void OnCompleted(Action continuation)
        {
            effTask.AsTask().ConfigureAwait(continueOnCapturedContext: true).GetAwaiter().OnCompleted(continuation);
        }
        public void UnsafeOnCompleted(Action continuation)
        {
            effTask.AsTask().ConfigureAwait(continueOnCapturedContext: true).GetAwaiter().UnsafeOnCompleted(continuation);
        }
    }
}
