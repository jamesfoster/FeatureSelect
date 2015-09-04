namespace FeatureSelect
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class FeatureFactory
    {
        public IFeature Create(string featureName, FeatureState state, Dictionary<string, string> options = null)
        {
            switch (state)
            {
                case FeatureState.On:
                    return new OnFeature(featureName);

                case FeatureState.Off:
                    return new OffFeature(featureName);

                case FeatureState.Invalid:
                    return new InvalidFeature(featureName);

                case FeatureState.Property:
                    var propertyName = options.Keys.Single();
                    var values = options[propertyName].Split(',');

                    return new PropertyFeature(featureName, propertyName, values);

                default:
                    throw new InvalidOperationException(string.Format("Invalid state {0}", state));
            }
        }
    }
}