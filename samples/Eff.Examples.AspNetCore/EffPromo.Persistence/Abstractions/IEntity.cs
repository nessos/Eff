namespace Nessos.EffPromo.Persistence.Abstractions
{
	using System;

	public interface IEntity<T>
		where T : IComparable
	{
		T Id { get; set; }
	}
}
