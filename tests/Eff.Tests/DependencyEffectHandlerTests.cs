using Nessos.Effects.DependencyInjection;
using Nessos.Effects.Handlers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Nessos.Effects.Tests
{
    public class DependencyEffectHandlerTests : EffectHandlerTests
    {
        public const int IntDependency = 42;
        public const string StringDependency = "lorem ipsum";

        [Fact]
        public async Task DependencyEffect_IntDependency_HappyPath()
        {
            async Eff<int> Test()
            {
                return await IO<int>.Do(x => x + 1);
            }

            var result = await Test().Run(Handler);
            Assert.Equal(IntDependency + 1, result);
        }

        [Fact]
        public async Task DependencyEffect_IntDependency_Action_HappyPath()
        {
            int counter = 0;
            async Eff Test()
            {
                await IO<int>.Do(x => { counter += x; });
            }

            await Test().Run(Handler);
            Assert.Equal(IntDependency, counter);
        }

        [Fact]
        public async Task DependencyEffect_IntDependency_Async_HappyPath()
        {
            static async Eff<int> Test()
            {
                return await IO<int>.Do(async x => { await Task.Delay(1); return x + 1; });
            }

            var result = await Test().Run(Handler);
            Assert.Equal(IntDependency + 1, result);
        }

        [Fact]
        public async Task DependencyEffect_IntDependency_AsyncAction_HappyPath()
        {
            int counter = 0;
            async Eff Test()
            {
                await IO<int>.Do(async x => { await Task.Delay(1); counter += x; });
            }

            await Test().Run(Handler);
            Assert.Equal(IntDependency, counter);
        }

        [Fact]
        public async Task DependencyEffect_CombinedDependencies_HappyPath()
        {
            static async Eff<(int, string)> Test()
            {
                var intDep = await IO<int>.Do(x => x);
                var stringDep = await IO<string>.Do(x => x);
                return (intDep, stringDep);
            }

            var result = await Test().Run(Handler);
            Assert.Equal((IntDependency, StringDependency), result);
        }

        [Fact]
        public async Task DependencyEffect_ContextCallback_HappyPath()
        {
            static async Eff<(int, string)> Test()
            {
                return await IO.Do(ctx => (ctx.Resolve<int>(), ctx.Resolve<string>()));
            }

            var result = await Test().Run(Handler);
            Assert.Equal((IntDependency, StringDependency), result);
        }

        [Fact]
        public async Task DependencyEffect_Run_HappyPath()
        {
            var effect = IO.Do(ctx => (ctx.Resolve<int>(), ctx.Resolve<string>()));
            var result = await effect.Run(Handler);
            Assert.Equal((IntDependency, StringDependency), result);
        }

        [Fact]
        public async Task DependencyEffect_MissingDependency_ShouldThrowKeyNotFoundException()
        {
            async Eff Test()
            {
                await IO<UninhabitedType>.Do(d => d.Test());
            }

            await Assert.ThrowsAsync<KeyNotFoundException>(() => Test().Run(Handler).AsTask());
        }

        public class UninhabitedType
        {
            private UninhabitedType() { }

            public void Test() { }
        }

        protected override IEffectHandler Handler => new DependencyEffectHandler(
            new Container() { 
                IntDependency, 
                StringDependency
            });

        private class Container : IContainer, IEnumerable
        {
            private readonly Dictionary<Type, object?> _dict = new Dictionary<Type, object?>();

            public void Add<TDependency>(TDependency dependency) => _dict[typeof(TDependency)] = dependency;

            public TDependency Resolve<TDependency>() => (TDependency)_dict[typeof(TDependency)]!;

            public IEnumerator GetEnumerator() => _dict.Values.GetEnumerator();
        }
    }
}
