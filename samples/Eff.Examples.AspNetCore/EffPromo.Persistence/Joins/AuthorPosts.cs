namespace Nessos.EffPromo.Persistence.Joins
{
	using Nessos.EffPromo.Persistence.Model;

	public class AuthorPosts
	{
		public Author Author { get; set; }

		public Post Post { get; set; }
	}
}
