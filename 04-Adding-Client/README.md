# Adding a client
It is no fun with banking withour any clients that uses the banking APIs.

## More dependencies
You need NodeJs and  "create-react-app" to get started with this.

This guide will not go through how to do that, use public sources.

One solution to get "create-react-app" after installing NodeJs is to use
```
npm install -g create-react-app
```
and after
```
npx create-react-app banking
```

## The structure of the client
We will create a very simple payment client.
The outermost component in the App will be ```<BankingShell />``` inside of that, we will host ```<Payments />```

The Payments-components has a ```<PaymentForm />``` on the top, which expands when you click a button. When you click "Send payment" in that form, the payment is sent to the API through the its own back-end-for-frontend API (BFF) and backed by a MongoDB data collection "banking-bff". The payment order and some control data is stored in the data collection, and used as a source for the "Sent transactions"-list.

The "Sent transactions" displays some information from the "banking-bff"-collection.

Once every second, the component refreshes all payments that are not in the final state (ACCC for now) with the API.

## Other changes
The worker for the "update status" step in the workflow now actually stores the updated state.

## Trying out this step

The setup is becoming more complex, here is some tips to get it running.

The kubernetes cluster needs to be running, with camunda and mongodb, ref [prerequisites](../00-Prerequisite/)

A great way to do these steps, is to start VS Code from the [banking](./banking/) folder, and create four terminal windows using the "New Terminal" command. To inspect the code, you should install reccomended plugins, such as ionide to inspect f#-code.

* Start by starting the [API](./banking/api/). (```dotnet run```)
* At least once, start the [workers](./banking/payment-initiation-process/), to get the BPMN-workflow deployed. You can stop the workers, see below. (```dotnet run```)
* Start the [BFF](./banking/client/banking-bff/) (```dotnet run```)
* Start the [frontend] - Once run ```npm install```, then(```npm start```)

Register and send some payments.

To see that the workflow engine is doing its work, check [Camunda Operate](http://localhost:8081) - select the workflow, and click the box for "completed".

You can also stop the workers, register payments, see that the initial status of PDNG is not changing, then start the workers, and let Zeebe do its magic.
