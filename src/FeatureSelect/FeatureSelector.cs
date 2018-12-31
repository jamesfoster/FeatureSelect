using System;

namespace FeatureSelect
{
	public interface FeatureSelector
	{
		T Execute<T>(
			string feature,
			Func<string, Maybe<string>> context,
			Func<T> ifEnabled,
			Func<T> ifDisabled
		);
	}

	public static class FeatureSelectorExtensions
	{
		public static FeatureExecutor Freeze(
			this FeatureSelector selector,
			string feature,
			Func<string, Maybe<string>> context
			)
		{
			return selector.Execute<FeatureExecutor>(
				feature,
				context,
				() => new EnabledExecutor(),
				() => new DisabledExecutor()
			);
		}
	}
}