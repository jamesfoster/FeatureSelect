Feature: Defining features in config
  In order to easily control which features are enabled
  As a release manager
  I want to use standard .NET configuration 

Scenario: Loading config from a file
  Given features are defined in a config file "settings.json" under prefix "features"
  Then feature "feature1" is enabled
  And feature "feature1b" is enabled
  And feature "feature2" is disabled
  And feature "feature2b" is disabled
  And feature "feature3" is disabled
  And feature "feature4" is disabled
  And feature "feature5" is disabled
  And feature "feature6" is disabled
  And feature "feature7" is disabled
  And feature "null-feature" is disabled
  And feature "unknown" is unknown

Scenario Outline: Can configure features based on config

  Feature "feature3" is enabled only if the cohort is "male 18-25"

  Given features are defined in a config file "settings.json" under prefix "features"
  And the context contains
  | Key    | Value    |
  | cohort | <value>  |
  Then feature "feature3" is <result>

  Examples: 
  | value        | result   |
  | male 18-25   | enabled  |
  | male 25-30   | disabled |
  | female 18-25 | disabled |

Scenario Outline: Config based features can accept multiple values

  Feature "feature4" is enabled only if the company is "Abc" OR "Xyz"

  Given features are defined in a config file "settings.json" under prefix "features"
  And the context contains
  | Key     | Value   |
  | company | <value> |
  Then feature "feature4" is <result>

  Examples:
  | value | result   |
  | Abc   | enabled  |
  | Xyz   | enabled  |
  | Ghi   | disabled |

Scenario Outline: Config based features can accept regular expressions

  Feature "feature5" is enabled only if the company
  starts with "abc"

  Given features are defined in a config file "settings.json" under prefix "features"
  And the context contains
  | Key     | Value   |
  | company | <value> |
  Then feature "feature5" is <result>

  Examples:
  | value  | result   |
  | abc    | enabled  |
  | abcdef | enabled  |
  | xyz    | disabled |

Scenario Outline: Multiple values including regular expressions

  Feature "feature6" is enabled only if the company
  starts with "abc" OR
  ends with "abc" OR
  is "ZZZ"

  Given features are defined in a config file "settings.json" under prefix "features"
  And the context contains
  | Key     | Value   |
  | company | <value> |
  Then feature "feature6" is <result>

  Examples:
  | value  | result   |
  | abc    | enabled  |
  | abcdef | enabled  |
  | defabc | enabled  |
  | ZZZ    | enabled  |
  | xyz    | disabled |

Scenario Outline: Config based features can accept comparisons

  Feature "feature7" is enabled only if the price is
  less than 27 OR
  greater than or equal to 30

  Given features are defined in a config file "settings.json" under prefix "features"
  And the context contains
  | Key     | Value   |
  | price   | <value> |
  Then feature "feature7" is <result>

  Examples:
  | value | result   |
  | 26    | enabled  |
  | 27    | disabled |
  | 28    | disabled |
  | 29    | disabled |
  | 30    | enabled  |
  | 31    | enabled  |

Scenario Outline: Config based features can accept compound comparisons

  Feature "feature8" is enabled only if the price is
  greater than 17 AND
  less than or equal to 21

  Given features are defined in a config file "settings.json" under prefix "features"
  And the context contains
  | Key     | Value   |
  | price   | <value> |
  Then feature "feature8" is <result>

  Examples:
  | value | result   |
  | 17    | disabled |
  | 18    | enabled  |
  | 19    | enabled  |
  | 20    | enabled  |
  | 21    | enabled  |
  | 22    | disabled |
