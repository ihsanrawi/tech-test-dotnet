### Test Description
In the 'PaymentService.cs' file you will find a method for making a payment. At a high level the steps for making a payment are:

 - Lookup the account the payment is being made from
 - Check the account is in a valid state to make the payment
 - Deduct the payment amount from the account's balance and update the account in the database
 
What we’d like you to do is refactor the code with the following things in mind:  
 - Adherence to SOLID principals
 - Testability  
 - Readability 

We’d also like you to add some unit tests to the ClearBank.DeveloperTest.Tests project to show how you would test the code that you’ve produced. The only specific ‘rules’ are:  

 - The solution should build.
 - The tests should all pass.
 - You should not change the method signature of the MakePayment method.

You are free to use any frameworks/NuGet packages that you see fit.  
 
You should plan to spend around 1 to 3 hours to complete the exercise.

## Changes I've made
- SOLID principles implementation
  - Make `PaymentService.MakePayment()` method be single responsibility - by moving other logics into separate methods/classes
  - Decouple data store reponsibilities and payment validation from `MakePayment` method
  - Dependency injection on constructor to remove direct instantiation
- Remove data store creation duplication
- Uses IOption pattern with DI for configuration manager dependency
- Factory pattern implementation on datastore and payment validator
- Improved error handling by introducing `ErrorMessage` in `MakePaymentResult` and trycatch if update account fails
- Introduce simpler Result pattern on `MakePayment` method and early returns for error cases.
- Introduce `DataStoreType` enum to remove  magic string
- Uses moq and AutoFixture for unit tests for mocking dependencies and test data generation
 
## Improvement
- Implement proper Result<T> pattern on `MakePayment` method.
- Better error handling on paymentvalidator result as it does not explain the reason for failure.
- Introduce result pattern to payment validator to support point above.
- Proper logging - using structured logging and enriched context. Help with issue investigation and debugging
