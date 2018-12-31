using System;

namespace FeatureSelect
{
	public interface FeatureExecutor
	{
		T Execute<T>(Func<T> ifEnabled, Func<T> ifDisabled);
	}

	internal class EnabledExecutor : FeatureExecutor
	{
		public T Execute<T>(Func<T> ifEnabled, Func<T> ifDisabled)
		{
			return ifEnabled();
		}
	}

	internal class DisabledExecutor : FeatureExecutor
	{
		public T Execute<T>(Func<T> ifEnabled, Func<T> ifDisabled)
		{
			return ifDisabled();
		}
	}
}