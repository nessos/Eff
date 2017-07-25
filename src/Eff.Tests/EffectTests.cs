#pragma warning disable 1998

using Eff.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Eff.Tests
{
    public class EffectTests
    {
        [Fact]
        public void SimpleReturn()
        {
            async Eff<int> Foo(int x)
            {
                return x + 1;
            }

            var handler = new TestEffectHandler();
            Assert.Equal(2, Foo(1).Run(handler).Result);
        }

        [Fact]
        public void AwaitEffEffect()
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
            Assert.Equal(3, Foo(1).Run(handler).Result);
        }

        [Fact]
        public void AwaitCustomEffect()
        {
            async Eff<DateTime> Foo()
            {
                var y = await CustomEffect.DateTimeNow();
                return y;
            }
            var now = DateTime.Now;
            var handler = new TestEffectHandler(now);
            Assert.Equal(now, Foo().Run(handler).Result);
        }

        [Fact]
        public void AwaitTaskEffect()
        {
            async Eff<int> Foo(int x)
            {
                var y = await Task.Run(() => x + 1).AsEffect();
                return y + 1;
            }

            var handler = new TestEffectHandler();
            Assert.Equal(3, Foo(1).Run(handler).Result);
        }

        [Fact]
        public void AwaitTaskDelay()
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
            Assert.Equal(3, Foo(1).Run(handler).Result);
        }

        [Fact]
        public void AwaitSequenceOfTaskEffects()
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
            Assert.Equal(3, foo.Result);
        }

        [Fact]
        public void AwaitCombinationOfEffandTaskEffects()
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
            Assert.Equal(3, foo.Result);
        }


        [Fact]
        public void TestExceptionPropagation()
        {
            async Eff<int> Foo(int x)
            {
                return 1 / x;
            }

            var handler = new TestEffectHandler();
            var ex = Foo(0).Run(handler).Exception;
            Assert.IsType<DivideByZeroException>(ex.InnerException);
        }

        [Fact]
        public void TestExceptionPropagationWithAwait()
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
            try
            {
                var _ = Foo(0).Run(handler).Result;
            }
            catch (AggregateException ex)
            {
                Assert.IsType<DivideByZeroException>(ex.InnerException);
            }
        }

        [Fact]
        public void TestExceptionLog()
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
            try
            {
                var _ = Foo(0).Run(handler).Result;
            }
            catch (AggregateException ex)
            {
                Assert.IsType<DivideByZeroException>(ex.InnerException);
                Assert.Equal(1, handler.ExceptionLogs.Count);
                Assert.Equal(ex.InnerException, handler.ExceptionLogs[0].Exception);
            }
        }

        [Fact]
        public void TestTraceLog()
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
            var result = Foo(1).Run(handler).Result;
            Assert.Equal(2, result);
            Assert.Equal(1, handler.TraceLogs.Count);
            Assert.Equal(result, (int)handler.TraceLogs[0].Result);
        }

        [Fact]
        public void TestParametersLogging()
        {
            async Eff<int> Foo(int x)
            {
                var y = await Task.FromResult(1).AsEffect();
                return x + y;
            }
            var handler = new TestEffectHandler();
            var result = Foo(1).Run(handler).Result;
            Assert.Equal(2, result);
            Assert.Equal(1, handler.TraceLogs.Count);
            Assert.Equal(1, handler.TraceLogs[0].Parameters.Length);
            Assert.Equal("x", handler.TraceLogs[0].Parameters[0].name);
            Assert.Equal(1, (int)handler.TraceLogs[0].Parameters[0].value);
        }

        [Fact]
        public void TestLocalVariablesLogging()
        {
            async Eff<int> Foo(int x)
            {
                var y = await Task.FromResult(1).AsEffect();
                await Task.Delay(10).AsEffect();
                return x + y;
            }
            var handler = new TestEffectHandler();
            var result = Foo(1).Run(handler).Result;
            Assert.Equal(2, result);
            Assert.Equal(2, handler.TraceLogs.Count);
            Assert.Equal(1, handler.TraceLogs[0].LocalVariables.Length);
            Assert.Equal("y", handler.TraceLogs[0].LocalVariables[0].name);
            Assert.Equal(1, (int)handler.TraceLogs[0].LocalVariables[0].value);
            Assert.Equal(1, handler.TraceLogs[1].LocalVariables.Length);
            Assert.Equal("y", handler.TraceLogs[1].LocalVariables[0].name);
            Assert.Equal(1, (int)handler.TraceLogs[1].LocalVariables[0].value);
        }

        [Fact]
        public void AwaitFuncEffect()
        {
            async Eff<int> Foo(int x)
            {
                var y = await CustomEffect.Func(() => x + 1);
                return y + 1;
            }

            var handler = new TestEffectHandler();
            Assert.Equal(3, Foo(1).Run(handler).Result);
        }

        [Fact]
        public void AwaitActionEffect()
        {
            async Eff<int> Foo(int x)
            {
                int y = 0;
                await CustomEffect.Action(() => y = x + 1);
                return y + 1;
            }

            var handler = new TestEffectHandler();
            Assert.Equal(3, Foo(1).Run(handler).Result);
        }

        [Fact]
        public void AwaitCaptureStateEffect()
        {
            async Eff<int> Foo(int x)
            {
                var y = await Task.FromResult(1).AsEffect();
                await Task.Delay(10).AsEffect(captureState : true);
                return x + y;
            }
            var handler = new TestEffectHandler();
            var result = Foo(1).Run(handler).Result;
            Assert.Equal(2, result);
            Assert.Equal(1, handler.CaptureStateParameters.Length);
            Assert.Equal("x", handler.CaptureStateParameters[0].name);
            Assert.Equal(1, (int)handler.CaptureStateParameters[0].value);
            Assert.Equal(1, handler.CaptureStateLocalVariables.Length);
            Assert.Equal("y", handler.CaptureStateLocalVariables[0].name);
            Assert.Equal(1, (int)handler.CaptureStateLocalVariables[0].value);
        }

        [Fact]
        public void TestEffectsinLoops()
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
            var result = Foo(0).Run(handler).Result;
            Assert.Equal(10000, result);
        }
    }
}