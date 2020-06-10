namespace Nessos.Effects.Examples.AspNetCore
{
    using System.IO;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Rewrite;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.OpenApi.Models;

    using Nessos.Effects.Examples.AspNetCore.EffBindings;
    using Nessos.Effects.Examples.AspNetCore.Domain;

    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddControllers()
                .AddControllersAsServices()
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            services
                .AddSingleton<IUserService>(_ => new InMemoryUserService())
                .AddSingleton<EffectLogger>(_ => new EffectLogger())
                .AddScoped<IMvcEffectHandlerFactory>(provider => new RecordReplayEffectHandlerFactory(provider));

            services
                .AddLogging(logging => logging.AddConsole())
                .AddSwaggerGen(c =>
                    {
                        c.SwaggerDoc("v1", new OpenApiInfo { Title = "Eff Sample", Version = "v1" });
                        c.OperationFilter<SwashBuckleReplayHeaderOperationFilter>();

                        // Set the comments path for the Swagger JSON and UI.
                        string assemblyLocation = System.Reflection.Assembly.GetExecutingAssembly().Location;
                        var xmlFile = Path.ChangeExtension(assemblyLocation, ".xml");
                        c.IncludeXmlComments(xmlFile);
                    });
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseRouting();
            app.UseEndpoints(endpoints => endpoints.MapControllers());

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Eff Sample"));
            app.UseRewriter(new RewriteOptions().AddRedirect("^/?$", "/swagger"));
        }
    }
}
