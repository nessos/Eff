﻿using System;
using System.Threading.Tasks;

namespace Eff.Core
{
    public class TaskEffect<TResult> : Effect<TResult>
    {
        private readonly Task<TResult> task;

        public TaskEffect(Task<TResult> task, string memberName, string sourceFilePath, int sourceLineNumber)
            : base(memberName, sourceFilePath, sourceLineNumber)
        {
            this.task = task;
        }

        public Task<TResult> Task => task;

        public override Task Accept(IEffectHandler handler)
        {
            return handler.Handle(this);
        }
    }
}