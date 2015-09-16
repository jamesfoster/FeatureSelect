namespace FeatureSelect
{
    using System.Collections.Generic;

    public interface IFeatureSource
    {
        IFeature GetFeature(string featureName);

        bool SetFeature(string featureName, string state, IDictionary<string, string> options);

        IEnumerable<IFeature> ListFeatures();
    }
}