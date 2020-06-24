using Nessos.Effects.Handlers;
using System;
using System.Linq;
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

            await Assert.ThrowsAsync<NotSupportedException>(() => Test().Run(Handler));
        }

        [Fact]
        public async Task EffUntyped_AwaitEffect_ShouldThrowNotSupportedException()
        {
            async Eff Test()
            {
                await new TestEffect();
            }

            await Assert.ThrowsAsync<NotSupportedException>(() => Test().Run(Handler));
        }

        [Fact]
        public async Task EffTyped_CloningHandler_ShouldBeThreadSafe()
        {
            int counter = 0;

            async Eff<int> Foo()
            {
                for (int i = 0; i < 10; i++)
                {
                    await Task.Delay(1).AsEff();
                    Interlocked.Increment(ref counter);
                }

                return 42;
            }

            var eff = Foo();
            var handler = Handler;

            Assert.Equal(0, counter);
            await Task.WhenAll(Enumerable.Range(0, 100).Select(_ => Task.Run(() => eff.Run(handler))));
            Assert.Equal(1000, counter);
        }

        [Fact]
        public async Task EffUntyped_CloningHandler_ShouldBeThreadSafe()
        {
            int counter = 0;

            async Eff Foo()
            {
                for (int i = 0; i < 10; i++)
                {
                    await Task.Delay(1).AsEff();
                    Interlocked.Increment(ref counter);
                }
            }

            var eff = Foo();
            var handler = Handler;

            Assert.Equal(0, counter);
            await Task.WhenAll(Enumerable.Range(0, 100).Select(_ => Task.Run(() => eff.Run(handler))));
            Assert.Equal(1000, counter);
        }

        public class EffectHandlerThatDoesntCompleteAwaiter : EffectHandler
        {
            public override Task Handle<TResult>(EffectAwaiter<TResult> awaiter) => Task.CompletedTask;
        }

        [Fact]
        public async Task EffectHandlerThatDoesntCompleteAwaiter_ShouldThrowInvalidOperationException()
        {
            async Eff<int> Test()
            {
                return await new TestEffect<int>();
            }

            var handler = new EffectHandlerThatDoesntCompleteAwaiter();
            await Assert.ThrowsAsync<InvalidOperationException>(() => Test().Run(handler));
        }

        public class EffectHandlerThatSetsExceptionToAwaiter<TException> : EffectHandler
            where TException : Exception, new()
        {
            public override Task Handle<TResult>(EffectAwaiter<TResult> awaiter)
            {
                awaiter.SetException(new TException());
                return Task.CompletedTask;
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
            await Assert.ThrowsAsync<DivideByZeroException>(() => Test().Run(handler));
        }

        public class EffectHandlerThatThrowsException<TException> : EffectHandler
            where TException : Exception, new()
        {
            public override Task Handle<TResult>(EffectAwaiter<TResult> awaiter)
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
            await Assert.ThrowsAsync<DivideByZeroException>(() => Test().Run(handler));
        }
    }
}
