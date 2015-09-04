namespace FeatureSelect
{
    public class OffFeature : IFeature
    {
        public string Name { get; set; }

        public OffFeature(string name)
        {
            Name = name;
        }

        public bool IsEnabled(object context) { return false; }
    }
}