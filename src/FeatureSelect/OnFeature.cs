namespace FeatureSelect
{
    public class OnFeature : IFeature
    {
        public bool IsEnabled(object context) { return true; }
    }
}