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

		public T Execute<T>(
			string feature,
			Func<string, Maybe<string>> context,
			Func<T> ifEnabled,
			Func<T> ifDisabled)
		{
			if (HasExceededMaxFailedAttempts(feature)) return ifDisabled();

			return inner.Execute(feature, context, IncrementOnFailure(ifEnabled), ifDisabled);

			Func<T> IncrementOnFailure(Func<T> func) => () =>
			{
				try
				{
					return func();
				}
				catch (Exception)
				{
					IncrementFailureCount(feature);
					throw;
				}
			};
		}

		private bool HasExceededMaxFailedAttempts(string feature) =>
			FailureCount(feature) >= maxFailedAttempts;

		private void IncrementFailureCount(string feature) =>
			failures[feature] = FailureCount(feature) + 1;

		private int FailureCount(string feature) =>
			failures.TryGetValue(feature, out var f) ? f : 0;

		public void Reset(string feature) =>
			failures[feature] = 0;
	}
}