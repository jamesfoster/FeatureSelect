namespace FeatureSelect
{
    public class InvalidFeature : IFeature
    {
        public bool IsEnabled(object context) { return false; }
    }
}