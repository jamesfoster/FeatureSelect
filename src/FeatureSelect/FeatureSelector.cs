using System;

namespace FeatureSelect
{
	public interface FeatureSelector
	{
		FeatureExecutor Freeze(
			string feature,
			FeatureContext context
		);
	}

	public static class FeatureSelectorExtensions
	{
		public static T Execute<T>(
			this FeatureSelector selector,
			string feature,
			FeatureContext context,
			Func<T> ifEnabled,
			Func<T> ifDisabled)
		{
			return selector
				.Freeze(feature, context)
				.Execute(ifEnabled, ifDisabled);
		}
	}
}