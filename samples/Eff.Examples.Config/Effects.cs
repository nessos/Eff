using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Eff.Examples.Config
{

    public static class Effect
    {
        public static ConfigEffect Config(string key,
                                            [CallerMemberName] string memberName = "",
                                            [CallerFilePath] string sourceFilePath = "",
                                            [CallerLineNumber] int sourceLineNumber = 0,
                                            bool captureState = false)
        {
            return new ConfigEffect(key, memberName, sourceFilePath, sourceLineNumber, captureState);
        }
    }
}
