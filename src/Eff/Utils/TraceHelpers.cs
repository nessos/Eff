namespace Nessos.Effects.Utils;

using System.Collections.Concurrent;
using System.Reflection;
using System.Runtime.CompilerServices;

/// <summary>
///   Helper methods for extracting state machine environment metadata.
/// </summary>
public static class TraceHelpers
{
    private static readonly ConcurrentDictionary<Type, (string name, FieldInfo fieldInfo)[]> s_parametersInfoCache = new();
    private static readonly ConcurrentDictionary<Type, (string name, FieldInfo fieldInfo)[]> s_localVariablesInfoCache = new();
    private static (string name, object? value)[] GetValues((string name, FieldInfo fieldInfo)[] fieldsInfo, object state)
    {
        (string name, object? value)[] result = new (string name, object? value)[fieldsInfo.Length];
        for (int j = 0; j < result.Length; j++)
        {
            result[j] = (fieldsInfo[j].name, fieldsInfo[j].fieldInfo.GetValue(state));
        }
        return result;
    }

    private static (string name, FieldInfo fieldInfo)[] GetParametersInfo(object state)
    {
        FieldInfo[] fieldInfos = state.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        return fieldInfos
            .Where(fieldInfo => !fieldInfo.Name.StartsWith("<", StringComparison.Ordinal))
            .Select(fieldInfo => (fieldInfo.Name, fieldInfo))
            .ToArray();
    }

    private static (string name, FieldInfo fieldInfo)[] GetLocalVariablesInfo(object state)
    {
        FieldInfo[] fieldInfos = state.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        return fieldInfos
            .Where(fieldInfo => !fieldInfo.Name.StartsWith("<>", StringComparison.Ordinal))
            .Where(fieldInfo => fieldInfo.Name.StartsWith("<", StringComparison.Ordinal))
            .Select(fieldInfo => (fieldInfo.Name[1..fieldInfo.Name.LastIndexOf('>')], fieldInfo))
            .ToArray();
    }

    /// <summary>
    ///   Uses reflection to extract all parameter values from an async state machine.
    /// </summary>
    /// <param name="stateMachine">The state machine to extract metadata from.</param>
    /// <returns>Key/value pairs containing all state machine parameter variables.</returns>
    /// <exception cref="ArgumentNullException" />
    public static (string name, object? value)[] GetParameterValues(this IAsyncStateMachine stateMachine)
    {
        if (stateMachine is null)
        {
            throw new ArgumentNullException(nameof(stateMachine));
        }

        (string name, FieldInfo fieldInfo)[] parametersInfo = s_parametersInfoCache.GetOrAdd(stateMachine.GetType(), _ => GetParametersInfo(stateMachine));

        return GetValues(parametersInfo, stateMachine);
    }

    /// <summary>
    ///   Uses reflection to extract all local variables values from an async state machine.
    /// </summary>
    /// <param name="stateMachine">The state machine to extract metadata from.</param>
    /// <returns>Key/value pairs containing all state machine local variables.</returns>
    /// <exception cref="ArgumentNullException" />
    public static (string name, object? value)[] GetLocalVariableValues(this IAsyncStateMachine stateMachine)
    {
        if (stateMachine is null)
        {
            throw new ArgumentNullException(nameof(stateMachine));
        }

        (string name, FieldInfo fieldInfo)[] localVariablesInfo = s_localVariablesInfoCache.GetOrAdd(stateMachine.GetType(), _ => GetLocalVariablesInfo(stateMachine));

        return GetValues(localVariablesInfo, stateMachine);
    }

    /// <summary>
    ///   Uses reflection to extract the method name from an async state machine.
    /// </summary>
    /// <param name="stateMachine">The state machine to extract metadata from.</param>
    /// <returns>The async method name.</returns>
    /// <exception cref="ArgumentNullException" />
    public static string GetMethodName(this IAsyncStateMachine stateMachine)
    {
        if (stateMachine is null)
        {
            throw new ArgumentNullException(nameof(stateMachine));
        }

        string name = stateMachine.GetType().Name;
        return name.StartsWith("<", StringComparison.Ordinal)
            ? name[1..name.LastIndexOf('>')]
            : name;
    }
}
