using System;
using FeatureSelect.SimpleInjector;
using NUnit.Framework;
using SimpleInjector;
using TechTalk.SpecFlow;

namespace FeatureSelect.Tests.Specs
{
	[Binding]
	public class SimpleInjectorSteps
	{
		[Given(@"an empty Ioc container capable of resolving features")]
		public void GivenAnEmptyIocContainerCapableOfResolvingFeatures()
		{
			SpecData.Container = new Container();
			SpecData.Container.Options.AddFeatureInjectionBehavior();
		}

		[Given(@"I register a SampleService")]
		public void GivenIRegisterASampleService()
		{
			SpecData.Container.Register<SampleService>();
		}

		[Given(@"I register a SampleClient which depends on SampleService")]
		public void GivenIRegisterASampleClientWhichDependsOnSampleService()
		{
			SpecData.Container.Register<SampleClient>();
		}


		[Given(@"I register a FridayService which depends on feature ""friday""")]
		public void GivenIRegisterAClassWhichDependsOnFeature()
		{
			SpecData.Container.Register<FridayService>();
		}

		[Given(@"I register a Calculator which depends on features ""plus-two"" and ""times-five""")]
		public void GivenIRegisterACalculatorWhichDependsOnFeaturesAnd()
		{
			SpecData.Container.Register<Calculator>();
		}

		[Given(@"I register a FeatureSelector")]
		public void GivenIRegisterFeatureSelector()
		{
			SpecData.Container.RegisterInstance(SpecData.FeatureSelector);
		}

		[Given(@"I register a facotry for context")]
		public void GivenIRegisterAFacotryForContext()
		{
			SpecData.Container.Register(
				() => SpecData.FeatureContext,
				Lifestyle.Transient
			);
		}

		[Then(@"the container verifies successfully")]
		public void ThenTheConatinerVerifiesSuccessfully()
		{
			SpecData.Container.Verify();
		}

		[Then(@"the container fails to verify")]
		public void ThenTheConatinerFailsToVerify()
		{
			Assert.Throws<InvalidOperationException>(() => SpecData.Container.Verify());
		}

		[When(@"I run the FridayService")]
		public void WhenIRunTheFridayService()
		{
			var fridayService = SpecData.Container.GetInstance<FridayService>();

			SpecData.Result = fridayService.Run();
		}

		[Then(@"the FridayService returns null")]
		public void ThenTheFridayServiceReturnsNull()
		{
			Assert.AreEqual(null, SpecData.Result);
		}

		[Then(@"the FridayService returns ""(.*)""")]
		public void ThenTheFridayServiceReturns(string value)
		{
			Assert.AreEqual(value, SpecData.Result);
		}

		[When(@"I calculate the result from (.*)")]
		public void WhenICalculateTheResultFrom(int input)
		{
			var calculator = SpecData.Container.GetInstance<Calculator>();

			SpecData.Result = calculator.Calculate(input);
		}

		[Then(@"the result is (.*)")]
		public void ThenTheResultIs(int expected)
		{
			Assert.AreEqual(expected, SpecData.Result);
		}

		public class SampleService { }

		public class SampleClient
		{
			public SampleClient(SampleService service)
			{
			}
		}

		public class FridayService
		{
			private readonly FeatureExecutor fridayFeature;

			public FridayService([Feature("friday")] FeatureExecutor fridayFeature)
			{
				this.fridayFeature = fridayFeature;
			}

			public string Run()
			{
				return fridayFeature.Execute(
					ifEnabled: () => "Thank goodness",
					ifDisabled: () => null
				);
			}
		}

		private class Calculator
		{
			private readonly FeatureExecutor plusTwo;
			private readonly FeatureExecutor timesFive;

			public Calculator(
				[Feature("plus-two")] FeatureExecutor plusTwo,
				[Feature("times-five")] FeatureExecutor timesFive)
			{
				this.plusTwo = plusTwo;
				this.timesFive = timesFive;
			}

			public int Calculate(int input)
			{
				var plus = plusTwo.Execute(
					ifEnabled: () => 2,
					ifDisabled: () => 0
				);
				var times = timesFive.Execute(
					ifEnabled: () => 5,
					ifDisabled: () => 1
				);

				return (input + plus) * times;
			}
		}
	}
}
