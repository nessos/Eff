﻿using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

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

        /// <summary>
        ///   Uses reflection to extract all parameter values from an async state machine.
        /// </summary>
        /// <param name="stateMachine">The state machine to extract metadata from.</param>
        /// <returns>Key/value pairs containing all state machine parameter variables.</returns>
        public static (string name, object? value)[] GetParameterValues(this IAsyncStateMachine stateMachine)
        {
            if (stateMachine is null)
            {
                throw new ArgumentNullException(nameof(stateMachine));
            }

            var parametersInfo = s_parametersInfoCache.GetOrAdd(stateMachine.GetType(), _ => GetParametersInfo(stateMachine));

            return GetValues(parametersInfo, stateMachine);
        }

        /// <summary>
        ///   Uses reflection to extract all local variables values from an async state machine.
        /// </summary>
        /// <param name="stateMachine">The state machine to extract metadata from.</param>
        /// <returns>Key/value pairs containing all state machine local variables.</returns>
        public static (string name, object? value)[] GetLocalVariableValues(this IAsyncStateMachine stateMachine)
        {
            if (stateMachine is null)
            {
                throw new ArgumentNullException(nameof(stateMachine));
            }

            var localVariablesInfo = s_localVariablesInfoCache.GetOrAdd(stateMachine.GetType(), _ => GetLocalVariablesInfo(stateMachine));

            return GetValues(localVariablesInfo, stateMachine);
        }

        /// <summary>
        ///   Uses reflection to extract the method name from an async state machine.
        /// </summary>
        /// <param name="stateMachine">The state machine to extract metadata from.</param>
        /// <returns>The async method name.</returns>
        public static string GetMethodName(this IAsyncStateMachine stateMachine)
        {
            if (stateMachine is null)
            {
                throw new ArgumentNullException(nameof(stateMachine));
            }

            var name = stateMachine.GetType().Name;
            if (name.StartsWith("<"))
                return name.Substring(1, name.LastIndexOf(">") - 1);
            else
                return name;
        }
    }
}
