#pragma warning disable 1998

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Eff.Core
{
    public static class EffExecutor
    {

        private static (string name, object value)[] GetParameters((string name, FieldInfo fieldInfo)[] parametersInfo, IAsyncStateMachine _stateMachine)
        {
            return parametersInfo.Select(parameter => (parameter.name, parameter.fieldInfo.GetValue(_stateMachine)))
                                 .ToArray();
        }

        private static (string name, object value)[] GetLocalVariables((string name, FieldInfo fieldInfo)[] localVariablesInfo, IAsyncStateMachine _stateMachine)
        {
            return localVariablesInfo.Select(local => (local.name, local.fieldInfo.GetValue(_stateMachine)))
                                     .ToArray();
        }

        private static (string name, FieldInfo fieldInfo)[] GetParametersInfo(IAsyncStateMachine stateMachine)
        {
            var fieldInfos = stateMachine.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            var parametersInfo = fieldInfos
                                       .Where(fieldInfo => !fieldInfo.Name.StartsWith("<"))
                                       .Select(fieldInfo => (fieldInfo.Name, fieldInfo))
                                       .ToArray();
            return parametersInfo;
        }

        private static (string name, FieldInfo fieldInfo)[] GetLocalVariablesInfo(IAsyncStateMachine stateMachine)
        {
            var fieldInfos = stateMachine.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            var localVariablesInfo = fieldInfos
                                           .Where(fieldInfo => !fieldInfo.Name.StartsWith("<>"))
                                           .Where(fieldInfo => fieldInfo.Name.StartsWith("<"))
                                           .Select(fieldInfo => (fieldInfo.Name.Substring(1, fieldInfo.Name.LastIndexOf(">") - 1), fieldInfo))
                                           .ToArray();
            return localVariablesInfo;
        }

        public static async Task<TResult> Run<TResult>(this Eff<TResult> eff, IEffectHandler handler)
        {
            if (eff == null)
                throw new ArgumentNullException(nameof(eff));
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            var parametersInfo = default((string name, FieldInfo fieldInfo)[]);
            var localVariablesInfo = default((string name, FieldInfo fieldInfo)[]);
            var result = default(TResult);
            var done = false;
            while (!done)
            {
                switch (eff)
                {
                    case SetException<TResult> setException:
                        System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(setException.Exception).Throw();
                        break;
                    case SetResult<TResult> setResult:
                        result = setResult.Result;
                        done = true;
                        break;
                    case Delay<TResult> delay:
                        eff = delay.Func();
                        break;
                    case Await<TResult> awaitEff:
                        var effect = awaitEff.Effect;

                        // Initialize State Info
                        if ((effect.CaptureState || handler.EnableParametersLogging) && parametersInfo == null)
                            parametersInfo = GetParametersInfo(awaitEff.StateMachine);
                        if ((effect.CaptureState || handler.EnableLocalVariablesLogging) && localVariablesInfo == null)
                            localVariablesInfo = GetLocalVariablesInfo(awaitEff.StateMachine);

                        // Initialize State Values
                        var parameters = default((string name, object value)[]);
                        var localVariables = default((string name, object value)[]);
                        if (effect.CaptureState)
                        {
                            parameters = GetParameters(parametersInfo, awaitEff.StateMachine);
                            localVariables = GetLocalVariables(localVariablesInfo, awaitEff.StateMachine);
                            effect.SetState(parameters, localVariables);
                        }
                        else
                        {
                            if (handler.EnableParametersLogging)
                                parameters = GetParameters(parametersInfo, awaitEff.StateMachine);
                            if (handler.EnableLocalVariablesLogging)
                                localVariables = GetLocalVariables(localVariablesInfo, awaitEff.StateMachine);
                        }

                        // Execute Effect
                        try
                        {
                            await effect.Accept(handler);
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
                            if (handler.EnableExceptionLogging && effect.Exception != null)
                            {
                                await handler.Log(new ExceptionLog
                                {
                                    CallerFilePath = effect.CallerFilePath,
                                    CallerLineNumber = effect.CallerLineNumber,
                                    CallerMemberName = effect.CallerMemberName,
                                    Exception = effect.Exception,
                                    Parameters = parameters,
                                    LocalVariables = localVariables,
                                });
                            }
                            if (handler.EnableTraceLogging && effect.HasResult)
                            {
                                await handler.Log(new ResultLog
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
                        break;
                    default:
                        throw new NotSupportedException($"{eff.GetType().Name}");
                }
            }

            return result;
        }
    }


}
