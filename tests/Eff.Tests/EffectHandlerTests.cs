using Nessos.Effects.Handlers;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Nessos.Effects.Tests
{
    public abstract class EffectHandlerTests
    {
        protected abstract IEffectHandler Handler { get; }

        [Fact]
        public async Task EffTyped_Stub()
        {
            async Eff<int> Test(int x)
            {
                return x + 1;
            }

            Assert.Equal(2, await Test(1).Run(Handler));
        }

        [Fact]
        public async Task EffUntyped_Stub()
        {
            int count = 0;

            async Eff Test(int x)
            {
                count += x;
            }

            await Test(5).Run(Handler);
            Assert.Equal(5, count);
        }

        [Fact]
        public async Task EffTyped_NestedAwait()
        {
            async Eff<int> Test(int x)
            {
                var y = await Nested(x);
                return y + 1;

                async Eff<int> Nested(int x)
                {
                    return x + 1;
                }
            }

            Assert.Equal(3, await Test(1).Run(Handler));
        }

        [Fact]
        public async Task EffUntyped_NestedAwait()
        {
            int counter = 0;

            async Eff Test(int x)
            {
                await Nested(x);
                counter++;

                async Eff Nested(int x)
                {
                    counter += x;
                }
            }

            await Test(2).Run(Handler);
            Assert.Equal(3, counter);
        }

        [Fact]
        public async Task EffTyped_NestedEffUntyped()
        {
            int counter = 0;
            async Eff<int> Test(int x)
            {
                await Nested(x);
                return counter;

                async Eff Nested(int x)
                {
                    counter += x;
                }
            }

            Assert.Equal(1, await Test(1).Run(Handler));
        }

        [Fact]
        public async Task EffUntyped_NestedEffTyped()
        {
            int counter = 0;
            async Eff Test(int x)
            {
                var y = await Nested(x);
                counter += y;

                async Eff<int> Nested(int x)
                {
                    return x + 1;
                }
            }

            await Test(1).Run(Handler);
            Assert.Equal(2, counter);
        }

        [Fact]
        public async Task EffTyped_ShouldHaveCallByNameSemantics()
        {
            int counter = 0;

            async Eff<int> Test()
            {
                return counter++;
            }

            var eff = Test();
            Assert.Equal(0, counter);
            Assert.Equal(0, await eff.Run(Handler));
            Assert.Equal(1, counter);
            Assert.Equal(1, await eff.Run(Handler));
            Assert.Equal(2, counter);
        }

        [Fact]
        public async Task EffUntyped_ShouldHaveCallByNameSemantics()
        {
            int counter = 0;

            async Eff Test()
            {
                counter++;
            }

            var eff = Test();
            Assert.Equal(0, counter);
            await eff.Run(Handler);
            Assert.Equal(1, counter);
            await eff.Run(Handler);
            Assert.Equal(2, counter);
        }

        [Fact]
        public async Task EffUntyped_ForLoop()
        {
            int counter = 0;

            async Eff Test()
            {
                for (int i = 1; i <= 10; i++)
                {
                    if (i < 5)
                    {
                        await Nested(i);
                    }
                    else
                    {
                        break;
                    }

                    async Eff Nested(int i)
                    {
                        counter += i;
                    }
                }
            }

            await Test().Run(Handler);
            Assert.Equal(10, counter);
        }

        [Fact]
        public async Task EffTyped_ForLoop()
        {
            int counter = 0;
            async Eff<int> Test()
            {
                for (int i = 1; i <= 10; i++)
                {
                    if (i < 5)
                    {
                        await Nested(i);
                    }
                    else
                    {
                        return i;
                    }

                    async Eff Nested(int i)
                    {
                        counter += i;
                    }
                }

                return -1;
            }

            await Test().Run(Handler);
            Assert.Equal(10, counter);
        }

        [Fact]
        public async Task EffTyped_AwaitTask()
        {
            async Eff<int> Test(int x)
            {
                var y = await TaskMethod();
                return y + 1;

                async Task<int> TaskMethod()
                {
                    await Task.Delay(1000);
                    return x + 1;
                }
            }

            Assert.Equal(3, await Test(1).Run(Handler));
        }

        [Fact]
        public async Task EffUntyped_AwaitTask()
        {
            int counter = 0;
            async Eff Test(int x)
            {
                await TaskMethod();
                counter++;

                async Task TaskMethod()
                {
                    await Task.Delay(1000);
                    counter += x;
                }
            }

            await Test(2).Run(Handler);
            Assert.Equal(3, counter);
        }

        public class TestEffect<T> : Effect<T>
        {

        }

        public class TestEffect : Effect
        {

        }

        [Fact]
        public async Task EffTyped_AwaitSequenceOfTaskEffects()
        {
            async Eff<int> Test(int x)
            {
                var y = await Nested(x).ConfigureAwait();
                return y + 1;

                async Eff<int> Nested(int x)
                {
                    await Task.Delay(50);
                    var y = await Task.FromResult(x + 1);
                    return y;
                }
            }

            Assert.Equal(3, await Test(1).Run(Handler));
        }

        [Fact]
        public async Task EffUntyped_AwaitSequenceOfTaskEffects()
        {
            int counter = 0;

            async Eff Test(int x)
            {
                await Nested(x).ConfigureAwait();
                counter++;

                async Eff Nested(int x)
                {
                    await Task.Delay(50);
                    var y = await Task.FromResult(x + 1);
                    counter += y;
                }
            }

            await Test(1).Run(Handler);
            Assert.Equal(3, counter);
        }

        [Fact]
        public async Task EffTyped_AwaitCombinationOfEffAndTaskEffects()
        {
            async Eff<int> Test(int x)
            {
                await Task.Delay(1000);
                var y = await Nested(x);
                return y + 1;

                async Eff<int> Nested(int x)
                {
                    var y = await Task.FromResult(x + 1);
                    return y;
                }
            }

            Assert.Equal(3, await Test(1).Run(Handler));
        }

        [Fact]
        public async Task EffUntyped_AwaitCombinationOfEffAndTaskEffects()
        {
            int counter = 0;

            async Eff Test(int x)
            {
                await Task.Delay(1000);
                await Nested(x);
                counter++;

                async Eff Nested(int x)
                {
                    var y = await Task.FromResult(x + 1);
                    counter += y;
                }
            }

            await Test(1).Run(Handler);
            Assert.Equal(3, counter);
        }

        [Fact]
        public async Task EffTyped_SimpleExceptionPropagation()
        {
            async Eff<int> Test(int x)
            {
                return 1 / x;
            }

            var eff = Test(0);
            await Assert.ThrowsAsync<DivideByZeroException>(() => eff.Run(Handler).AsTask());
        }

        [Fact]
        public async Task EffUntyped_SimpleExceptionPropagation()
        {
            async Eff Test(int x)
            {
                _ = 1 / x;
            }

            var eff = Test(0);
            await Assert.ThrowsAsync<DivideByZeroException>(() => eff.Run(Handler).AsTask());
        }

        [Fact]
        public async Task EffTyped_NestedExceptionPropagation()
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

            var eff = Test(0);
            await Assert.ThrowsAsync<DivideByZeroException>(() => eff.Run(Handler).AsTask());
        }

        [Fact]
        public async Task EffUntyped_NestedExceptionPropagation()
        {
            async Eff Test(int x)
            {
                await Nested(x);

                async Eff Nested(int x)
                {
                    _ = 1 / x;
                }
            }

            var eff = Test(0);
            await Assert.ThrowsAsync<DivideByZeroException>(() => eff.Run(Handler).AsTask());
        }

        public class AwaiterThatThrows<T> : EffAwaiter<T>
        {
            public override string Id => throw new NotImplementedException();

            public override ValueTask Accept(IEffectHandler handler)
            {
                SetException(new DivideByZeroException());
                throw new NotImplementedException();
            }
        }

        [Fact]
        public async Task AwaiterThatThrows_ShouldNotInterruptInterpretation()
        {
            bool isFinallyBlockExecuted = false;
            async Eff Test()
            {
                try
                {
                    await new AwaiterThatThrows<int>();
                }
                finally
                {
                    isFinallyBlockExecuted = true;
                }
            }

            await Assert.ThrowsAsync<NotImplementedException>(() => Test().Run(Handler).AsTask());
            Assert.True(isFinallyBlockExecuted);
        }

        [Fact]
        public async Task CompletedEff_ShouldComplete()
        {
            await Eff.CompletedEff.Run(Handler);
        }

        [Fact]
        public async Task FromResult_ShouldReturnResult()
        {
            var eff = Eff.FromResult(42);
            Assert.Equal(42, await eff.Run(Handler));
        }

        [Fact]
        public async Task FromFunc_Typed_HappyPath()
        {
            int counter = 0;
            var eff = Eff.FromFunc(async () => ++counter);
            Assert.Equal(0, counter);
            Assert.Equal(1, await eff.Run(Handler));
            Assert.Equal(1, counter);
        }

        [Fact]
        public async Task FromFunc_Typed_Exception()
        {
            var eff = Eff.FromFunc<int>(async () => throw new DivideByZeroException());
            await Assert.ThrowsAsync<DivideByZeroException>(() => eff.Run(Handler).AsTask());
        }

        [Fact]
        public async Task FromFunc_Untyped_HappyPath()
        {
            int counter = 0;
            var eff = Eff.FromFunc(async () => { counter++; });
            Assert.Equal(0, counter);
            await eff.Run(Handler);
            Assert.Equal(1, counter);
        }

        [Fact]
        public async Task FromFunc_Untyped_Exception()
        {
            var eff = Eff.FromFunc(async () => { throw new DivideByZeroException(); });
            await Assert.ThrowsAsync<DivideByZeroException>(() => eff.Run(Handler).AsTask());
        }
    }
}