using Microsoft.Extensions.Configuration;

namespace FeatureSelect;

public class ConfigurationFeatureSource : FeatureSource
{
    private readonly IConfiguration config;

    public ConfigurationFeatureSource(IConfiguration config)
    {
        this.config = config;
    }

    public Feature GetFeature(string feature)
    {
        var value = config[feature];
        if (value == null) return Feature.Disabled;

        if (value.Equals("enabled", StringComparison.OrdinalIgnoreCase))
        {
            return Feature.Enabled;
        }

        return Feature.Disabled;
    }
}