using Eff.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eff.Tests
{
    static class Utils
    {
        public static string StackTraceLog(this Exception ex)
        {
            if (ex.Data["StackTraceLog"] is Queue<ExceptionLog> queue)
            {
                var builder = new StringBuilder();
                foreach (var item in queue)
                {
                    builder.AppendLine($"at {item.CallerMemberName} in {Path.GetFileName(item.CallerFilePath)}: line {item.CallerLineNumber}");
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
            else return string.Empty;
        }
    }
}
