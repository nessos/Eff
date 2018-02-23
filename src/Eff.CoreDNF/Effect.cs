﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Eff.Core
{
    public abstract class Effect<TResult> : IEffect<TResult>
    {
        private readonly string memberName;
        private readonly string sourceFilePath;
        private readonly int sourceLineNumber;

        protected bool hasResult;
        protected TResult result;
        protected Exception exception;
        protected object state;

        public Effect(string memberName, string sourceFilePath, int sourceLineNumber)
        {
            this.memberName = memberName;
            this.sourceFilePath = sourceFilePath;
            this.sourceLineNumber = sourceLineNumber;
        }

        public string CallerMemberName => memberName;
        public string CallerFilePath => sourceFilePath;
        public int CallerLineNumber => sourceLineNumber;

        public bool IsCompleted => hasResult || exception != null;

        public bool HasResult => hasResult;
        public Exception Exception => exception;
        public object Result => result;


        public TResult GetResult()
        {
            if (exception != null)
            {
                System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(exception).Throw();
                throw exception;
            }
            else return result;
        }
        public IEffect<TResult> GetAwaiter() => this;

        public void SetResult(TResult result)
        {
            hasResult = true;
            this.result = result;
        }

        public void SetException(Exception ex)
        {
            exception = ex;
        }

        public object State => state;
        public void SetState(object state)
        {
            this.state = state;
        }

        public virtual Task Accept(IEffectHandler handler)
        {
            return handler.Handle(this);
        }

        public virtual void OnCompleted(Action continuation)
        {
            throw new NotSupportedException();
        }

        public virtual void UnsafeOnCompleted(Action continuation)
        {
            throw new NotSupportedException();
        }


    }

    
    
}
