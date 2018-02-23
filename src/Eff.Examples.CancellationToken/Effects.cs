﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Eff.Examples.CancellationToken
{

    public static class Effect
    {
        public static CancellationTokenEffect CancellationToken(
                                                    [CallerMemberName] string memberName = "",
                                                    [CallerFilePath] string sourceFilePath = "",
                                                    [CallerLineNumber] int sourceLineNumber = 0,
                                                    bool captureState = false)
        {
            return new CancellationTokenEffect(memberName, sourceFilePath, sourceLineNumber, captureState);
        }
    }
}
