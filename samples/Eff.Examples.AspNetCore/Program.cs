using Nessos.Effects.Examples.AspNetCore;

var host = Host
    .CreateDefaultBuilder(args)
    .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>())
    .Build();

await host.RunAsync();