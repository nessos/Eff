﻿using Nessos.Effects.Handlers;
using System;
using System.Threading.Tasks;

namespace Nessos.Effects.Examples.Console
{
    /// <summary>
    ///   Handles console effects by calling the standard System.Console API
    /// </summary>
    public class ConsoleEffectHandler : EffectHandler
    {
        public override Task Handle<TResult>(EffectAwaiter<TResult> awaiter)
        {
            switch (awaiter)
            {
                case EffectAwaiter<Unit> { Effect: ConsolePrintEffect printEffect } awtr:
                    System.Console.Write(printEffect.Message);
                    awtr.SetResult(Unit.Value);
                    break;
                case EffectAwaiter<string> { Effect: ConsoleReadEffect _ } awtr:
                    string message = System.Console.ReadLine();
                    awtr.SetResult(message);
                    break;
                default:
                    throw new NotSupportedException(awaiter.Id);
            }

            return Task.CompletedTask;
        }
    }
}
