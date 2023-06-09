# Receiving payment requests
At this step we receive and store the payment request.

We also implement requests to retrieve payment information and status information.

We also now need a client, and we start planning that.

## Implementing payment service

Looking into the Payment service, we implement Create, Read and Delete for supporting the APIs in the separate library project.

After implementing the status service we can also create the initial status record for the payment.

### Limitations

We do no sort of sanity validation, user validation. Neither do we delete the status when deleting the payment.

Deleting payments should only be legal when the payment is in certain statuses, but for now we don't care.

The most important limitation of course, is that nothing really happens when posting these requests. You get the initial status but nothing happens.

Also, for now the organisation of code is naive, Config is hardcoded, and there is no efforts to make the code testable.

## Implementing the status service

For this service we implement a create function which is not exposed to the APIs, but the get function is used by the APIs. 

Both the API project and the Payment project need to reference the status service, since the API project will expose the status API, and the Payment service needs to create the initial status.
