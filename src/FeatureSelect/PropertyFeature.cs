namespace FeatureSelect
{
    using System;
    using System.Linq;

    public class PropertyFeature : IFeature
    {
        public string Property { get; set; }

        public string[] Values { get; set; }

        public PropertyFeature(string property, string[] values)
        {
            Property = property;
            Values = values;
        }

        public bool IsEnabled(object context)
        {
            if (context == null)
            {
                return false;
            }

            var property = context.GetType().GetProperty(Property);

            if (property == null)
            {
                return false;
            }

            var value = property.GetValue(context);

            return Values.Any(x => ValueMatches(x, value));
        }

        private static bool ValueMatches(string value1, object value2)
        {
            var str1 = value1.Trim();
            var str2 = value2.ToString().Trim();

            return str1.Equals(str2, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}