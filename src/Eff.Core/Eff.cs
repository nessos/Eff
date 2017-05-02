using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eff.Core
{
    public struct Eff<TResult> 
    {
        private readonly Task<TResult> task;
        private readonly TResult result;

        public Eff(TResult result)
        {
            this.task = null;
            this.result = result;
        }
        public Eff(Task<TResult> task)
        {
            this.task = task;
            this.result = default(TResult);
        }

        public Task<TResult> AsTask() => task ?? Task.FromResult(result);
        
        public bool IsCompleted => task == null || task.IsCompleted; 
        public bool IsFaulted => task == null || task.IsFaulted;
        public bool IsCanceled => task != null && task.IsCanceled;

        public TResult Result => task == null ? result : task.Result;
        public AggregateException Exception => task != null ? task.Exception : null;
    }
}
