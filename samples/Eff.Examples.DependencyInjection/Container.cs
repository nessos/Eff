using System;
using System.Collections.Generic;
using System.Text;

namespace Nessos.Eff.Examples.DependencyInjection
{
    // Poor man's DI container
    public class Container
    {
        private Dictionary<Type, object> _index = new Dictionary<Type, object>();

        public void Add<T>(T value) => _index[typeof(T)] = value;
        public T Get<T>() => (T)_index[typeof(T)];
    }
}
