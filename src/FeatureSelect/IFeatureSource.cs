namespace FeatureSelect
{
    using System.Collections.Generic;

    public interface IFeatureSource
    {
        IFeature GetFeature(string featureName);

        IEnumerable<IFeature> ListFeatures();
    }
}