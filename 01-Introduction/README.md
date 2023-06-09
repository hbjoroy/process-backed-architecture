# Introduction

This series aims to showcase an architecture model that in a tidy way separates the APIs of a system from the services needed by those APIs, orchestrated by a process engine.

[Prerequisites](../00-Prerequisite/README.md)

## Use case

We want to create a simplified payment API, inspired by the Berlin Group NextGenPSD2. [Here](https://openbankingproject-ch.github.io/obp-apis/berlin-group.html#tag/Payment-Initiation-Service-(PIS)) you can browse a version of this specification.

## Components

The main components of the API is:
* Payment services
* Status service
* Authorization service

For the process the main processes are
* Payment Initiation
* Payment cancellation
* Authorization
* Strong Customer Authentication (SCA)

The processes themselves will introduce several needed back end services supporting the processes.

We also need to create a simple payment client

## The philosophy

The apis will work with a REST mindset, and will be supported by a data store. When data gets posted, the order will be stored in a data store, and typically a supporting process will be started.

Initially, the processes will create all other data objects needed. The only data directly created as a side effect, is the status-object when a payment is submitted.

The call will return immediately, with reference to the created object.

By polling the status service, the client may take action or inform the user.

# Initial code 

[Here](./banking/) you can find the initial source code.

## The api
in the folder ```api``` you will find the code that receives the REST calls from the client.

It is a minimal API coded in F#.

It routes the calls to the endpoints to the respective services.

For this first introduction we only map payment initiation and status, and only "locally".

There is a powershell scripts that call the API.