namespace Nessos.EffPromo.Api
{
	using System;
	using System.IO;

	using Microsoft.AspNetCore.Hosting;
	using Microsoft.Extensions.Configuration;
	using Microsoft.Extensions.DependencyInjection;
	using Microsoft.Extensions.Hosting;

	using Serilog;
	using Serilog.Context;
	using Serilog.Exceptions;
	using Serilog.Exceptions.Core;
	using Serilog.Exceptions.SqlServer.Destructurers;

	public static class Program
	{
		private static IConfiguration? configuration;

		private static IConfiguration Configuration => configuration??=
			new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("connectionStrings.json", true, true)
				.AddJsonFile("appsettings.json", true, true)
				.AddJsonFile("appsettings.Local.json", true, true)
				.AddEnvironmentVariables()
				.Build();

		public static void Main(string[] args)
		{
			Log.Logger = new LoggerConfiguration()
				.ReadFrom.Configuration(Configuration)
				.Enrich.WithExceptionDetails(new DestructuringOptionsBuilder()
					.WithDefaultDestructurers()
					.WithDestructurers(new[] { new DbUpdateExceptionDestructurer() })
					.WithRootName("Exception"))
				.Enrich.WithProperty("Application", typeof(Program).Assembly.GetName().Name)
				.CreateLogger();

			try
			{
				var host = CreateHostBuilder(args).Build();
				var env = host.Services.GetRequiredService<IWebHostEnvironment>();
				LogContext.PushProperty("Environment", env.EnvironmentName);
				host.Run();
			}
#pragma warning disable CA1031 // Exception can not be handled
			catch (Exception e)
#pragma warning restore CA1031 // Exception can not be handled
			{
				Log.Fatal(e, "Unhandled exception occured! {$Message}", e.Message);
			}
			finally
			{
				Log.CloseAndFlush();
			}
		}

		private static IHostBuilder CreateHostBuilder(string[] args)
			=> Host.CreateDefaultBuilder(args)
				.UseSerilog()
				.ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>())
				.ConfigureAppConfiguration(configure => configure
					.AddJsonFile("connectionStrings.json", true, true)
					.AddJsonFile("appsettings.Local.json", true, true));
	}
}
