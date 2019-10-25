# EffPromo API

A barebones AspNetCore 3.0 api project to showcase effects.

EffPromo.Api folder needs two files to function:

appsettings.Local.json
(Optional, changes default log levels and logs to seq if available)

```(json)
{
  "Serilog": {
    "LevelSwitches": { "$seqLevel": "Verbose" },
    "MinimumLevel": {
      "Default": "Verbose",
      "Override": {
        "Microsoft.Hosting.Lifetime": "Information"
      }
    }
  },
  "Serilog:WriteTo:2:Args:configureLogger:WriteTo:0:Args": {
    "serverUrl": "https://localhost",
    "apiKey": "your-key-here"
  }
}
```

connectionStrings.json
(Optional, changes default location of database)

```(json)
{
  "ConnectionStrings": {
    "EffPromoSample": "Data Source=C:\\Software\\Users\\akritikos\\Desktop\\GitHub\\EffPromoSample.db"
  }
}
```
