Feature: DateCalculationMode

A short summary of the feature


Scenario Outline: DateCalculationTesting
	Given I open CalulatorApps
	When I Select Date Mode
	And I select Difference Between Dates
	And I Select 1 of January in StartDate DateSelector
	And I Select 24 of February in EndDate DateSelector
	Then Day Difference shows 54 days
	Then Week Difference shows 1 month, 3 weeks, 2 days
	
