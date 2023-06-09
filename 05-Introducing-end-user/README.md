# Introducing end user

In this step we introduce the idea of an end user.

We are not yet introducing security, but the frontend will ask for an user id, which we pair with the PSU-ID we want to use towards the back end.

## Frontend

On the first page of the frontent we ask for the user ID. We will check with the bff if there is a profile associated with this user ID. If it is not, we will create a new profile.

On the new profile, we will ask for the PSU-ID. We will for the time being allow multiple profiles for the same PSU-ID.

The user id will be sent to the backend in the header `x-user-id`.

Step 1 of adding login, is to change the <BankingShell /> so it asks for an username. This user name is for now sent to the <Payments /> component, so it can be used in the <PaymentList /> and <PaymentForm /> to filter on the correct user.

For now we add the user to the header in the fetch() calls.

## Backend (BFF)

We will add services to the BFF to create and retrieve profiles. The users accounts will for now be stored directly in the profile.

First we extract the user id from the header. Later we will add authentication middleware, but we are keeping it simple for now.

## API

We will send the PSU-ID to the API in the header `PSU-ID`.

This will allow for the API to validate that the user is allowed to access the account.
