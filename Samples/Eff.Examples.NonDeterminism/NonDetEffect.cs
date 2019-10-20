using Eff.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Eff.Examples.NonDeterminism
{
    public class NonDetEffect<T> : Effect<T>
    {

        public NonDetEffect(T[] choices, 
                            string memberName, string sourceFilePath, int sourceLineNumber, bool captureState)
            : base(memberName, sourceFilePath, sourceLineNumber)
        {
            Choices = choices;
        }

        public T[] Choices { get; }

    }

}