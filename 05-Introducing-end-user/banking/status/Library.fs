namespace status
open MongoDB.Driver
open System

module database =
    let collection =
        MongoClient(Config.connectionString)
            .GetDatabase(Config.databaseName)
            .GetCollection<PaymentStatus>(Config.collectionName)

    let postStatus paymentStatus =
        collection.InsertOne(paymentStatus)
    
    let getStatus paymentId =
        collection.Find(fun x -> x.PaymentId = paymentId).FirstOrDefault()

    let deleteStatus paymentId =
        collection.DeleteOne(fun x -> x.PaymentId = paymentId)

    let updateStatus paymentId transactionStatus =
        let paymentStatus = getStatus paymentId
        let updatedPaymentStatus = 
            {   PaymentId = paymentId
                TransactionStatus = transactionStatus }
        printfn $"Updating Payment Status: {paymentStatus} to {updatedPaymentStatus}"
        deleteStatus paymentId |> ignore
        postStatus updatedPaymentStatus

module services =
    let createPaymentInitiationStatus paymentService paymentProduct paymentId=
        printfn $"Payment Initiation Status Request: {paymentService} {paymentProduct} {paymentId}"        
        {   PaymentId = paymentId
            TransactionStatus = "PDNG" }
        |> database.postStatus
        Ok {TransactionStatus = "PDNG"; PaymentId = paymentId }
    
    let getPaymentInitiationStatus paymentService paymentProduct paymentId =
        printfn $"Get Payment Initiation Status Request: {paymentService} {paymentProduct} {paymentId}"
        let paymentRequest = database.getStatus paymentId

        {   TransactionStatus = paymentRequest.TransactionStatus }
        |> Ok   
    
    let updatePaymentInitiationStatus paymentId transactionStatus =  
        printfn $"Update Payment Initiation Status Request: {paymentId} {transactionStatus}"
        database.updateStatus paymentId transactionStatus
        Ok {TransactionStatus = transactionStatus; PaymentId = paymentId }