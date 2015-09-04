namespace FeatureSelect
{
    using System.Collections.Generic;
    using System.Linq;

    public class FeatureSelector
    {
        public List<IFeatureSource> Sources { get; set; }

        private readonly HashSet<string> unknownFeatures;

        public FeatureSelector()
        {
            Sources = new List<IFeatureSource>();
            unknownFeatures = new HashSet<string>();
        }

        public void AddFeatureSource(IFeatureSource source)
        {
            Sources.Add(source);
        }

        public bool Disabled(string featureName, object state = null)
        {
            return !Enabled(featureName, state);
        }

        public bool Enabled(string featureName, object state = null)
        {
            var feature = GetFeature(featureName);

            if (feature == null)
            {
                unknownFeatures.Add(featureName);
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

        public List<IFeature> ListFeatures()
        {
            var unknown = unknownFeatures.Select(x => new InvalidFeature(x));

            return Sources
                .SelectMany(x => x.ListFeatures())
                .ToLookup(x => x.Name)
                .Select(x => x.First())
                .Concat(unknown)
                .OrderBy(x => x.Name)
                .ToList();
        }

        public Dictionary<string, bool> ListFeatureStates(object context)
        {
            return ListFeatures()
                .ToDictionary(x => x.Name, x => x.IsEnabled(context));
        }
    }
}
