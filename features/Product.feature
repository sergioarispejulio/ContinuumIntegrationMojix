Feature: Product

A short summary of the feature

@Product 
Scenario: Get all products
	Given I want all products
	When I send a request - Product
	Then I expected a list of products

@Product 
Scenario: Create a product
	Given I need login to the API 
	When I setting the product to create
	And I send a request - Product
	Then I expected a bad request to confirm the creation (API rules)

@Product 
Scenario: Get products with a specific category
	Given I have the ID of a category with value 5
	When I send a request - Product
	Then I expected a list of products only with the category
