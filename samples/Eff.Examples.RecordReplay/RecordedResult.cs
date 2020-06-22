﻿#pragma warning disable 1998
using Nessos.Effects.Handlers;
using System;

namespace Nessos.Effects.Examples.RecordReplay
{
    public class RecordedResult
    {
        public Exception? Exception { get; set; }
        public object? Value { get; set; }

        public static RecordedResult FromAwaiter<TResult>(Awaiter<TResult> awaiter)
        {
            return (awaiter.Exception is Exception e) ?
                new RecordedResult() { Exception = e } :
                new RecordedResult() { Value = awaiter.Result };
        }

        public void ToAwaiter<TResult>(Awaiter<TResult> awaiter)
        {
            if (Exception is Exception e)
            {
                awaiter.SetException(e);
            }
            else
            {
                awaiter.SetResult((TResult)Value!);
            }
        }
    }
}
