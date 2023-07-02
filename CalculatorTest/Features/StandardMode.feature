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



Scenario Outline: Retreive Cancel and retreve and validate the history
	Given I open CalculatorApp in full window
	When I want to do addition for following
	| ParamOne | ParamTwo | Sum |
	| 1        | 2        | 3   |
	| 2        | 2        | 4   |
	| 6        | 6        | 12  |
	Then I can retreive the following calculations from the history
	| Calculation | Expression | Result |
	| 1 + 2= 3    | 1 + 2      | 3      |
	| 2 + 2= 4    | 2 + 2      | 4      |

Scenario Outline: Verify Memory Addition and Retrival
	Given I open CalculatorApp in full window
	When I want to do addition for following with memory addition
	| ParamOne | ParamTwo | Sum |
	| 1        | 2        | 3   |
	| 2        | 2        | 4   |
	| 6        | 6        | 12  |
	Then I can retreive the following calculations from the memory
	| Result |
	| 3      |
	| 4      |


