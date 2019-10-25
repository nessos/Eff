namespace Nessos.EffPromo.Persistence.Base
{
	using System;

	using Nessos.EffPromo.Persistence.Abstractions;

	public class Entity<T> : IEntity<T>
		where T : IComparable
	{
		public T Id { get; set; }
	}
}
