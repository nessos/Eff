using Nessos.Eff;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Eff.Examples.Config
{
    public class ConfigEffect : Effect<string>
    {

        private readonly string key;
        public ConfigEffect(string key, 
                            string memberName, string sourceFilePath, int sourceLineNumber, bool captureState)
            : base(memberName, sourceFilePath, sourceLineNumber)
        {
            this.key = key;
        }

        public string Key => key;

    }

}