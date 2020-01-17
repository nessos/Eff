﻿using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Nessos.Eff
{
    /// <summary>
    /// Abstract effect type.
    /// </summary>
    public abstract class Effect : Effect<Unit>
    {

    }

    /// <summary>
    /// Abstract effect type.
    /// </summary>
    /// <typeparam name="TResult">Return type of the abstract effect.</typeparam>
    public abstract class Effect<TResult>
    {
        public EffAwaiter<TResult> GetAwaiter() => new EffectEffAwaiter<TResult>(this);

        /// <summary>
        /// Configures an EffAwaiter instance with supplied parameters.
        /// </summary>
        /// <param name="callerMemberName"></param>
        /// <param name="callerFilePath"></param>
        /// <param name="callerLineNumber"></param>
        /// <returns>An EffAwaiter instance with callsite metadata.</returns>
        public EffAwaiter<TResult> ConfigureAwait([CallerMemberName] string callerMemberName = "",
                                                  [CallerFilePath] string callerFilePath = "",
                                                  [CallerLineNumber] int callerLineNumber = 0)
        {
            return new EffectEffAwaiter<TResult>(this)
            {
                CallerMemberName = callerMemberName,
                CallerLineNumber = callerLineNumber,
                CallerFilePath = callerFilePath
            };
        }
    }
}
