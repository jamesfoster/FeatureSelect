namespace FeatureSelect
{
    using System.Collections.Generic;
    using System.Linq;

    public static class Feature
    {
        private static readonly FeatureSelector Instance = new FeatureSelector();

        public static void AddConfigSource(IFeatureSource source)
        {
            Instance.AddFeatureSource(source);
        }

        public static bool Enabled(string featureName, object context = null)
        {
            return Instance.Enabled(featureName, context);
        }

        public static bool Disabled(string featureName, object context = null)
        {
            return Instance.Disabled(featureName, context);
        }

        public static IFeature GetFeature(string featureName)
        {
            return Instance.GetFeature(featureName);
        }

        public static void SetFeature(string featureName, string state, IDictionary<string, string> options = null)
        {
            Instance.SetFeature(featureName, state, options);
        }

        public static List<IFeature> ListFeatures()
        {
            return Instance.ListFeatures();
        }

        public static Dictionary<string, bool> ListFeatureStates(object context)
        {
            return Instance
                .ListFeatures()
                .ToDictionary(x => x.Name, x => x.IsEnabled(context));
        }
    }
}