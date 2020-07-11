using Nessos.Effects.Handlers;
using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Nessos.Effects.Tests
{
    public class DefaultEffectHandlerTests : EffectHandlerTests
    {
        protected override IEffectHandler Handler => new DefaultEffectHandler();

        [Fact]
        public async Task EffTyped_AwaitEffect_ShouldThrowNotSupportedException()
        {
            async Eff<int> Test()
            {
                return await new TestEffect<int>();
            }

            await Assert.ThrowsAsync<NotSupportedException>(() => Test().Run(Handler).AsTask());
        }

        [Fact]
        public async Task Effect_Run_ShouldThrowNotSupportedException()
        {
            var effect = new TestEffect<int>();

            await Assert.ThrowsAsync<NotSupportedException>(() => effect.Run(Handler));
        }

        [Fact]
        public async Task EffUntyped_AwaitEffect_ShouldThrowNotSupportedException()
        {
            async Eff Test()
            {
                await new TestEffect();
            }

            await Assert.ThrowsAsync<NotSupportedException>(() => Test().Run(Handler).AsTask());
        }

        [Fact]
        public async Task EffTyped_ShouldBeThreadSafe()
        {
            int counter = 0;

            async Eff<int> Test()
            {
                for (int i = 0; i < 10; i++)
                {
                    await Task.Delay(1);
                    Interlocked.Increment(ref counter);
                }

                return 42;
            }

            var eff = Test();
            var handler = Handler;

            Assert.Equal(0, counter);
            await Task.WhenAll(Enumerable.Range(0, 100).Select(_ => eff.Run(handler).AsTask()));
            Assert.Equal(1000, counter);
        }

        [Fact]
        public async Task EffUntyped_ShouldBeThreadSafe()
        {
            int counter = 0;

            async Eff Foo()
            {
                for (int i = 0; i < 10; i++)
                {
                    await Task.Delay(1);
                    Interlocked.Increment(ref counter);
                }
            }

            var eff = Foo();
            var handler = Handler;

            Assert.Equal(0, counter);
            await Task.WhenAll(Enumerable.Range(0, 100).Select(_ => eff.Run(handler).AsTask()));
            Assert.Equal(1000, counter);
        }

        public class EffectHandlerThatDoesntCompleteAwaiter : EffectHandler
        {
            public override ValueTask Handle<TResult>(EffectAwaiter<TResult> awaiter) => default;
        }

        [Fact]
        public async Task EffectHandlerThatDoesntCompleteAwaiter_ShouldThrowInvalidOperationException()
        {
            async Eff<int> Test()
            {
                return await new TestEffect<int>();
            }

            var handler = new EffectHandlerThatDoesntCompleteAwaiter();
            await Assert.ThrowsAsync<InvalidOperationException>(() => Test().Run(handler).AsTask());
        }

        public class EffectHandlerThatSetsExceptionToAwaiter<TException> : EffectHandler
            where TException : Exception, new()
        {
            public override ValueTask Handle<TResult>(EffectAwaiter<TResult> awaiter)
            {
                awaiter.SetException(new TException());
                return default;
            }
        }

        [Fact]
        public async Task EffectHandlerThatSetsExceptionToAwaiter_ShouldThrowTheException()
        {
            async Eff<int> Test()
            {
                return await new TestEffect<int>();
            }

            var handler = new EffectHandlerThatSetsExceptionToAwaiter<DivideByZeroException>();
            await Assert.ThrowsAsync<DivideByZeroException>(() => Test().Run(handler).AsTask());
        }

        public class EffectHandlerThatThrowsException<TException> : EffectHandler
            where TException : Exception, new()
        {
            public override ValueTask Handle<TResult>(EffectAwaiter<TResult> awaiter)
            {
                throw new TException();
            }
        }

        [Fact]
        public async Task EffectHandlerThatThrowsException_ShouldPropagateTheException()
        {
            async Eff<int> Test()
            {
                return await new TestEffect<int>();
            }

            var handler = new EffectHandlerThatThrowsException<DivideByZeroException>();
            await Assert.ThrowsAsync<DivideByZeroException>(() => Test().Run(handler).AsTask());
        }

        [Fact]
        public async Task Exception_Stacktrace_ShouldHaveCorrectDepth()
        {
            async Eff Test()
            {
                try
                {
                    await Nested(0);
                    throw new Exception("Should throw an exception");

                    async Eff<int> Nested(int x) => 1 / x;
                }
                catch (DivideByZeroException exception)
                {
                    var methodNames = new StackTrace(exception).GetFrames()
                        .Select(f => f!.GetMethod()!.Name)
                        .ToArray();

                    var expected = new[]
                    {
                        nameof(EffStateMachine<int>.MoveNext),
                        nameof(ExceptionDispatchInfo.Throw),
                        nameof(EffAwaiter.GetResult),
                        nameof(EffStateMachine<int>.MoveNext)
                    };

                    Assert.Equal(expected, methodNames);
                }

            }

            await Test().Run(Handler);
        }
    }
}
