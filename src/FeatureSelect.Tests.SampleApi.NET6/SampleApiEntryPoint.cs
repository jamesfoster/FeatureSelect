using FeatureSelect.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

var config = builder.Configuration;

builder.Services.AddControllers();
builder.Services.AddFeatureSelect(config.GetSection("Features"));

var app = builder.Build();

app.UseRouting();

app.MapControllers();

app.Run();

public partial class SampleApiEntryPoint { }