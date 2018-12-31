using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FeatureSelect.Configuration;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using TechTalk.SpecFlow;

namespace FeatureSelect.Tests.Specs
{
	[Binding]
	public class FeatureSteps
	{
		#region Data

		private static FeatureSource FeatureSource
		{
			get => Spec.GetOrDefault(Feature.EmptySource);
			set => Spec.Set(value);
		}

		private static FeatureSelector FeatureSelector
		{
			get => Spec.GetOrDefault<FeatureSelector>(new SimpleFeatureSelector(FeatureSource));
			set => Spec.Set(value);
		}

		private static Func<string, Maybe<string>> FeatureContext
		{
			get => Spec.GetOrDefault<Func<string, Maybe<string>>>("FeatureContext", _ => Maybe.Nothing<string>());
			set => Spec.Set(value, "FeatureContext");
		}

		private static Dictionary<string, FeatureExecutor> FrozenFeatures
		{
			get
			{
				if (!Spec.TryGetValue<Dictionary<string, FeatureExecutor>>(out var value))
				{
					value = new Dictionary<string, FeatureExecutor>();
					Spec.Set(value);
				}

				return value;
			}
		}

		private static object Result
		{
			get => Spec.Get<object>("Result");
			set => Spec.Set(value, "Result");
		}

		private static Exception ThrownException
		{
			get => Spec.Get<Exception>();
			set => Spec.Set(value);
		}

		private static Func<string> IfEnabled
		{
			get => () =>
			{
				Spec.Set(true, "EnabledCalled");
				return Spec.GetOrDefault<Func<string>>("IfEnabled", () => "a")();
			};
			set => Spec.Set(value, "IfEnabled");
		}

		private static Func<string> IfDisabled
		{
			get => () =>
			{
				Spec.Set(true, "DisabledCalled");
				return Spec.GetOrDefault<Func<string>>("IfDisabled", () => "b")();
			};
			//set => Spec.Set(value, "IfDisabled");
		}

		#endregion

		[Given(@"feature ""(.*)"" is disabled")]
		public void GivenFeatureIsDisabled(string featureName)
		{
			FeatureSource = new DisabledFeatureSource(featureName);
		}

		[Given(@"feature ""(.*)"" is enabled")]
		public void GivenFeatureIsEnabled(string featureName)
		{
			FeatureSource = new EnabledFeatureSource(featureName);
		}

		[Given(@"feature ""(.*)"" is enabled when the context contains")]
		public void GivenFeatureIsEnabledWhenTheContextContains(string featureName, Table table)
		{
			FeatureSource = new TableFeatureSource(featureName, table);
		}

		[Given(@"features are defined in a config file ""(.*)"" under prefix ""(.*)""")]
		public void GivenFeaturesAreDefinedInAConfigFile(string configFile, string prefix)
		{
			var config = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("settings.json")
				.Build()
				.GetSection(prefix);

			FeatureSource = new ConfigurationFeatureSource(config);
		}

		[Given(@"I freeze feature ""(.*)""")]
		public void GivenIFreezeFeature(string featureName)
		{
			FrozenFeatures[featureName] = FeatureSelector.Freeze(featureName, FeatureContext);
		}

		[Given(@"features will be disabled after (.*) failed attempts")]
		public void GivenFeaturesWillBeDisabledAfterFailedAttempts(int attemps)
		{
			FeatureSelector = new CircuitBreakingFeatureSelector(FeatureSelector, attemps);
		}

		[When(@"I reset the circuit breaker for feature ""(.*)""")]
		public void WhenIResetTheCircuitBreakerForFeature(string featureName)
		{
			((CircuitBreakingFeatureSelector)FeatureSelector).Reset(featureName);
		}

		[Given(@"feature ""(.*)"" throws an exception if enabled")]
		public void GivenFeatureThrowsAnExceptionIfEnabled(string featureName)
		{
			IfEnabled = () => throw new Exception("Oops!");
		}

		[Given(@"feature ""(.*)"" does not throw an exception if enabled")]
		public void GivenFeatureDoesNotThrowAnExceptionIfEnabled(string p0)
		{
			IfEnabled = () => "a";
		}

		[Given(@"the context contains")]
		public void GivenTheContextContains(Table table)
		{
			var map = table
				.Rows
				.ToDictionary(x => x["Key"], x => ParseNull(x["Value"]));

			FeatureContext = key => map.TryGetValue(key, out var value)
				? Maybe.Just(value)
				: Maybe.Nothing<string>();

			string ParseNull(string s) => s == "[null]" ? null : s;
		}

		[When(@"I execute feature ""(.*)""")]
		public void WhenIExecuteFeature(string featureName)
		{
			Spec.Set(false, "EnabledCalled");
			Spec.Set(false, "DisabledCalled");

			try
			{
				Result = FeatureSelector.Execute(
					featureName,
					FeatureContext,
					IfEnabled,
					IfDisabled
				);
			}
			catch (Exception e)
			{
				ThrownException = e;
			}
		}

		[When(@"I execute the frozen feature ""(.*)""")]
		public void WhenIExecuteTheFrozenFeature(string featureName)
		{
			Spec.Set(false, "EnabledCalled");
			Spec.Set(false, "DisabledCalled");

			Result = FrozenFeatures[featureName].Execute(IfEnabled, IfDisabled);
		}

		[Then(@"the enabled code block did execute")]
		public void ThenTheEnabledCodeBlockDidExecute()
		{
			Assert.AreEqual(true, Spec.Get<bool>("EnabledCalled"));
		}

		[Then(@"the enabled code block did not execute")]
		public void ThenTheEnabledCodeBlockDidNotExecute()
		{
			Assert.AreEqual(false, Spec.Get<bool>("EnabledCalled"));
		}

		[Then(@"the disabled code block did execute")]
		public void ThenTheDisabledCodeBlockDidExecute()
		{
			Assert.AreEqual(true, Spec.Get<bool>("DisabledCalled"));
		}

		[Then(@"the disabled code block did not execute")]
		public void ThenTheDisabledCodeBlockDidNotExecute()
		{
			Assert.AreEqual(false, Spec.Get<bool>("DisabledCalled"));
		}

		[Then(@"the result is the result of the enabled code block")]
		public void ThenTheResultIsTheResultOfTheEnabledCodeBlock()
		{
			Assert.AreEqual("a", Result);
		}

		[Then(@"the result is the result of the disabled code block")]
		public void ThenTheResultIsTheResultOfTheDisabledCodeBlock()
		{
			Assert.AreEqual("b", Result);
		}

		[Then(@"feature ""(.*)"" is disabled")]
		public void ThenFeatureIsDisabled(string featureName)
		{
			var state = FeatureSource.GetFeatureState(featureName, FeatureContext);

			Assert.AreEqual($"{featureName}:Disabled", $"{featureName}:{state}");
		}

		[Then(@"feature ""(.*)"" is enabled")]
		public void ThenFeatureIsEnabled(string featureName)
		{
			var state = FeatureSource.GetFeatureState(featureName, FeatureContext);

			Assert.AreEqual($"{featureName}:Enabled", $"{featureName}:{state}");
		}

		[Then(@"feature ""(.*)"" is unknown")]
		public void ThenFeatureIsUnknown(string featureName)
		{
			var state = FeatureSource.GetFeatureState(featureName, FeatureContext);

			Assert.AreEqual($"{featureName}:Unknown", $"{featureName}:{state}");
		}

		[Then(@"the exception is thrown")]
		public void ThenTheExceptionIsThrown()
		{
			Assert.AreEqual("Oops!", ThrownException.Message);
		}
	}

	public class EnabledFeatureSource : FeatureSource
	{
		private readonly string enabledFeature;

		public EnabledFeatureSource(string enabledFeature) =>
			this.enabledFeature = enabledFeature;

		public FeatureState GetFeatureState(string feature, Func<string, Maybe<string>> context) =>
			feature == enabledFeature ? FeatureState.Enabled : FeatureState.Unknown;
	}

	public class DisabledFeatureSource : FeatureSource
	{
		private readonly string disabledFeature;

		public DisabledFeatureSource(string disabledFeature) =>
			this.disabledFeature = disabledFeature;

		public FeatureState GetFeatureState(string feature, Func<string, Maybe<string>> context) =>
			feature == disabledFeature ? FeatureState.Disabled : FeatureState.Unknown;
	}

	public class TableFeatureSource : FeatureSource
	{
		private readonly Table table;
		private readonly string featureName;

		public TableFeatureSource(string featureName, Table table)
		{
			this.featureName = featureName;
			this.table = table;
		}

		public FeatureState GetFeatureState(string feature, Func<string, Maybe<string>> context)
		{
			if (feature != featureName) return FeatureState.Unknown;

			return table.Rows.All(r => Matches(context(r["Key"]), r["Value"]))
				? FeatureState.Enabled
				: FeatureState.Disabled;

			bool Matches(Maybe<string> expected, string value) =>
				expected.Match(
					just: v => v == value,
					nothing: () => false
				);
		}
	}
}
