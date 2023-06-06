Feature: Category

A short summary of the feature

@Category
Scenario: Get all categories
	Given I want all categories
	When I send a request - Category
	Then I expected a list of categories

