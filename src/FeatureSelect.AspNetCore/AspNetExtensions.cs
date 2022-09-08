using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FeatureSelect.AspNetCore;

public static class AspNetExtensions
{
    public static void AddFeatureSelect(this IServiceCollection services, IConfiguration config)
    {
        var source = new ConfigurationFeatureSource(config);

        services.AddSingleton<FeatureSource>(source);
        services.PostConfigure<MvcOptions>(opt => AddFeatureConventions(opt, source));
    }

    private static void AddFeatureConventions(MvcOptions options, FeatureSource source)
    {
        options.Conventions.Add(new MvcFeatureConvention(source));
    }
}
