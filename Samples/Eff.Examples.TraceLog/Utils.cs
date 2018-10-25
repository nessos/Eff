using Eff.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eff.Examples.TraceLog
{

    public class ResultLog
    {
        public string CallerMemberName { get; set; }
        public string CallerFilePath { get; set; }
        public int CallerLineNumber { get; set; }
        public object Result { get; set; }
        public (string name, object value)[] Parameters { get; set; }
        public (string name, object value)[] LocalVariables { get; set; }
    }
    static class Utils
    {
        public static string Dump(this IList<ResultLog> traceLogs)
        {
            var builder = new StringBuilder();
            foreach (var item in traceLogs)
            {
                builder.AppendLine($"at {item.CallerMemberName} in {Path.GetFileName(item.CallerFilePath)}: line {item.CallerLineNumber}");
                builder.AppendLine($"result: {item.Result}");
                builder.Append("paremeters:");
                foreach (var (name, value) in item.Parameters)
                {
                    builder.Append($" ({name}, {value}) ");
                }
                builder.AppendLine();

                builder.Append("locals:");
                foreach (var (name, value) in item.LocalVariables)
                {
                    builder.Append($" ({name}, {value}) ");
                }
                builder.AppendLine();
            }

            return builder.ToString();
        }
    }
}
