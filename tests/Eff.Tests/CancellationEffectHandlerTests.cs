using Nessos.Effects.Cancellation;
using Nessos.Effects.Handlers;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Nessos.Effects.Tests
{
    public class CancellationEffectHandlerTests : EffectHandlerTests
    {
        protected override IEffectHandler Handler => new CancellationEffectHandler(CancellationToken.None);

        [Fact]
        public async Task Stub_CanceledToken_ShouldThrowOperationCanceledException()
        {
            async Eff<int> Test() => 42;

            var handler = new CancellationEffectHandler(new CancellationToken(canceled: true));
            await Assert.ThrowsAsync<OperationCanceledException>(() => Test().Run(handler).AsTask());
        }

        [Fact]
        public async Task DivergingWorkflow_CanceledToken_ShouldThrowOperationCanceledException()
        {
            async Eff Test()
            {
                while (await ShouldContinue())
                {
                    await Task.Delay(millisecondsDelay: 5);

                }

                async Eff<bool> ShouldContinue() => true;
            }

            using var cts = new CancellationTokenSource(1_000);
            var handler = new CancellationEffectHandler(cts.Token);
            await Assert.ThrowsAsync<OperationCanceledException>(() => Test().Run(handler).AsTask());
        }

        [Fact]
        public async Task CancellationTokenEffect_PassedToTask_ShouldThrowTaskCanceledException()
        {
            async Eff Test()
            {
                var token = await CancellationTokenEffect.Value;
                await Task.Delay(60_000, token);
            }

            using var cts = new CancellationTokenSource(1_000);
            var handler = new CancellationEffectHandler(cts.Token);
            await Assert.ThrowsAsync<TaskCanceledException>(() => Test().Run(handler).AsTask());
        }

        [Fact]
        public async Task FinallyBlock_OnCancellation_ShouldBeRun()
        {
            bool isFinallyBlockExecuted = false;

            async Eff Test()
            {
                try
                {
                    while (await ShouldContinue())
                    {
                        await Task.Delay(millisecondsDelay: 5);
                    }

                    async Eff<bool> ShouldContinue() => true;
                }
                finally
                {
                    isFinallyBlockExecuted = true;
                }
            }

            using var cts = new CancellationTokenSource(1_000);
            var handler = new CancellationEffectHandler(cts.Token);
            await Assert.ThrowsAsync<OperationCanceledException>(() => Test().Run(handler).AsTask());
            Assert.True(isFinallyBlockExecuted);
        }
    }
}
