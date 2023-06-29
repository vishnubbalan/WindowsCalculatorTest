Feature: ScientificMode

A short summary of the feature

@tag1
Scenario Outline: ScientificModeTesting
	Given I open CalulatorApps
	When I Select Scientific Mode
	And I enter <Number> in the NumberPad
	And I Select <Method> Method For Calculation
	Then Results should display <Result>

	Scenarios: 
	| ScenarioId   | Number | Method       | Result   |
	| SQUARE       | 9      | SQUARE       | 81       |
	| SQUARE_ROOT  | 64     | SQUARE_ROOT  | 8        |
	| FACTORIAL    | 8      | FACTORIAL    | 40320    |
	| TEN_EXPONENT | 7      | TEN_EXPONENT | 10000000 |
