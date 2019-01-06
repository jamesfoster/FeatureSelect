using System;

namespace FeatureSelect.SimpleInjector
{
	public class FeatureAttribute : Attribute
	{
		public string Feature { get; }

		public FeatureAttribute(string feature)
		{
			Feature = feature;
		}
	}
}