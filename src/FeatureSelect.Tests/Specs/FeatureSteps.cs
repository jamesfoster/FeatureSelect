using System;
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
		private FeatureSource CombineWith(FeatureSource source) =>
			new CompoundFeatureSource(source, SpecData.FeatureSource);

		[Given(@"feature ""(.*)"" is disabled")]
		public void GivenFeatureIsDisabled(string featureName)
		{
			SpecData.FeatureSource = CombineWith(new DisabledFeatureSource(featureName));
		}

		[Given(@"feature ""(.*)"" is enabled")]
		public void GivenFeatureIsEnabled(string featureName)
		{
			SpecData.FeatureSource = CombineWith(new EnabledFeatureSource(featureName));
		}

		[Given(@"feature ""(.*)"" is enabled when the context contains")]
		public void GivenFeatureIsEnabledWhenTheContextContains(string featureName, Table table)
		{
			SpecData.FeatureSource = CombineWith(new TableFeatureSource(featureName, table));
		}

		[Given(@"features are defined in a config file ""(.*)"" under prefix ""(.*)""")]
		public void GivenFeaturesAreDefinedInAConfigFile(string configFile, string prefix)
		{
			var config = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("settings.json")
				.Build()
				.GetSection(prefix);

			SpecData.FeatureSource = new ConfigurationFeatureSource(config);
		}

		[Given(@"I freeze feature ""(.*)""")]
		public void GivenIFreezeFeature(string featureName)
		{
			SpecData.FrozenFeatures[featureName] = SpecData.FeatureSelector.Freeze(
				featureName,
				SpecData.FeatureContext
			);
		}

		[Given(@"features will be disabled after (.*) failed attempts")]
		public void GivenFeaturesWillBeDisabledAfterFailedAttempts(int attemps)
		{
			SpecData.FeatureSelector = new CircuitBreakingFeatureSelector(
				SpecData.FeatureSelector,
				attemps
			);
		}

		[When(@"I reset the circuit breaker for feature ""(.*)""")]
		public void WhenIResetTheCircuitBreakerForFeature(string featureName)
		{
			((CircuitBreakingFeatureSelector)SpecData.FeatureSelector).Reset(featureName);
		}

		[Given(@"feature ""(.*)"" throws an exception if enabled")]
		public void GivenFeatureThrowsAnExceptionIfEnabled(string featureName)
		{
			SpecData.IfEnabled = () => throw new Exception("Oops!");
		}

		[Given(@"feature ""(.*)"" does not throw an exception if enabled")]
		public void GivenFeatureDoesNotThrowAnExceptionIfEnabled(string p0)
		{
			SpecData.IfEnabled = () => "a";
		}

		[Given(@"feature ""(.*)"" throws an exception if disabled")]
		public void GivenFeatureThrowsAnExceptionIfDisabled(string p0)
		{
			SpecData.IfDisabled = () => throw new Exception("Oops!");
		}

		[Given(@"the context contains")]
		public void GivenTheContextContains(Table table)
		{
			var map = table
				.Rows
				.ToDictionary(x => x["Key"], x => ParseNull(x["Value"]));

			SpecData.FeatureContext = key => map.TryGetValue(key, out var value)
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
				SpecData.Result = SpecData.FeatureSelector.Execute(
					featureName,
					SpecData.FeatureContext,
					SpecData.IfEnabled,
					SpecData.IfDisabled
				);
			}
			catch (Exception e)
			{
				SpecData.ThrownException = e;
			}
		}

		[When(@"I execute the frozen feature ""(.*)""")]
		public void WhenIExecuteTheFrozenFeature(string featureName)
		{
			Spec.Set(false, "EnabledCalled");
			Spec.Set(false, "DisabledCalled");

			try
			{
				SpecData.Result = SpecData
					.FrozenFeatures[featureName]
					.Execute(SpecData.IfEnabled, SpecData.IfDisabled);
			}
			catch (Exception e)
			{
				SpecData.ThrownException = e;
			}
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
			Assert.AreEqual("a", SpecData.Result);
		}

		[Then(@"the result is the result of the disabled code block")]
		public void ThenTheResultIsTheResultOfTheDisabledCodeBlock()
		{
			Assert.AreEqual("b", SpecData.Result);
		}

		[Then(@"feature ""(.*)"" is disabled")]
		public void ThenFeatureIsDisabled(string featureName)
		{
			var state = SpecData.FeatureSource.GetFeatureState(featureName, SpecData.FeatureContext);

			Assert.AreEqual($"{featureName}:Disabled", $"{featureName}:{state}");
		}

		[Then(@"feature ""(.*)"" is enabled")]
		public void ThenFeatureIsEnabled(string featureName)
		{
			var state = SpecData.FeatureSource.GetFeatureState(featureName, SpecData.FeatureContext);

			Assert.AreEqual($"{featureName}:Enabled", $"{featureName}:{state}");
		}

		[Then(@"feature ""(.*)"" is unknown")]
		public void ThenFeatureIsUnknown(string featureName)
		{
			var state = SpecData.FeatureSource.GetFeatureState(featureName, SpecData.FeatureContext);

			Assert.AreEqual($"{featureName}:Unknown", $"{featureName}:{state}");
		}

		[Then(@"the exception is thrown")]
		public void ThenTheExceptionIsThrown()
		{
			Assert.AreEqual("Oops!", SpecData.ThrownException.Message);
		}
	}

	public class CompoundFeatureSource : FeatureSource
	{
		private readonly FeatureSource first;
		private readonly FeatureSource second;

		public CompoundFeatureSource(FeatureSource first, FeatureSource second)
		{
			this.first = first;
			this.second = second;
		}

		public FeatureState GetFeatureState(string feature, FeatureContext context)
		{
			var state = first.GetFeatureState(feature, context);

			return state != FeatureState.Unknown 
				? state 
				: second.GetFeatureState(feature, context);
		}
	}

	public class EnabledFeatureSource : FeatureSource
	{
		private readonly string enabledFeature;

		public EnabledFeatureSource(string enabledFeature) =>
			this.enabledFeature = enabledFeature;

		public FeatureState GetFeatureState(string feature, FeatureContext context) =>
			feature == enabledFeature ? FeatureState.Enabled : FeatureState.Unknown;
	}

	public class DisabledFeatureSource : FeatureSource
	{
		private readonly string disabledFeature;

		public DisabledFeatureSource(string disabledFeature) =>
			this.disabledFeature = disabledFeature;

		public FeatureState GetFeatureState(string feature, FeatureContext context) =>
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

		public FeatureState GetFeatureState(string feature, FeatureContext context)
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
