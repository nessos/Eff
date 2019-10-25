namespace Nessos.EffPromo.Api
{
	using System.IO;

	using EffImplementation;

	using Microsoft.AspNetCore.Builder;
	using Microsoft.AspNetCore.Hosting;
	using Microsoft.AspNetCore.Http;
	using Microsoft.AspNetCore.Mvc;
	using Microsoft.EntityFrameworkCore;
	using Microsoft.EntityFrameworkCore.Diagnostics;
	using Microsoft.Extensions.Configuration;
	using Microsoft.Extensions.DependencyInjection;
	using Microsoft.Extensions.Hosting;
	using Microsoft.Extensions.Logging;
	using Microsoft.OpenApi.Models;

	using Nessos.EffPromo.Persistence;

	using Serilog;
	using Serilog.Extensions.Logging;

	public class Startup
	{
		public Startup(IConfiguration conf, IWebHostEnvironment env)
		{
			Configuration = conf;
			Environment = env;
		}

		private IConfiguration Configuration { get; }

		private IWebHostEnvironment Environment { get; }

		public void ConfigureServices(IServiceCollection services)
		{
			services.Configure<CookiePolicyOptions>(options =>
			{
				options.Secure = CookieSecurePolicy.SameAsRequest;
				options.CheckConsentNeeded = context => true;
			});

			var dbLocation = Configuration.GetConnectionString("EffPromoSample")
			                 ??
			                 $"Data Source={Path.Combine(Directory.GetCurrentDirectory(), "bin", "EffPromoSample.db")}";

			services.AddDbContext<EffDbContext>(options => options
				.UseSqlite(
					dbLocation,
					opt => opt.UseRelationalNulls().MigrationsAssembly(typeof(EffDbContext).Assembly.FullName))
				.EnableSensitiveDataLogging(Environment.IsDevelopment())
				.EnableDetailedErrors(Environment.IsDevelopment())
				.ConfigureWarnings(warnings => warnings
					.Ignore(CoreEventId.SensitiveDataLoggingEnabledWarning)
					.Log(
						(RelationalEventId.QueryPossibleExceptionWithAggregateOperatorWarning, LogLevel.Warning),
						(RelationalEventId.QueryPossibleUnintendedUseOfEqualsWarning, LogLevel.Warning),
						(RelationalEventId.CommandExecuting, LogLevel.Information)))
				.UseLoggerFactory(new SerilogLoggerFactory(Log.Logger)));

			services.AddControllers()
				.AddControllersAsServices()
				.SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

			services.AddRazorPages()
				.SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

			services.AddSwaggerGen(c => c.SwaggerDoc("v1", new OpenApiInfo { Title = "Eff Sample", Version = "v1" }));

			services.AddScoped(provider => new EffectContext(provider));
		}

		public void Configure(IApplicationBuilder app)
		{
			if (Environment.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseExceptionHandler("/Error");
				app.UseHsts();
			}

			app.UseHttpsRedirection();
			app.UseCookiePolicy();

			app.UseStaticFiles();
			app.UseSerilogRequestLogging();
			app.UseRouting();

			app.UseAuthorization();
			app.UseSwagger();
			app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Eff Sample v1"));
			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
				endpoints.MapRazorPages();
			});
		}
	}
}
