namespace FeatureSelect
{
    using System.Collections.Generic;
    using System.Linq;

    public class FeatureSelector
    {
        public List<IFeatureSource> Sources { get; set; }

        public FeatureSelector()
        {
            Sources = new List<IFeatureSource>();
        }

        public void AddFeatureSource(IFeatureSource source)
        {
            Sources.Add(source);
        }

        public bool Enabled(string featureName, object state = null)
        {
            var feature = GetFeature(featureName);

            if (feature == null)
            {
                return false;
            }

            return feature.IsEnabled(state);
        }

        public IFeature GetFeature(string featureName)
        {
            return Sources
                .Select(x => x.GetFeature(featureName))
                .FirstOrDefault(x => x != null);
        }

        public bool Disabled(string featureName, object state = null)
        {
            return !Enabled(featureName, state);
        }
    }
}
