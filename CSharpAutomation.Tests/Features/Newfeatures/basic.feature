@sanity @Regression
Feature: looking for Login to Sabre Hotel Booker

@LoginTest
Scenario: Successful Login with valid credentials
    Given I navigate to the login page
    When I enter username "QASabreClientAgentAdmin" and password "Demo@321@123"
    And I click the login button
    Then I should be logged in successfully
