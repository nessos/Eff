﻿using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;

namespace Nessos.Effects.Handlers
{
    /// <summary>
    ///   Base awaiter class for Eff awaitables.
    /// </summary>
    public abstract class EffAwaiter : ICriticalNotifyCompletion
    {
        internal EffAwaiter() { }

        /// <summary>
        ///   Awaiter identifier for debugging purposes.
        /// </summary>
        public abstract string Id { get; }

        /// <summary>
        ///   Gets or sets the method or property name of the caller to the method.
        /// </summary>
        public string CallerMemberName { get; set; } = "";

        /// <summary>
        ///   Gets or sets the full path of the source file that contains the caller.
        /// </summary>
        public string CallerFilePath { get; set; } = "";

        /// <summary>
        ///   Gets or sets the line number at the source file at which the method is called.
        /// </summary>
        public int CallerLineNumber { get; set; } = 0;

        /// <summary>
        ///   Returns true if the awaiter has been completed with a result value.
        /// </summary>
        public bool HasResult { get; internal set; }

        /// <summary>
        ///   Gets the exception result for the awaiter.
        /// </summary>
        public Exception? Exception { get; internal set; }

        /// <summary>
        ///   Gets a state machine awaiting on the current awaiter instance.
        /// </summary>
        [DisallowNull]
        public IEffStateMachine? AwaitingStateMachine { get; internal set; }

        /// <summary>
        ///   Returns true if the awaiter has been completed with an exception value.
        /// </summary>
        public bool HasException => !(Exception is null);

        /// <summary>
        ///   Returns true if the awaiter has been completed with either a result or an exception.
        /// </summary>
        public bool IsCompleted => HasResult || HasException;

        /// <summary>
        ///   Sets an exception value for the awaiter.
        /// </summary>
        public abstract void SetException(Exception exception);

        /// <summary>
        ///   Processes the awaiter using the provided effect handler.
        /// </summary>
        public abstract Task Accept(IEffectHandler handler);

        /// <summary>
        ///   Configures the EffAwaiter instance with supplied parameters.
        /// </summary>
        /// <param name="callerMemberName">The method or property name of the caller to the method.</param>
        /// <param name="callerFilePath">The full path of the source file that contains the caller.</param>
        /// <param name="callerLineNumber">The line number at the source file at which the method is called.</param>
        /// <returns>An EffAwaiter instance with callsite metadata.</returns>
        public EffAwaiter ConfigureAwait(
            [CallerMemberName] string callerMemberName = "",
            [CallerFilePath] string callerFilePath = "",
            [CallerLineNumber] int callerLineNumber = 0)
        {
            CallerMemberName = callerMemberName;
            CallerFilePath = callerFilePath;
            CallerLineNumber = callerLineNumber;
            return this;
        }

        /// <summary>
        ///   For use by EffMethodBuilder
        /// </summary>
        public EffAwaiter GetAwaiter() => this;

        /// <summary>
        ///   For use by EffMethodBuilder
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)] // for stacktrace formatting purposes
        public void GetResult()
        {
            if (Exception is Exception exn)
            {
                ExceptionDispatchInfo.Capture(exn).Throw();
                return;
            }

            if (!HasResult)
            {
                throw new InvalidOperationException($"{nameof(EffAwaiter)} of type {Id} has not been completed.");
            }
        }

        void INotifyCompletion.OnCompleted(Action continuation) => throw new NotSupportedException("Eff awaitables should only be awaited in Eff methods.");
        void ICriticalNotifyCompletion.UnsafeOnCompleted(Action continuation) => throw new NotSupportedException("Eff awaitables should only be awaited in Eff methods.");
    }

    /// <summary>
    ///   Base awaiter class for Eff awaitables.
    /// </summary>
    public abstract class EffAwaiter<TResult> : EffAwaiter
    {
        [AllowNull]
        private TResult _result = default;

        /// <summary>
        ///   Gets either the result value or throws the exception that have been stored in the awaiter.
        /// </summary>
        public TResult Result => GetResult();

        /// <summary>
        ///   Sets a result value for the awaiter.
        /// </summary>
        public void SetResult(TResult value)
        {
            Exception = null;
            _result = value;
            HasResult = true;
        }

        public sealed override void SetException(Exception exception)
        {
            if (exception is null)
            {
                throw new ArgumentNullException(nameof(exception));
            }

            HasResult = false;
            _result = default;
            Exception = exception;
        }

        /// <summary>
        ///   For use by EffMethodBuilder
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)] // for stacktrace formatting purposes
        public new TResult GetResult()
        {
            if (Exception is Exception exn)
            {
                ExceptionDispatchInfo.Capture(exn).Throw();
                return default!;
            }

            if (!HasResult)
            {
                throw new InvalidOperationException($"Awaiter of type {Id} has not been completed.");
            }

            return _result;
        }

        /// <summary>
        ///   For use by EffMethodBuilder
        /// </summary>
        public new EffAwaiter<TResult> GetAwaiter() => this;

        /// <summary>
        ///   Configures the EffAwaiter instance with supplied parameters.
        /// </summary>
        /// <param name="callerMemberName">The method or property name of the caller to the method.</param>
        /// <param name="callerFilePath">The full path of the source file that contains the caller.</param>
        /// <param name="callerLineNumber">The line number at the source file at which the method is called.</param>
        /// <returns>An EffAwaiter instance with callsite metadata.</returns>
        public new EffAwaiter<TResult> ConfigureAwait(
            [CallerMemberName] string callerMemberName = "",
            [CallerFilePath] string callerFilePath = "",
            [CallerLineNumber] int callerLineNumber = 0)
        {
            CallerMemberName = callerMemberName;
            CallerFilePath = callerFilePath;
            CallerLineNumber = callerLineNumber;
            return this;
        }
    }
}
