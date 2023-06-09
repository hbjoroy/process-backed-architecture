namespace BankingBff.Datatypes
open MongoDB.Bson.Serialization.Attributes
open System.Text.Json.Serialization

type DomesticPaymentRequest = 
    {
        Amount: decimal
        Currency: string
        FromAccount: string
        ToAccount: string
        CreditorName: string
    }

type Payment = 
    {
        [<BsonId>]
        PaymentID: string
        UserId: string
        PaymentRequest: DomesticPaymentRequest
        TransactionStatus: string
    }

type ApiPaymentRequest = 
    {   Amount: decimal
        Currency: string
        DebtorAccount: string
        CreditorAccount: string
        CreditorName: string }

type ApiPaymentInitiationResponse =
    {   TransactionStatus: string
        PaymentId : string }

type ApiPaymentInitiationStatusResponse =
    {   TransactionStatus: string }

type Profile = 
    {   [<BsonId>]
        UserId: string
        PsuId: string
        Name: string        
        Accounts: string ResizeArray } // Trouble deserializing string list
