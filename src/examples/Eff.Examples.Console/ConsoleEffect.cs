using Eff.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Runtime.CompilerServices;

namespace Eff.Examples.Console
{
    public class ConsolePrintEffect : Effect<ValueTuple>
    {

        private readonly string message;
        public ConsolePrintEffect(string message, 
                            string memberName, string sourceFilePath, int sourceLineNumber, bool captureState)
            : base(memberName, sourceFilePath, sourceLineNumber)
        {
            this.message = message;
        }

        public string Message => message;

    }

    public class ConsoleReadEffect : Effect<string>
    {
        public ConsoleReadEffect(string memberName, string sourceFilePath, int sourceLineNumber, bool captureState)
            : base(memberName, sourceFilePath, sourceLineNumber)
        {
        }
    }

    public static class Effect
    {
        public static ConsolePrintEffect Print(string message,
                                            [CallerMemberName] string memberName = "",
                                            [CallerFilePath] string sourceFilePath = "",
                                            [CallerLineNumber] int sourceLineNumber = 0,
                                            bool captureState = false)
        {
            return new ConsolePrintEffect(message, memberName, sourceFilePath, sourceLineNumber, captureState);
        }

        public static ConsoleReadEffect Read([CallerMemberName] string memberName = "",
                                             [CallerFilePath] string sourceFilePath = "",
                                             [CallerLineNumber] int sourceLineNumber = 0,
                                             bool captureState = false)
        {
            return new ConsoleReadEffect(memberName, sourceFilePath, sourceLineNumber, captureState);
        }
    }

}