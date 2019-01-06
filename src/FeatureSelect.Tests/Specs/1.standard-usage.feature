Feature: Standard usage
  In order to toggle parts of my application on or off
  As a developer
  I want to only execute certain code if a feature is enabled

  Note: the API requires 2 code blocks. One to execute if the feature
  is enabled and one to execute if the feature is disabled. The result
  of calling the API is the result of whichever block executed.

Scenario: Missing feature
  When I execute feature "foo"
  Then the enabled code block did not execute
  And the disabled code block did execute
  And the result is the result of the disabled code block

Scenario: Disabled feature
  Given feature "foo" is disabled
  When I execute feature "foo"
  Then the enabled code block did not execute
  And the disabled code block did execute
  And the result is the result of the disabled code block

Scenario: Enabled feature
  Given feature "foo" is enabled
  When I execute feature "foo"
  Then the enabled code block did execute
  And the disabled code block did not execute
  And the result is the result of the enabled code block
