using Nessos.Effects.DependencyInjection;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Nessos.Effects.Examples.DependencyInjection
{
    // Poor man's DI container
    public class Container : IContainer, IEnumerable
    {
        private Dictionary<Type, object> _dict = new Dictionary<Type, object>();

        public void Add<T>(T value) => _dict[typeof(T)] = value!;

        public T Resolve<T>() => (T)_dict[typeof(T)];
        public IEnumerator GetEnumerator() => _dict.Values.GetEnumerator();
    }
}
