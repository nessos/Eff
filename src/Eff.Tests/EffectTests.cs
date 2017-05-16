using Eff.Core;
using System;
using System.Collections.Generic;
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
            async EffTask<int> Foo(int x)
            {
                return x + 1;
            }

            Assert.Equal(2, Foo(1).Result);
        }

        [Fact]
        public void AwaitEffTaskEffect()
        {
            async EffTask<int> Bar(int x)
            {
                return x + 1;
            }
            async EffTask<int> Foo(int x)
            {
                var y = await Bar(x).AsEffect();
                return y + 1;
            }

            Assert.Equal(3, Foo(1).Result);
        }

        [Fact]
        public void AwaitCustomEffect()
        {
            async EffTask<DateTime> Foo()
            {
                var y = await Effect.DateTimeNow();
                return y;
            }
            var now = DateTime.Now;
            EffectExecutionContext.Handler = new TestEffectHandler(now);
            Assert.Equal(now, Foo().Result);
        }

        [Fact]
        public void AwaitTaskEffect()
        {
            async EffTask<int> Foo(int x)
            {
                var y = await Task.Run(() => x + 1).AsEffect();
                return y + 1;
            }

            EffectExecutionContext.Handler = new TestEffectHandler();
            Assert.Equal(3, Foo(1).Result);
        }

        [Fact]
        public void AwaitTaskDelay()
        {
            async Task<int> Bar(int x)
            {
                await Task.Delay(1000);
                return x + 1;
            }
            async EffTask<int> Foo(int x)
            {
                var y = await Bar(x).AsEffect();
                return y + 1;
            }

            EffectExecutionContext.Handler = new TestEffectHandler();
            Assert.Equal(3, Foo(1).Result);
        }

        [Fact]
        public void AwaitSequenceOfTaskEffects()
        {
            async EffTask<int> Bar(int x)
            {
                await Task.Delay(1000).AsEffect();
                var y = await Task.FromResult(x + 1).AsEffect();
                return y;
            }
            async EffTask<int> Foo(int x)
            {
                var y = await Bar(x).AsEffect();
                return y + 1;
            }

            EffectExecutionContext.Handler = new TestEffectHandler();
            var foo = Foo(1);
            Assert.Equal(3, foo.Result);
        }

        [Fact]
        public void AwaitCombinationOfEffandTaskEffects()
        {
            async EffTask<int> Bar(int x)
            {
                var y = await Task.FromResult(x + 1).AsEffect();
                return y;
            }
            async EffTask<int> Foo(int x)
            {
                await Task.Delay(1000).AsEffect();
                var y = await Bar(x).AsEffect();
                return y + 1;
            }

            EffectExecutionContext.Handler = new TestEffectHandler();
            var foo = Foo(1);
            Assert.Equal(3, foo.Result);
        }


        [Fact]
        public void TestExceptionPropagation()
        {
            async EffTask<int> Foo(int x)
            {
                return 1 / x;
            }

            EffectExecutionContext.Handler = new TestEffectHandler();
            var ex = Foo(0).Exception;
            Assert.IsType<DivideByZeroException>(ex.InnerException);
        }

        [Fact]
        public void TestExceptionPropagationWithAwait()
        {
            async EffTask<int> Bar(int x)
            {
                return 1 / x;
            }

            async EffTask<int> Foo(int x)
            {
                var y = await Bar(x).AsEffect();
                return y;
            }

            EffectExecutionContext.Handler = new TestEffectHandler();
            var ex = Foo(0).Exception;
            Assert.IsType<DivideByZeroException>(ex.InnerException);
        }

        [Fact]
        public void TestExceptionLog()
        {
            async EffTask<int> Bar(int x)
            {
                return 1 / x;
            }

            async EffTask<int> Foo(int x)
            {
                var y = await Bar(x).AsEffect();
                return y;
            }
            var handler = new TestEffectHandler();
            EffectExecutionContext.Handler = handler;
            var ex = Foo(0).Exception;
            Assert.IsType<DivideByZeroException>(ex.InnerException);
            Assert.Equal(1, handler.ExceptionLogs.Count);
            Assert.Equal(ex.InnerException, handler.ExceptionLogs[0].Exception);
        }

        [Fact]
        public void TestTraceLog()
        {
            async EffTask<int> Bar(int x)
            {
                return x + 1;
            }

            async EffTask<int> Foo(int x)
            {
                var y = await Bar(x).AsEffect();
                return y;
            }
            var handler = new TestEffectHandler();
            EffectExecutionContext.Handler = handler;
            var result = Foo(1).Result;
            Assert.Equal(2, result);
            Assert.Equal(1, handler.TraceLogs.Count);
            Assert.Equal(result, (int)handler.TraceLogs[0].Result);
        }

        [Fact]
        public void TestParametersLogging()
        {
            async EffTask<int> Foo(int x)
            {
                var y = await Task.FromResult(1).AsEffect();
                return x + y;
            }
            var handler = new TestEffectHandler();
            EffectExecutionContext.Handler = handler;
            var result = Foo(1).Result;
            Assert.Equal(2, result);
            Assert.Equal(1, handler.TraceLogs.Count);
            Assert.Equal(1, handler.TraceLogs[0].Parameters.Length);
            Assert.Equal("x", handler.TraceLogs[0].Parameters[0].name);
            Assert.Equal(1, (int)handler.TraceLogs[0].Parameters[0].value);
        }
    }
}