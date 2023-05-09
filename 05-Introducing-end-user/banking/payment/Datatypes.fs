namespace payment
open MongoDB.Bson.Serialization.Attributes

type PaymentRequest = 
    {   Amount: decimal
        Currency: string
        DebtorAccount: string
        CreditorAccount: string
        CreditorName: string }

type PaymentRequestData =
    {   [<BsonId>]
        PaymentId: string
        PaymentService: string
        PaymentProduct: string
        PaymentRequest: PaymentRequest }

type PaymentInitiationResponse =
    {   TransactionStatus: string
        PaymentId : string }

type PaymentInformationResponse =
    {   DebtorAccount: string
        InstructedAmount : string 
        CreditorAccount: string
        CreditorName: string }
