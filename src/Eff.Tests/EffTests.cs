using Eff.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Eff.Tests
{
    public class EffTests
    {
        [Fact]
        public void SimpleReturn()
        {
            async Eff<int> Foo(int x)
            {
                return x + 1;
            }

            Assert.Equal(2, Foo(1).Result);
        }

        [Fact]
        public void AwaitEff()
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

            Assert.Equal(3, Foo(1).Result);
        }

        [Fact]
        public void AwaitTask()
        {
            async Eff<int> Foo(int x)
            {
                var y = await Task.FromResult(x + 1);
                return y + 1;
            }

            Assert.Equal(3, Foo(1).Result);
        }

        [Fact]
        public void AwaitCustomEffect()
        {
            async Eff<DateTime> Foo()
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
            async Eff<int> Foo(int x)
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
            async Eff<int> Foo(int x)
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
            async Eff<int> Bar(int x)
            {
                await Task.Delay(1000).AsEffect();
                var y = await Task.Run(() => x + 1).AsEffect();
                return y;
            }
            async Eff<int> Foo(int x)
            {
                var y = await Bar(x).AsEffect();
                return y + 1;
            }

            EffectExecutionContext.Handler = new TestEffectHandler();
            Assert.Equal(3, Foo(1).Result);
        }

    }
}
