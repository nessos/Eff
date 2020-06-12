using Nessos.Effects.Handlers;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Nessos.Effects.Tests
{
    public static class EffAwaiterTests
    {
        [Fact]
        public static void IncompleteAwaiter_ShouldReportCorrectStatus()
        {
            var awaiter = new TaskAwaiter<int>(new ValueTask<int>());

            Assert.False(awaiter.HasResult);
            Assert.False(awaiter.HasException);
            Assert.False(awaiter.IsCompleted);

            Assert.Throws<InvalidOperationException>(() => awaiter.Result);
            Assert.Throws<InvalidOperationException>(() => ((EffAwaiterBase)awaiter).Result);
            Assert.Null(awaiter.Exception);
        }

        [Fact]
        public static void ValueCompleteAwaiter_ShouldReportCorrectStatus()
        {
            var awaiter = new TaskAwaiter<int>(new ValueTask<int>());

            awaiter.SetResult(42);

            Assert.True(awaiter.HasResult);
            Assert.False(awaiter.HasException);
            Assert.True(awaiter.IsCompleted);

            Assert.Equal(42, awaiter.Result);
            Assert.Equal(42, ((EffAwaiterBase)awaiter).Result);
            Assert.Null(awaiter.Exception);
        }

        [Fact]
        public static void ExceptionCompleteAwaiter_ShouldReportCorrectStatus()
        {
            var awaiter = new TaskAwaiter<int>(new ValueTask<int>());

            var exn = new DivideByZeroException();
            awaiter.SetException(exn);

            Assert.False(awaiter.HasResult);
            Assert.True(awaiter.HasException);
            Assert.True(awaiter.IsCompleted);

            Assert.Throws<DivideByZeroException>(() => awaiter.Result);
            Assert.Throws<DivideByZeroException>(() => ((EffAwaiterBase)awaiter).Result);
            Assert.Equal(exn, awaiter.Exception);
        }

        [Fact]
        public static void ValueCompleteAwaiter_SettingNewResult_ShouldThrowInvalidOperationException()
        {
            var awaiter = new TaskAwaiter<int>(new ValueTask<int>());

            awaiter.SetResult(42);

            // Attempt to set new values
            Assert.Throws<InvalidOperationException>(() => awaiter.SetResult(-1));
            Assert.Throws<InvalidOperationException>(() => awaiter.SetException(new Exception()));

            // Validate postconditions
            Assert.True(awaiter.HasResult);
            Assert.False(awaiter.HasException);
            Assert.True(awaiter.IsCompleted);

            Assert.Equal(42, awaiter.Result);
            Assert.Null(awaiter.Exception);
        }

        [Fact]
        public static void ExceptionCompleteAwaiter_SettingNewResult_ShouldThrowInvalidOperationException()
        {
            var awaiter = new TaskAwaiter<int>(new ValueTask<int>());

            var exn = new DivideByZeroException();
            awaiter.SetException(exn);

            // Attempt to set new values
            Assert.Throws<InvalidOperationException>(() => awaiter.SetResult(-1));
            Assert.Throws<InvalidOperationException>(() => awaiter.SetException(new Exception()));

            // Validate postconditions
            Assert.False(awaiter.HasResult);
            Assert.True(awaiter.HasException);
            Assert.True(awaiter.IsCompleted);

            Assert.Throws<DivideByZeroException>(() => awaiter.Result);
            Assert.Equal(exn, awaiter.Exception);
        }

        [Fact]
        public static void ValueCompleteAwaiter_Clear_ShouldResetAwaiterState()
        {
            var awaiter = new TaskAwaiter<int>(new ValueTask<int>());

            var exn = new DivideByZeroException();
            awaiter.SetResult(42);

            awaiter.Clear();

            // Validate postconditions
            Assert.False(awaiter.HasResult);
            Assert.False(awaiter.HasException);
            Assert.False(awaiter.IsCompleted);

            Assert.Throws<InvalidOperationException>(() => awaiter.Result);
            Assert.Null(awaiter.Exception);
        }

        [Fact]
        public static void ExceptionCompleteAwaiter_Clear_ShouldResetAwaiterState()
        {
            var awaiter = new TaskAwaiter<int>(new ValueTask<int>());

            var exn = new DivideByZeroException();
            awaiter.SetException(exn);

            awaiter.Clear();

            // Validate postconditions
            Assert.False(awaiter.HasResult);
            Assert.False(awaiter.HasException);
            Assert.False(awaiter.IsCompleted);

            Assert.Throws<InvalidOperationException>(() => awaiter.Result);
            Assert.Null(awaiter.Exception);
        }

        [Fact]
        public static void ConfigureAwait_ShouldAddCallerInfo()
        {
            var awaiter = new TaskAwaiter<int>(new ValueTask<int>());

            Assert.False(awaiter.CallerMemberName?.Length > 0);
            Assert.False(awaiter.CallerFilePath?.Length > 0);
            Assert.False(awaiter.CallerLineNumber > 0);

            awaiter.ConfigureAwait();

            Assert.True(awaiter.CallerMemberName?.Length > 0);
            Assert.True(awaiter.CallerFilePath?.Length > 0);
            Assert.True(awaiter.CallerLineNumber > 0);
        }
    }
}
