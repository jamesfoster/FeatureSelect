Feature: Circuit breaking
  In order to prevent buggy features from affecting the application
  As a developer
  I want to be able to configure short circuiting to turn features off automatically

Scenario: Exceptions are rethrown

  Given feature "foo" throws an exception if enabled
  And feature "foo" is enabled
  When I execute feature "foo"
  Then the enabled code block did execute
  And the exception is thrown

Scenario: Disabling a feature when too many attemps are made

  Given feature "foo" throws an exception if enabled
  And feature "foo" is enabled
  And features will be disabled after 3 failed attempts
  When I execute feature "foo"
  Then the enabled code block did execute
  When I execute feature "foo"
  Then the enabled code block did execute
  When I execute feature "foo"
  Then the enabled code block did execute
  When I execute feature "foo"
  Then the disabled code block did execute

Scenario: Resetting a broken circuit

  Given feature "foo" throws an exception if enabled
  And feature "foo" is enabled
  And features will be disabled after 3 failed attempts
  When I execute feature "foo"
  And I execute feature "foo"
  And I execute feature "foo"
  And I execute feature "foo"
  Then the disabled code block did execute

  When I reset the circuit breaker for feature "foo"
  And I execute feature "foo"
  Then the enabled code block did execute
  
Scenario: Executing a code block which doesn't throw does not reset the failure count

  Given feature "foo" throws an exception if enabled
  And feature "foo" is enabled
  And features will be disabled after 3 failed attempts
  When I execute feature "foo"
  And I execute feature "foo"

  Given feature "foo" does not throw an exception if enabled
  When I execute feature "foo"
  Then the enabled code block did execute
  
  Given feature "foo" throws an exception if enabled
  When I execute feature "foo"
  And I execute feature "foo"
  Then the disabled code block did execute

Scenario: Exceptions from the disabled code block are rethrown

  Given feature "foo" throws an exception if disabled
  And feature "foo" is disabled
  When I execute feature "foo"
  Then the disabled code block did execute
  And the exception is thrown

Scenario: Exceptions from the disabled code block do not increment the failure count

  Given feature "foo" throws an exception if disabled
  And feature "foo" is enabled when the context contains
  | Key    | Value |
  | Broken | false |
  And features will be disabled after 3 failed attempts
  When I execute feature "foo"
  And I execute feature "foo"
  And I execute feature "foo"
  And I execute feature "foo"
  Given the context contains
  | Key    | Value |
  | Broken | false |
  When I execute feature "foo"
  Then the enabled code block did execute
