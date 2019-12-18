using System;

namespace Nessos.Eff.Tests
{
    public class ExceptionLog
    {
        public string CallerMemberName { get; set; }
        public string CallerFilePath { get; set; }
        public int CallerLineNumber { get; set; }
        public Exception Exception { get; set; }
        public (string name, object value)[] Parameters { get; set; }
        public (string name, object value)[] LocalVariables { get; set; }
    }

    public class ResultLog
    {
        public string CallerMemberName { get; set; }
        public string CallerFilePath { get; set; }
        public int CallerLineNumber { get; set; }
        public object Result { get; set; }
        public (string name, object value)[] Parameters { get; set; }
        public (string name, object value)[] LocalVariables { get; set; }
    }
}
