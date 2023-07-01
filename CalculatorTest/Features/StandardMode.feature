Feature: StandardMode

A short summary of the feature


Scenario Outline: StandardModeCalculationTesting
	Given I open CalulatorApps
	When I enter <FirstNumber> in the NumberPad
	And I want to do <Function> calculation
	And I enter <SecondNumber> in the NumberPad
	And I enter = in the NumberPad
	Then Results should display <Result>

	Scenarios:
	| ScenarioId     | FirstNumber | SecondNumber | Function       | Result  |
	| Addition       | 9           | 10           | ADDITION       | 19      |
	| SUBSTRACTION   | 15          | 5            | SUBSTRATION    | 10      |
	| DIVISION       | 100         | 10           | DIVISION       | 10      |
	| MULTIPLICATION | 7           | 7            | MULTIPLICATION | 49      |
	| SQUARE         | 11          |              | SQUARE         | 121     |
	| SQUARE_ROOT    | 155         |              | SQUARE_ROOT    | 12.44989|
	| INVERSION      | 154         |              | INVERSION      | 0.006493|