using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Nessos.Effects;

namespace Nessos.Effects.Examples.DependencyInjection
{
    public class DependencyEffect<T> : Effect<T>
    {

    }

    public static class Effect
    {
        public static DependencyEffect<T> GetDependency<T>() => new DependencyEffect<T>();
    }
}
