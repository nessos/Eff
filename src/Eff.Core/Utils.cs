using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Eff.Core
{
    public static class Utils
    {
        private static ConcurrentDictionary<Type, (string name, FieldInfo fieldInfo)[]> parametersInfoCache = new ConcurrentDictionary<Type, (string name, FieldInfo fieldInfo)[]>();
        private static ConcurrentDictionary<Type, (string name, FieldInfo fieldInfo)[]> localVariablesInfoCache = new ConcurrentDictionary<Type, (string name, FieldInfo fieldInfo)[]>();
        private static (string name, object value)[] GetValues((string name, FieldInfo fieldInfo)[] fieldsInfo, object state)
        {
            var result = new(string name, object value)[fieldsInfo.Length];
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

        public static (string name, object value)[] GetParametersValues(object state)
        {
            var parametersInfo = parametersInfoCache.GetOrAdd(state.GetType(), _ => Utils.GetParametersInfo(state));

            return GetValues(parametersInfo, state);
        }

        public static (string name, object value)[] GetLocalVariablesValues(object state)
        {
            var localVariablesInfo = localVariablesInfoCache.GetOrAdd(state.GetType(), _ => Utils.GetLocalVariablesInfo(state));

            return GetValues(localVariablesInfo, state); ;
        }

        public static string GetMethodName(object state)
        {
            if (state == null)
                throw new ArgumentNullException(nameof(state));

            var name = state.GetType().Name;
            if (name.StartsWith("<"))
                return name.Substring(1, name.LastIndexOf(">") - 1);
            else
                return name;
        }
    }
}
