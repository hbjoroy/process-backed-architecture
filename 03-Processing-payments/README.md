# Processing payments

Ok. Now for the fun parts.

The software is now accepting payment requests and setting initial status. 

You can query the status, but it never changes.

The next bit will be to make payments happen.

We still want to do it very naively, without validating much, and also no kind of SCA.

## The process

1. First the system receives the payment, and sets the initial status
2. Next part is to accept the payment, and start processing
3. When it is processed, the status should be set to its final state

For the second part, the payment service is important, and also the product for that service.

## The tools

We will model the process using BPMN and Camunda/Zeebe.

## The workers


