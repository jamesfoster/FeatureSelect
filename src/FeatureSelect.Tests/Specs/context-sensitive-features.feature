Feature: Context sensitive features
  In order to enable parts of my application in a given situation
  As a developer
  I want to be able to enable/disable a feature based on context

Scenario: Context does not contain required values
  Given feature "foo" is enabled when the context contains
  | Key     | Value    |
  | Company | Fabricam |
  Then feature "foo" is disabled
  
Scenario: Context contains required values
  Given feature "foo" is enabled when the context contains
  | Key     | Value    |
  | Company | Fabricam |
  And the context contains
  | Key     | Value    |
  | Company | Fabricam |
  Then feature "foo" is enabled
  
Scenario: Context contains incorrect values
  Given feature "foo" is enabled when the context contains
  | Key     | Value    |
  | Company | Fabricam |
  And the context contains
  | Key     | Value    |
  | Company | Exemplum |
  Then feature "foo" is disabled
