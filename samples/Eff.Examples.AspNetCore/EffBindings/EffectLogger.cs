namespace Nessos.Effects.Examples.AspNetCore.EffBindings
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using Nessos.Effects.Handlers;
    using Newtonsoft.Json;

    /// <summary>
    ///   Stores effect trace information that can be re-run with a replay effect handler.
    /// </summary>
    public class EffectLogger
    {
        private ConcurrentDictionary<string, ImmutableArray<PersistedEffect>> _store = 
            new ConcurrentDictionary<string, ImmutableArray<PersistedEffect>>();

        /// <summary>
        ///   Commit an effect trace log, returning a unique identifier that can be used for future retrievals.
        /// </summary>
        public string Commit(IEnumerable<PersistedEffect> effectLogEntry)
        {
            var immutableLogEntry = effectLogEntry.ToImmutableArray();
            var logId = Guid.NewGuid().ToString("D");
            _store[logId] = immutableLogEntry;
            return logId;
        }

        /// <summary>
        ///   Look up effect log by given identifier.
        /// </summary>
        public bool TryGetLogById(string logId, out ImmutableArray<PersistedEffect> result)
        {
            return _store.TryGetValue(logId, out result);
        }
    }

    /// <summary>
    ///   A struct containing the serialized result of an effectful computation.
    /// </summary>
    public struct PersistedEffect
    {
        public bool IsException { get; set; }
        public string Value { get; set; }

        /// <summary>
        ///   Captures a completed Eff awaiter instance into a serialized PersistedEffect value.
        /// </summary>
        public static PersistedEffect FromCompletedAwaiter<TResult>(EffAwaiter<TResult> awaiter)
        {
            if (!awaiter.IsCompleted)
            {
                throw new ArgumentException("EffAwaiter is not in completed state.", nameof(awaiter));
            }

            bool isException;
            string value;

            if (awaiter.Exception is Exception e)
            {
                isException = true;
                value = JsonConvert.SerializeObject(e);
            }
            else
            {
                isException = false;
                value = JsonConvert.SerializeObject(awaiter.Result);
            }

            return new PersistedEffect() { IsException = isException, Value = value };
        }

        ///<summary>
        ///  Hydrates an incomplete awaiter instance with the persisted result.
        ///</summary>
        public void WriteToAwaiter<TResult>(EffAwaiter<TResult> awaiter)
        {
            if (awaiter.IsCompleted)
            {
                throw new ArgumentException("EffAwaiter is in completed state.", nameof(awaiter));
            }

            if (IsException)
            {
                var exn = JsonConvert.DeserializeObject<Exception>(Value);
                awaiter.SetException(exn!);
            }
            else
            {
                var value = JsonConvert.DeserializeObject<TResult>(Value);
                awaiter.SetResult(value!);
            }
        }
    }
}
