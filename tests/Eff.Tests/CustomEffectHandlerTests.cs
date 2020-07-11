using Nessos.Effects.Handlers;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Nessos.Effects.Tests
{
    public class CustomEffectHandlerTests : EffectHandlerTests
    {
        protected override IEffectHandler Handler => new CustomEffectHandler();

        [Fact]
        public async Task AwaitCustomEffect()
        {
            async Eff<DateTime> Foo<T>()
                where T : struct, IDateTimeNowEffect
            {
                var y = await default(T).DateTimeNow().ConfigureAwait();
                return y;
            }

            var now = DateTime.Now;
            var handler = new CustomEffectHandler(now);
            Assert.Equal(now, await Foo<CustomEffect>().Run(handler));
        }

        [Fact]
        public async Task TestExceptionLog()
        {
            async Eff<int> Test(int x)
            {
                var y = await Nested(x);
                return y;

                async Eff<int> Nested(int x)
                {
                    return 1 / x;
                }
            }

            var handler = new CustomEffectHandler();
            var ex = await Assert.ThrowsAsync<DivideByZeroException>(() => Test(0).Run(handler).AsTask());

            Assert.Single(handler.ExceptionLogs);
            Assert.Equal(ex, handler.ExceptionLogs[0].Exception);
        }

        [Fact]
        public async Task TestTraceLog()
        {
            async Eff<int> Test(int x)
            {
                var y = await Nested(x);
                return y;

                async Eff<int> Nested(int x)
                {
                    return x + 1;
                }
            }

            var handler = new CustomEffectHandler();
            var result = await Test(1).Run(handler);

            Assert.Equal(2, result);
            Assert.Single(handler.TraceLogs);
            Assert.Equal(result, (int)handler.TraceLogs[0].Result!);
        }

        [Fact]
        public async Task TestParametersLogging()
        {
            async Eff<int> Test(int x)
            {
                var y = await Eff.FromResult(1);
                return x + y;
            }

            var handler = new CustomEffectHandler();
            var result = await Test(1).Run(handler);

            Assert.Equal(2, result);
            Assert.Single(handler.TraceLogs);
            Assert.Single(handler.TraceLogs[0].Parameters);
            Assert.Equal("x", handler.TraceLogs[0].Parameters![0].name);
            Assert.Equal(1, (int)handler.TraceLogs[0].Parameters![0].value!);
        }

        [Fact]
        public async Task TestLocalVariablesLogging()
        {
            async Eff<int> Test(int x)
            {
                var y = await Eff.FromResult(1);
                await Eff.CompletedEff;
                return x + y;
            }

            var handler = new CustomEffectHandler();
            var result = await Test(1).Run(handler);

            Assert.Equal(2, result);
            Assert.Equal(2, handler.TraceLogs.Count);
            Assert.Single(handler.TraceLogs[0].LocalVariables);
            Assert.Equal("y", handler.TraceLogs[0].LocalVariables![0].name);
            Assert.Equal(0, (int)handler.TraceLogs[0].LocalVariables![0].value!);
            Assert.Single(handler.TraceLogs[1].LocalVariables);
            Assert.Equal("y", handler.TraceLogs[1].LocalVariables![0].name!);
            Assert.Equal(1, (int)handler.TraceLogs[1].LocalVariables![0].value!);
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

            Assert.Equal(3, await Foo<CustomEffect>(1).Run(Handler));
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

            Assert.Equal(3, await Foo<CustomEffect>(1).Run(Handler));
        }

        [Fact]
        public async Task AwaitCaptureStateEffect()
        {
            async Eff<int> Test(int x)
            {
                var y = await Eff.FromResult(1);
                await Eff.CompletedEff;
                return x + y;
            }

            var handler = new CustomEffectHandler();
            var result = await Test(1).Run(handler);

            Assert.Equal(2, result);
            Assert.Single(handler.CaptureStateParameters);
            Assert.Equal("x", handler.CaptureStateParameters![0].name);
            Assert.Equal(1, (int)handler.CaptureStateParameters[0].value!);
            Assert.Single(handler.CaptureStateLocalVariables);
            Assert.Equal("y", handler.CaptureStateLocalVariables![0].name);
            Assert.Equal(1, (int)handler.CaptureStateLocalVariables[0].value!);
        }
    }
}
