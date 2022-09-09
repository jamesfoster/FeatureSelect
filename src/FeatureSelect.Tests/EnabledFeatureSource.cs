namespace FeatureSelect.Tests;

internal class EnabledFeatureSource : FeatureSource
{
    public Feature GetFeature(string feature) => Feature.Enabled;
}