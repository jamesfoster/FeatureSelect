FeatureSelect
=

FeatureSelect is a flexible feature toggle library with ASP.NET Core integration.

Installing
-

Install via NuGet

`Install-Package FeatureSelect`

Usage
-

Create a feature source and use it get a feature. A "feature" takes two functions, one to execute if the feature is enabled, and one to execute if the feature is disabled.

```c#
var source = new ConfigurationFeatureSource(Configuration);

var feature = source.GetFeature("MyFeature");

var result = feature.Execute(() => "My feature is enabled", () => "My feature is disabled");
```

In order to integrate with ASP.NET Core, register FeatureSelect in Program.cs using the `AddFeatureSelect` extension method.

```c#
var builder = WebApplication.CreateBuilder(args);

var config = builder.Configuration;

builder.Services.AddControllers();
builder.Services.AddFeatureSelect(config.GetSection("Features"));

var app = builder.Build();

app.UseRouting();

app.MapControllers();

app.Run();
```

Now in your controllers you can use the `[IfEnabled]` and `[IfDisabled]` attributes to toggle on/off any controller or route.

```c#
[IfEnabled("Customers")]
public class CustomerController
{
    [HttpGet("/customers")]
    [IfEnabled("GetCustomers")]
    public IActionResult Get()
    {
        ...
    }
}
```

You can also inject an instance of `FeatureSource`.

```c#
public class MyController
{
    [HttpGet("/my-feature")]
    public string Get([FromServices] FeatureSource features)
    {
        return features
            .GetFeature("MyFeature")
            .Execute(() => "My feature is enabled", () => "My feature is disabled");
    }
}
```

In order to configure which features are enabled simple define them in `appsettings.json`, or you User Secrets or environment variables.

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "Features": {
    "feature1": "enabled",
    "feature2": "diabled",
    "feature3": "enabled"
  }
}
```