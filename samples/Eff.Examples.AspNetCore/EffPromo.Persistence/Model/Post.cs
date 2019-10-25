namespace Nessos.EffPromo.Persistence.Model
{
	using System;
	using System.Collections.Generic;

	using Nessos.EffPromo.Persistence.Base;
	using Nessos.EffPromo.Persistence.Joins;

	public class Post : Entity<long>
	{
		public string Title { get; set; } = string.Empty;

		public string Content { get; set; } = string.Empty;

		public DateTimeOffset PublishedAt { get; set; }

		public virtual Blog Blog { get; set; }

		public virtual List<AuthorPosts> AuthorPosts { get; } = new List<AuthorPosts>();
	}
}
