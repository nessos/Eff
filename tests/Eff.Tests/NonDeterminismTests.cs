using Nessos.Effects.Examples.NonDeterminism;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Nessos.Effects.Tests
{
    public static class NonDeterminismTests
    {
        [Fact]
        public static async Task SimpleValue_HappyPath()
        {
            async Eff<int> Test() => 42;

            var expected = new int[] { 42 };
            var results = await NonDetEffectHandler.Run(Test());
            Assert.Equal(expected, results);
        }

        [Fact]
        public static async Task SimpleException_ShouldPropagate()
        {
            async Eff<int> Test(int x)
            {
                return 42 / x;
            }

            await Assert.ThrowsAsync<DivideByZeroException>(() => NonDetEffectHandler.Run(Test(0)));
        }

        [Fact]
        public static async Task SimpleTask_HappyPath()
        {
            async Eff<int> Test(int x)
            {
                return await Task.Run(() => x + 1).AsEff();
            }

            var expected = new int[] { 42 };
            var results = await NonDetEffectHandler.Run(Test(41));
            Assert.Equal(expected, results);
        }

        [Fact]
        public static async Task SimpleEffect_HappyPath()
        {
            async Eff<int> Test() => await NonDetEffect.Choose(1, 2, 3, 4);

            var expected = new int[] { 1, 2, 3, 4 };
            var results = await NonDetEffectHandler.Run(Test());
            Assert.Equal(expected, results);
        }

        [Fact]
        public static async Task MultipleValues_HappyPath()
        {
            async Eff<(bool, int, string)> Test()
            {
                var x = await NonDetEffect.Choose(false, true);
                var y = await NonDetEffect.Choose(1, 2, 3);
                var z = await NonDetEffect.Choose("foo", "bar");
                return (!x, y + 1, z);
            }

            var expected =
                from x in new[] { false, true }
                from y in new[] { 1, 2, 3 }
                from z in new[] { "foo", "bar" }
                select (!x, y + 1, z);

            var results = await NonDetEffectHandler.Run(Test());
            Assert.Equal(expected, results);
        }

        [Fact]
        public static async Task MultipleValues_Nested_HappyPath()
        {
            async Eff<(bool, int, string)> Test()
            {
                async Eff<(bool, int)> Nested()
                {
                    var x = await NonDetEffect.Choose(false, true);
                    var y = await NonDetEffect.Choose(1, 2, 3);
                    return (!x, y + 1);
                }

                var (x, y) = await Nested();
                var z = await NonDetEffect.Choose("foo", "bar");

                return (x,y,z);
            }

            var expected =
                from x in new[] { false, true }
                from y in new[] { 1, 2, 3 }
                from z in new[] { "foo", "bar" }
                select (!x, y + 1, z);

            var results = await NonDetEffectHandler.Run(Test());
            Assert.Equal(expected, results);
        }

        [Fact]
        public static async Task NestedException_ShouldPropagate()
        {
            async Eff<int> Test()
            {
                async Eff<int> Divide(int y)
                {
                    var x = await NonDetEffect.Choose(1, 2, 3);
                    return x / y;
                }

                var y = await NonDetEffect.Choose(1, 2, 0);
                return await Divide(y);
            }

            await Assert.ThrowsAsync<DivideByZeroException>(() => NonDetEffectHandler.Run(Test()));
        }

        [Fact]
        public static async Task NestedExceptionHandler_ShouldExecuteAsExpected()
        {
            async Eff<int> Test()
            {
                async Eff<int> Divide(int y)
                {
                    try
                    {
                        var x = await NonDetEffect.Choose(1, 2, 3);
                        return x / y;
                    }
                    catch (DivideByZeroException)
                    {
                        return -1;
                    }
                }

                var y = await NonDetEffect.Choose(1, 2, 0);
                return await Divide(y);
            }

            var expected =
                from y in new[] { 1, 2, 0 }
                from x in new[] { 1, 2, 3 }
                select (y == 0) ? -1 : x / y;

            var results = await NonDetEffectHandler.Run(Test());
            Assert.Equal(expected, results);
        }
    }
}
