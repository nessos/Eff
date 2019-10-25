namespace Nessos.EffPromo.Persistence.Model
{
	using System;
	using System.Collections.Generic;

	using Nessos.EffPromo.Persistence.Base;
	using Nessos.EffPromo.Persistence.Joins;

	public class Author : Entity<Guid>
	{
		public string FirstName { get; set; } = string.Empty;

		public string LastName { get; set; } = string.Empty;

		public string Email { get; set; } = string.Empty;

		public virtual List<AuthorPosts> AuthorPosts { get; } = new List<AuthorPosts>();
	}
}
