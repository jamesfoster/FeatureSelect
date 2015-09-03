namespace FeatureSelect
{
    public interface IFeatureSource
    {
        IFeature GetFeature(string featureName);
    }
}