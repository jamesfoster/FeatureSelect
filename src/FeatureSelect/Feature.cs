namespace FeatureSelect
{
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
    }
}