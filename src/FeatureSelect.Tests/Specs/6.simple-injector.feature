Feature: Resolving features in an IoC container
  In order to easily wire up my application using features
  As a developer
  I want to inject a FeatureExecutor using an IoC container

  This example uses SimpleInjector and is a proof of concept
  for anyone wanting to build a container plugin/adapter

Scenario: Can resolve a class depending on a feature
  Given an empty Ioc container capable of resolving features
  And I register a FridayService which depends on feature "friday"
  And I register a FeatureSelector
  And I register a facotry for context
  Then the container verifies successfully

Scenario: Cannot resolve a class depending on a feature without FeatureSelector
  Given an empty Ioc container capable of resolving features
  And I register a FridayService which depends on feature "friday"
  And I register a facotry for context
  Then the container fails to verify

Scenario: Cannot resolve a class depending on a feature without Context
  Given an empty Ioc container capable of resolving features
  And I register a FridayService which depends on feature "friday"
  And I register a FeatureSelector
  Then the container fails to verify

Scenario: Doesn't affect resolving normal dependencies
  Given an empty Ioc container capable of resolving features
  And I register a SampleService
  And I register a SampleClient which depends on SampleService
  Then the container verifies successfully

Scenario: Resolved class correctly executes feature
  Given an empty Ioc container capable of resolving features
  And feature "friday" is enabled when the context contains
  | Key       | Value |
  | DayOfWeek | 5     |
  And I register a FridayService which depends on feature "friday"
  And I register a FeatureSelector
  And I register a facotry for context
  When I run the FridayService
  Then the FridayService returns null

  Given the context contains
  | Key       | Value |
  | DayOfWeek | 5     |
  When I run the FridayService
  Then the FridayService returns "Thank goodness"

Scenario: Resolving multiple features
  Given an empty Ioc container capable of resolving features
  And feature "plus-two" is enabled when the context contains
  | Key  | Value |
  | plus | yes   |
  And feature "times-five" is enabled when the context contains
  | Key   | Value |
  | times | yes   |
  And I register a Calculator which depends on features "plus-two" and "times-five"
  And I register a FeatureSelector
  And I register a facotry for context
  When I calculate the result from 7
  Then the result is 7

  Given the context contains
  | Key  | Value |
  | plus | yes   |
  When I calculate the result from 7
  Then the result is 9

  Given the context contains
  | Key   | Value |
  | times | yes   |
  When I calculate the result from 7
  Then the result is 35

  Given the context contains
  | Key   | Value |
  | plus  | yes   |
  | times | yes   |
  When I calculate the result from 7
  Then the result is 45

