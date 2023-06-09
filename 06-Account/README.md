# Bank Account - Pseudo Core
## Making it more real
So, my daughter walked in, and I demoed what we have so far, and she says, "Ok, and then you move money between accounts on that step". I replied, "Yes, we can pretend that, there is actually nothing happening". She was visibly disappointed, and I understand we have to introduce some kind of core functionality.

## Core components
* BankAccount - Entity transactions are executed towards, and keeps track of the current balances (available balance and booked balance)
* BankAccountContract - Entity that connects the account owner (PSU) with the BankAccount
* Transactions - Commmands and events that are used to execute transactions towards the BankAccount
* TransactionProcessor - The component that executes the transactions towards the BankAccount

## The BankAccount
The BankAccount is the entity that we will execute transactions towards. It is responsible for keeping track of the current balances (available balance and booked balance). It is also responsible for validating that the transaction is valid, and that the transaction is executed correctly. 

## The BankAccountContract
The BankAccountContract is the entity that connects the account owner (PSU) with the BankAccount. It is responsible for validating that the PSU is allowed to execute transactions towards the BankAccount. 

## Transactions
The transactions are the commands and events that are used to execute transactions towards the BankAccount. The transactions are used to communicate between the client and the BankAccount. The client sends a command to the BankAccount, and the BankAccount responds with an event. The client can then use the event to update the UI.

## TransactionProcessor
The TransactionProcessor is the component that executes the transactions towards the BankAccount. It is responsible for validating that the transaction is valid, and that the transaction is executed correctly. The TransactionProcessor is also responsible for publishing events when the balance changes.

## Connecting it with the API
The API is the component that connects the client with the core components. The API is responsible for validating that the PSU is allowed to execute transactions towards the BankAccount. The API is also responsible for executing the transactions towards the BankAccount.

### Endpoints
* GET /api/bankaccounts - Get all BankAccounts
* GET /api/bankaccounts/{bankAccountId} - Get a BankAccount
* POST /api/bankaccounts - Create a new BankAccount
* GET /api/bankaccounts/{bankAccountId}/status
* PUT /api/bankaccounts/{bankAccountId}/status
* GET /api/bankaccounts/{bankAccountId}/balances - Get the current balances of the BankAccount
* POST /api/bankaccounts/{bankAccountId}/transactions - Execute a transaction towards the BankAccount
* GET /api/bankaccounts/{bankAccountId}/transactions/{transactionId} - Get a transaction that has been executed towards the BankAccount
* GET /api/bankaccounts/{bankAccountId}/transactions - Get all transactions that has been executed towards the BankAccount
* POST /api/contracts/bankaccounts - Create a new BankAccountContract
* GET /api/contracts/bankaccounts/{bankAccountContractId} - Get a BankAccountContract
* GET /api/contracts/bankaccounts/{bankAccountContractId}/status
* PUT /api/contracts/bankaccounts/{bankAccountContractId}/status 