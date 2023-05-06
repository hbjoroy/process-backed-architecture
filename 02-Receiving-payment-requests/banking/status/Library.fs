namespace status
open MongoDB.Driver
open System

module database =
    let collection =
        MongoClient(Config.connectionString)
            .GetDatabase(Config.databaseName)
            .GetCollection<PaymentStatus>(Config.collectionName)

    let postRequest paymentStatus =
        collection.InsertOne(paymentStatus)
    
    let getRequest paymentId =
        collection.Find(fun x -> x.PaymentId = paymentId).FirstOrDefault()

    let deleteRequest paymentId =
        collection.DeleteOne(fun x -> x.PaymentId = paymentId)

module services =
    let createPaymentInitiationStatus paymentService paymentProduct paymentId=
        printfn $"Payment Initiation Status Request: {paymentService} {paymentProduct} {paymentId}"        
        {   PaymentId = paymentId
            TransactionStatus = "PDNG" }
        |> database.postRequest
        Ok {TransactionStatus = "PDNG"; PaymentId = paymentId }
    
    let getPaymentInitiationStatus paymentService paymentProduct paymentId =
        printfn $"Get Payment Initiation Status Request: {paymentService} {paymentProduct} {paymentId}"
        let paymentRequest = database.getRequest paymentId

        {   TransactionStatus = paymentRequest.TransactionStatus }
        |> Ok   