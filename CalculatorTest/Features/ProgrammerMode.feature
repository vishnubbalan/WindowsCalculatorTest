Feature: ProgrammerMode

A short summary of the feature


Scenario Outline: ProgrammerModeTesting
	Given I open CalulatorApps
	When I Select Programmer Mode
	And I select DEC Selection
	And I enter <DecimalValue> in the NumberPad
	And I select <Type> Selection
	Then Results should display <Result>

	Scenarios:
	| ScenarioId | DecimalValue | Type | Result |
	| HEXType    | 79           | HEX  | 4F     |
	| OCTType    | 46           | OCT  | 56     |
	| BINType    | 15           | BIN  | 1111   |
	| DECType    | 16           | DEC  | 16     |



