namespace Nessos.EffPromo.Persistence.Model
{
	using System.Collections.Generic;

	using Nessos.EffPromo.Persistence.Base;

	public class Blog : Entity<long>
	{
		public string Name { get; set; } = string.Empty;

		public virtual List<Post> Posts { get; } = new List<Post>();
	}
}
