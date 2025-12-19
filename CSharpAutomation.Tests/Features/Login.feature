@Regression
Feature: Login to Sabre Hotel Booker

@issue:BUG-100 @tms:TC-001 @link:REQ-LOGIN-01 @LoginTest
Scenario: Successful Login new with valid credentials
    Given I navigate to the login page
    When I enter username "QASabreClientAgentAdmin" and password "Demo@321@123"
    And I click the login button
    Then I should be logged in successfully
