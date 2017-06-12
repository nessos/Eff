using Eff.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Eff.Examples.CancellationToken
{
    public class CancellationTokenEffect : Effect<System.Threading.CancellationToken>
    {

        public CancellationTokenEffect(string memberName, string sourceFilePath, int sourceLineNumber, bool captureState)
            : base(memberName, sourceFilePath, sourceLineNumber, captureState)
        {
        }
    }

}