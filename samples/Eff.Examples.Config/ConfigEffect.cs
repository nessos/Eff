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
        public ConfigEffect(string key)
        {
            this.Key = key;
        }

        public string Key { get; }
    }
}