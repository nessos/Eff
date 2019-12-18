#pragma warning disable 1998

using Eff.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Eff.Tests
{
    public class EffectTests
    {
        [Fact]
        public async Task SimpleReturn()
        {
            async Eff<int> Foo(int x)
            {
                return x + 1;
            }

            var handler = new TestEffectHandler();
            Assert.Equal(2, await Foo(1).Run(handler));
        }

        [Fact]
        public async Task AwaitEffEffect()
        {
            async Eff<int> Bar(int x)
            {
                return x + 1;
            }
            async Eff<int> Foo(int x)
            {
                var y = await Bar(x).AsEffect();
                return y + 1;
            }

            var handler = new TestEffectHandler();
            Assert.Equal(3, await Foo(1).Run(handler));
        }

        [Fact]
        public async Task AwaitCustomEffect()
        {
            async Eff<DateTime> Foo<T>()
                where T : struct, IDateTimeNowEffect
            {
                var y = await default(T).DateTimeNow();
                return y;
            }
            var now = DateTime.Now;
            var handler = new TestEffectHandler(now);
            Assert.Equal(now, await Foo<CustomEffect>().Run(handler));
        }

        [Fact]
        public async Task AwaitTaskEffect()
        {
            async Eff<int> Foo(int x)
            {
                var y = await Task.Run(() => x + 1).AsEffect();
                return y + 1;
            }

            var handler = new TestEffectHandler();
            Assert.Equal(3, await Foo(1).Run(handler));
        }

        [Fact]
        public async Task AwaitTaskDelay()
        {
            async Task<int> Bar(int x)
            {
                await Task.Delay(1000);
                return x + 1;
            }
            async Eff<int> Foo(int x)
            {
                var y = await Bar(x).AsEffect();
                return y + 1;
            }

            var handler = new TestEffectHandler();
            Assert.Equal(3, await Foo(1).Run(handler));
        }

        [Fact]
        public async Task AwaitSequenceOfTaskEffects()
        {
            async Eff<int> Bar(int x)
            {
                await Task.Delay(1000).AsEffect();
                var y = await Task.FromResult(x + 1).AsEffect();
                return y;
            }
            async Eff<int> Foo(int x)
            {
                var y = await Bar(x).AsEffect();
                return y + 1;
            }

            var handler = new TestEffectHandler();
            var foo = Foo(1).Run(handler);
            Assert.Equal(3, await foo);
        }

        [Fact]
        public async Task AwaitCombinationOfEffandTaskEffects()
        {
            async Eff<int> Bar(int x)
            {
                var y = await Task.FromResult(x + 1).AsEffect();
                return y;
            }
            async Eff<int> Foo(int x)
            {
                await Task.Delay(1000).AsEffect();
                var y = await Bar(x).AsEffect();
                return y + 1;
            }

            var handler = new TestEffectHandler();
            var foo = Foo(1).Run(handler);
            Assert.Equal(3, await foo);
        }


        [Fact]
        public async Task TestExceptionPropagation()
        {
            async Eff<int> Foo(int x)
            {
                return 1 / x;
            }

            var handler = new TestEffectHandler();
            await Assert.ThrowsAsync<DivideByZeroException>(() => Foo(0).Run(handler));
        }

        [Fact]
        public async Task TestExceptionPropagationWithAwait()
        {
            async Eff<int> Bar(int x)
            {
                return 1 / x;
            }

            async Eff<int> Foo(int x)
            {
                var y = await Bar(x).AsEffect();
                return y;
            }

            var handler = new TestEffectHandler();
            await Assert.ThrowsAsync<DivideByZeroException>(() => Foo(0).Run(handler));
        }

        [Fact]
        public async Task TestExceptionLog()
        {
            async Eff<int> Bar(int x)
            {
                return 1 / x;
            }

            async Eff<int> Foo(int x)
            {
                var y = await Bar(x).AsEffect();
                return y;
            }
            var handler = new TestEffectHandler();
            var ex = await Assert.ThrowsAsync<DivideByZeroException>(() => Foo(0).Run(handler));
            Assert.Single(handler.ExceptionLogs);
            Assert.Equal(ex, handler.ExceptionLogs[0].Exception);
        }

        [Fact]
        public async Task TestTraceLog()
        {
            async Eff<int> Bar(int x)
            {
                return x + 1;
            }

            async Eff<int> Foo(int x)
            {
                var y = await Bar(x).AsEffect();
                return y;
            }

            var handler = new TestEffectHandler();            
            var result = await Foo(1).Run(handler);
            Assert.Equal(2, result);
            Assert.Single(handler.TraceLogs);
            Assert.Equal(result, (int)handler.TraceLogs[0].Result);
        }

        [Fact]
        public async Task TestParametersLogging()
        {
            async Eff<int> Foo(int x)
            {
                var y = await Task.FromResult(1).AsEffect();
                return x + y;
            }
            var handler = new TestEffectHandler();
            var result = await Foo(1).Run(handler);
            Assert.Equal(2, result);
            Assert.Single(handler.TraceLogs);
            Assert.Single(handler.TraceLogs[0].Parameters);
            Assert.Equal("x", handler.TraceLogs[0].Parameters[0].name);
            Assert.Equal(1, (int)handler.TraceLogs[0].Parameters[0].value);
        }

        [Fact]
        public async Task TestLocalVariablesLogging()
        {
            async Eff<int> Foo(int x)
            {
                var y = await Task.FromResult(1).AsEffect();
                await Task.Delay(10).AsEffect();
                return x + y;
            }
            var handler = new TestEffectHandler();
            var result = await Foo(1).Run(handler);
            Assert.Equal(2, result);
            Assert.Equal(2, handler.TraceLogs.Count);
            Assert.Single(handler.TraceLogs[0].LocalVariables);
            Assert.Equal("y", handler.TraceLogs[0].LocalVariables[0].name);
            Assert.Equal(0, (int)handler.TraceLogs[0].LocalVariables[0].value);
            Assert.Single(handler.TraceLogs[1].LocalVariables);
            Assert.Equal("y", handler.TraceLogs[1].LocalVariables[0].name);
            Assert.Equal(1, (int)handler.TraceLogs[1].LocalVariables[0].value);
        }

        [Fact]
        public async Task AwaitFuncEffect()
        {
            async Eff<int> Foo<T>(int x)
                where T : struct, IFuncEffect
            {
                var y = await default(T).Func(() => x + 1);
                return y + 1;
            }

            var handler = new TestEffectHandler();
            Assert.Equal(3, await Foo<CustomEffect>(1).Run(handler));
        }

        [Fact]
        public async Task AwaitActionEffect()
        {
            async Eff<int> Foo<T>(int x)
                where T : struct, IFuncEffect
            {
                int y = 0;
                await default(T).Action(() => y = x + 1);
                return y + 1;
            }

            var handler = new TestEffectHandler();
            Assert.Equal(3, await Foo<CustomEffect>(1).Run(handler));
        }

        [Fact]
        public async Task AwaitCaptureStateEffect()
        {
            async Eff<int> Foo(int x)
            {
                var y = await Task.FromResult(1).AsEffect();
                await Task.Delay(10).AsEffect();
                return x + y;
            }
            var handler = new TestEffectHandler();
            var result = await Foo(1).Run(handler);
            Assert.Equal(2, result);
            Assert.Single(handler.CaptureStateParameters);
            Assert.Equal("x", handler.CaptureStateParameters[0].name);
            Assert.Equal(1, (int)handler.CaptureStateParameters[0].value);
            Assert.Single(handler.CaptureStateLocalVariables);
            Assert.Equal("y", handler.CaptureStateLocalVariables[0].name);
            Assert.Equal(1, (int)handler.CaptureStateLocalVariables[0].value);
        }

        [Fact]
        public async Task TestEffectsinLoops()
        {
            async Eff<int> Foo(int x)
            {
                int sum = x;
                for (int i = 0; i < 10000; i++)
                {
                    sum += await Task.FromResult(1).AsEffect();
                }
                
                return sum;
            }
            var handler = new TestEffectHandler();
            var result = await Foo(0).Run(handler);
            Assert.Equal(10000, result);
        }

        [Fact]
        public async Task DefaultEffectHandler_OnEffect_ShouldThrow()
        {
            async Eff<DateTime> Foo()
            {
                return await (new CustomEffect()).DateTimeNow();
            }

            var exn = await Assert.ThrowsAsync<EffException>(() => Foo().Run(new DefaultEffectHandler()));
            Assert.Contains("Effect DateTimeNowEffect is not completed", exn.Message);
        }

        [Fact]
        public async Task Eff_Methods_Should_Be_ThreadSafe()
        {
            int counter = 0;

            async Eff<int> Foo()
            {
                for (int i = 0; i < 10; i++)
                {
                    await Task.Delay(1).AsEffect();
                    Interlocked.Increment(ref counter);
                }

                return 1;
            }

            var eff = Foo();
            var handler = new DefaultEffectHandler() { CloneDelayedStateMachines = true };

            Assert.Equal(0, counter);
            await Task.WhenAll(Enumerable.Range(0, 100).Select(_ => Task.Run(() => eff.Run(handler))));
            Assert.Equal(1000, counter);
        }
    }
}