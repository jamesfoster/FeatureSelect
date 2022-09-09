namespace FeatureSelect.Tests;

internal class DisabledFeatureSource : FeatureSource
{
    public Feature GetFeature(string feature) => Feature.Disabled;
}