namespace BankingBff.Datatypes
open MongoDB.Bson.Serialization.Attributes

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