using System;
using System.Collections.Generic;

namespace FeatureSelect
{
	public class CircuitBreakingFeatureSelector : FeatureSelector
	{
		private readonly FeatureSelector inner;
		private readonly int maxFailedAttempts;
		private readonly Dictionary<string, int> failures;

		public CircuitBreakingFeatureSelector(FeatureSelector inner, int maxFailedAttempts)
		{
			this.inner = inner;
			this.maxFailedAttempts = maxFailedAttempts;
			this.failures = new Dictionary<string, int>();
		}

		public FeatureExecutor Freeze(
			string feature,
			Func<string, Maybe<string>> context)
		{
			if (HasExceededMaxFailedAttempts(feature)) return new DisabledExecutor();

			return new CircuitBreakingFeatureExecutor(
				inner.Freeze(feature, context),
				IncrementFailureCount(feature)
			);
		}

		private bool HasExceededMaxFailedAttempts(string feature) =>
			FailureCount(feature) >= maxFailedAttempts;

		private Action IncrementFailureCount(string feature) => () =>
			failures[feature] = FailureCount(feature) + 1;

		private int FailureCount(string feature) =>
			failures.TryGetValue(feature, out var f) ? f : 0;

		public void Reset(string feature) =>
			failures[feature] = 0;

		private class CircuitBreakingFeatureExecutor : FeatureExecutor
		{
			private readonly FeatureExecutor inner;
			private readonly Action onException;

			public CircuitBreakingFeatureExecutor(FeatureExecutor inner, Action onException)
			{
				this.inner = inner;
				this.onException = onException;
			}

			public T Execute<T>(Func<T> ifEnabled, Func<T> ifDisabled)
			{
				return inner.Execute(IncrementOnException(ifEnabled), ifDisabled);

				Func<T> IncrementOnException(Func<T> func) => () =>
				{
					try
					{
						return func();
					}
					catch (Exception)
					{
						onException();
						throw;
					}
				};
			}
		}
	}
}