namespace FeatureSelect
{
    public class OnFeature : IFeature
    {
        public string Name { get; set; }

        public OnFeature(string name)
        {
            Name = name;
        }

        public bool IsEnabled(object context) { return true; }
    }
}