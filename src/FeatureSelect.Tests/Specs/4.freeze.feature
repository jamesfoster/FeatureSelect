Feature: Freeze
  In order to ensure my application behaves consistently in a threaded environment
  As a developer
  I want to be and to freeze the state of a feature

Scenario: Frozen features maintain the state at the time they were frozen
  Given feature "foo" is enabled
  And I freeze feature "foo"
  When I execute the frozen feature "foo"
  Then the enabled code block did execute

  Given feature "foo" is disabled
  When I execute the frozen feature "foo"
  Then the enabled code block did execute

Scenario: Freezing a feature again captures the new state
  Given feature "foo" is enabled when the context contains
  | Key  | Value |
  | Name | Fred  |
  And the context contains
  | Key  | Value |
  | Name | Frodo |
  And I freeze feature "foo"

  Given the context contains
  | Key  | Value |
  | Name | Fred  |
  When I execute the frozen feature "foo"
  Then the disabled code block did execute
  Given I freeze feature "foo"
  When I execute the frozen feature "foo"
  Then the enabled code block did execute
