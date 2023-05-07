namespace status
open MongoDB.Bson.Serialization.Attributes

type PaymentStatus = 
    {   TransactionStatus: string
        [<BsonId>]
        PaymentId : string }


type PaymentInitiationStatusResponse =
    {   TransactionStatus: string }
