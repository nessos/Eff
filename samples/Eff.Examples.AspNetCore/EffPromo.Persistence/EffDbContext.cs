namespace Nessos.EffPromo.Persistence
{
	using System;

	using Microsoft.EntityFrameworkCore;

	using Nessos.EffPromo.Persistence.Joins;
	using Nessos.EffPromo.Persistence.Model;

	public class EffDbContext : DbContext
	{
		public EffDbContext(DbContextOptions<EffDbContext> options)
			: base(options)
		{
		}

		public DbSet<Blog> Blogs { get; set; }

		public DbSet<Author> Authors { get; set; }

		public DbSet<Post> Posts { get; set; }

		public DbSet<AuthorPosts> AuthorPosts { get; set; }

		protected override void OnModelCreating(ModelBuilder builder)
		{
			if (builder == null)
			{
				throw new ArgumentNullException(nameof(builder));
			}

			base.OnModelCreating(builder);

			builder.Entity<AuthorPosts>(e =>
			{
				e.HasKey("PostId", "AuthorId");

				e.HasOne(x => x.Post)
					.WithMany(x => x.AuthorPosts)
					.HasForeignKey("PostId");

				e.HasOne(x => x.Author)
					.WithMany(x => x.AuthorPosts)
					.HasForeignKey("AuthorId");
			});
		}
	}
}
