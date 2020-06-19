using Nessos.Effects.Builders;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;

namespace Nessos.Effects.Utils
{
    public static class TraceHelpers
    {
        private static readonly ConcurrentDictionary<Type, (string name, FieldInfo fieldInfo)[]> s_parametersInfoCache = new ConcurrentDictionary<Type, (string name, FieldInfo fieldInfo)[]>();
        private static readonly ConcurrentDictionary<Type, (string name, FieldInfo fieldInfo)[]> s_localVariablesInfoCache = new ConcurrentDictionary<Type, (string name, FieldInfo fieldInfo)[]>();
        private static (string name, object? value)[] GetValues((string name, FieldInfo fieldInfo)[] fieldsInfo, object state)
        {
            var result = new(string name, object? value)[fieldsInfo.Length];
            for (int j = 0; j < result.Length; j++)
            {
                result[j] = (fieldsInfo[j].name, fieldsInfo[j].fieldInfo.GetValue(state));
            }
            return result;
        }

        private static (string name, FieldInfo fieldInfo)[] GetParametersInfo(object state)
        {
            var fieldInfos = state.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            var parametersInfo = fieldInfos
                                       .Where(fieldInfo => !fieldInfo.Name.StartsWith("<"))
                                       .Select(fieldInfo => (fieldInfo.Name, fieldInfo))
                                       .ToArray();
            return parametersInfo;
        }
        private static (string name, FieldInfo fieldInfo)[] GetLocalVariablesInfo(object state)
        {
            var fieldInfos = state.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            var localVariablesInfo = fieldInfos
                                           .Where(fieldInfo => !fieldInfo.Name.StartsWith("<>"))
                                           .Where(fieldInfo => fieldInfo.Name.StartsWith("<"))
                                           .Select(fieldInfo => (fieldInfo.Name.Substring(1, fieldInfo.Name.LastIndexOf(">") - 1), fieldInfo))
                                           .ToArray();
            return localVariablesInfo;
        }

        public static (string name, object? value)[] GetParametersValues(EffStateMachine stateMachine)
        {
            if (stateMachine is null)
            {
                throw new ArgumentNullException(nameof(stateMachine));
            }

            var state = stateMachine.GetState();

            var parametersInfo = s_parametersInfoCache.GetOrAdd(state.GetType(), _ => TraceHelpers.GetParametersInfo(state));

            return GetValues(parametersInfo, state);
        }

        public static (string name, object? value)[] GetLocalVariablesValues(EffStateMachine stateMachine)
        {
            if (stateMachine is null)
            {
                throw new ArgumentNullException(nameof(stateMachine));
            }

            var state = stateMachine.GetState();

            var localVariablesInfo = s_localVariablesInfoCache.GetOrAdd(state.GetType(), _ => TraceHelpers.GetLocalVariablesInfo(state));

            return GetValues(localVariablesInfo, state);
        }

        public static string GetMethodName(EffStateMachine stateMachine)
        {
            if (stateMachine is null)
            {
                throw new ArgumentNullException(nameof(stateMachine));
            }

            var state = stateMachine.GetState();

            var name = state.GetType().Name;
            if (name.StartsWith("<"))
                return name.Substring(1, name.LastIndexOf(">") - 1);
            else
                return name;
        }
    }
}
