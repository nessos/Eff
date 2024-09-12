namespace Nessos.Effects.Examples.AspNetCore;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Nessos.Effects.Examples.AspNetCore.Domain;
using Nessos.Effects.Examples.AspNetCore.EffBindings;
using System.IO;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services
            .AddControllers()
            .AddControllersAsServices();

        services
            .AddSingleton<IUserService>(_ => new InMemoryUserService())
            .AddSingleton<EffectLogger>(_ => new EffectLogger())
            .AddScoped<IEffectHandlerFactory>(provider => new RecordReplayEffectHandlerFactory(provider));

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
