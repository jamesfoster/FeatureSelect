namespace FeatureSelect
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class FeatureFactory
    {
        public IFeature Create(FeatureState state, Dictionary<string, string> options = null)
        {
            switch (state)
            {
                case FeatureState.On:
                    return new OnFeature();

                case FeatureState.Off:
                    return new OffFeature();

                case FeatureState.Invalid:
                    return new InvalidFeature();

                case FeatureState.Property:
                    var propertyName = options.Keys.Single();
                    var values = options[propertyName].Split(',');

                    return new PropertyFeature(propertyName, values);

                default:
                    throw new InvalidOperationException(string.Format("Invalid state {0}", state));
            }
        }
    }
}