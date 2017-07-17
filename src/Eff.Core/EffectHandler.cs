#pragma warning disable 1998

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Eff.Core
{
    public abstract class EffectHandler : IEffectHandler
    {

        public bool EnableExceptionLogging { get; }
        public bool EnableTraceLogging { get; }
        bool EnableParametersLogging { get; }
        bool EnableLocalVariablesLogging { get; }

        public EffectHandler(bool enableExceptionLogging = false, 
                             bool enableTraceLogging = false,
                             bool enableParametersLogging = false,
                             bool enableLocalVariablesLogging = false)
        {
            EnableExceptionLogging = enableExceptionLogging;
            EnableTraceLogging = enableTraceLogging;
            EnableParametersLogging = enableParametersLogging;
            EnableLocalVariablesLogging = enableLocalVariablesLogging;
        }

        public abstract ValueTask<ValueTuple> Handle<TResult>(IEffect<TResult> effect);
        public abstract ValueTask<ValueTuple> Log(ExceptionLog log);
        public abstract ValueTask<ValueTuple> Log(ResultLog log);

        protected (string name, object value)[] GetValues((string name, FieldInfo fieldInfo)[] fieldsInfo, IAsyncStateMachine _stateMachine)
        {
            var result = new(string name, object value)[fieldsInfo.Length];
            for (int j = 0; j < result.Length; j++)
            {
                result[j] = (fieldsInfo[j].name, fieldsInfo[j].fieldInfo.GetValue(_stateMachine));
            }
            return result;
        }

        protected (string name, FieldInfo fieldInfo)[] GetParametersInfo(IAsyncStateMachine stateMachine)
        {
            var fieldInfos = stateMachine.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            var parametersInfo = fieldInfos
                                       .Where(fieldInfo => !fieldInfo.Name.StartsWith("<"))
                                       .Select(fieldInfo => (fieldInfo.Name, fieldInfo))
                                       .ToArray();
            return parametersInfo;
        }

        protected (string name, FieldInfo fieldInfo)[] GetLocalVariablesInfo(IAsyncStateMachine stateMachine)
        {
            var fieldInfos = stateMachine.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            var localVariablesInfo = fieldInfos
                                           .Where(fieldInfo => !fieldInfo.Name.StartsWith("<>"))
                                           .Where(fieldInfo => fieldInfo.Name.StartsWith("<"))
                                           .Select(fieldInfo => (fieldInfo.Name.Substring(1, fieldInfo.Name.LastIndexOf(">") - 1), fieldInfo))
                                           .ToArray();
            return localVariablesInfo;
        }

        private static ConcurrentDictionary<Type, (string name, FieldInfo fieldInfo)[]> parametersInfoCache = new ConcurrentDictionary<Type, (string name, FieldInfo fieldInfo)[]>();
        private static ConcurrentDictionary<Type, (string name, FieldInfo fieldInfo)[]> localVariablesInfoCache = new ConcurrentDictionary<Type, (string name, FieldInfo fieldInfo)[]>();


        public virtual async ValueTask<ValueTuple> Handle<TResult>(TaskEffect<TResult> effect)
        {
            var result = await effect.Task;
            effect.SetResult(result);
            
            return ValueTuple.Create();
        }

        public virtual async ValueTask<ValueTuple> Handle(TaskEffect effect)
        {
            await effect.Task;
            effect.SetResult(ValueTuple.Create());

            return ValueTuple.Create();
        }



        public virtual async ValueTask<ValueTuple> Handle<TResult>(FuncEffect<TResult> effect)
        {
            var result = effect.Func();
            effect.SetResult(result);

            return ValueTuple.Create();
        }

        public virtual async ValueTask<ValueTuple> Handle(ActionEffect effect)
        {
            effect.Action();
            effect.SetResult(ValueTuple.Create());

            return ValueTuple.Create();
        }

        public virtual async ValueTask<ValueTuple> Handle<TResult>(EffEffect<TResult> effect)
        {
            var result = await effect.Eff.Run(this);
            effect.SetResult(result);

            return ValueTuple.Create();
        }

        public async ValueTask<TResult> Handle<TResult>(SetResult<TResult> setResult)
        {
            return setResult.Result;
        }

        public async ValueTask<ValueTuple> Handle<TResult>(SetException<TResult> setException)
        {
            System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(setException.Exception).Throw();

            return ValueTuple.Create();
        }

        public async ValueTask<Eff<TResult>> Handle<TResult>(Delay<TResult> delay)
        {
            return delay.Func();
        }


        public async ValueTask<Eff<TResult>> Handle<TResult>(Await<TResult> awaitEff)
        {
            var eff = default(Eff<TResult>);
            var effect = awaitEff.Effect;

            var parametersInfo = default((string name, FieldInfo fieldInfo)[]);
            var localVariablesInfo = default((string name, FieldInfo fieldInfo)[]);
            // Initialize State Info
            if ((effect.CaptureState || EnableParametersLogging) && parametersInfo == null)
                parametersInfo = parametersInfoCache.GetOrAdd(awaitEff.StateMachine.GetType(), _ => GetParametersInfo(awaitEff.StateMachine));
            if ((effect.CaptureState || EnableLocalVariablesLogging) && localVariablesInfo == null)
                localVariablesInfo = localVariablesInfoCache.GetOrAdd(awaitEff.StateMachine.GetType(), _ => GetLocalVariablesInfo(awaitEff.StateMachine));

            // Initialize State Values
            var parameters = default((string name, object value)[]);
            var localVariables = default((string name, object value)[]);
            if (effect.CaptureState)
            {
                parameters = GetValues(parametersInfo, awaitEff.StateMachine);
                localVariables = GetValues(localVariablesInfo, awaitEff.StateMachine);
                effect.SetState(parameters, localVariables);
            }
            else
            {
                if (EnableParametersLogging)
                    parameters = GetValues(parametersInfo, awaitEff.StateMachine);
                if (EnableLocalVariablesLogging)
                    localVariables = GetValues(localVariablesInfo, awaitEff.StateMachine);
            }

            // Execute Effect
            try
            {
                await effect.Accept(this);
                if (!effect.IsCompleted)
                    throw new EffException($"Effect {effect.GetType().Name} is not completed.");
            }
            catch (AggregateException ex)
            {
                effect.SetException(ex.InnerException);
            }
            catch (Exception ex)
            {
                effect.SetException(ex);
            }
            finally
            {
                if (EnableExceptionLogging && effect.Exception != null)
                {
                    await Log(new ExceptionLog
                    {
                        CallerFilePath = effect.CallerFilePath,
                        CallerLineNumber = effect.CallerLineNumber,
                        CallerMemberName = effect.CallerMemberName,
                        Exception = effect.Exception,
                        Parameters = parameters,
                        LocalVariables = localVariables,
                    });
                }
                if (EnableTraceLogging && effect.HasResult)
                {
                    await Log(new ResultLog
                    {
                        CallerFilePath = effect.CallerFilePath,
                        CallerLineNumber = effect.CallerLineNumber,
                        CallerMemberName = effect.CallerMemberName,
                        Result = effect.Result,
                        Parameters = parameters,
                        LocalVariables = localVariables,
                    });
                }

                eff = awaitEff.Continuation();
            }
            return eff;
        }
    }


}
