namespace payment

open MongoDB.Driver
open System

module database =
    let collection =
        MongoClient(Config.connectionString)
            .GetDatabase(Config.databaseName)
            .GetCollection<PaymentRequestData>(Config.collectionName)

    let postRequest paymentRequest =
        collection.InsertOne(paymentRequest)
    
    let getRequest paymentId =
        collection.Find(fun x -> x.PaymentId = paymentId).FirstOrDefault().PaymentRequest

    let deleteRequest paymentId =
        collection.DeleteOne(fun x -> x.PaymentId = paymentId)

module services = 
    let paymentInitiationRequest paymentService paymentProduct paymentRequest =
        printfn $"Payment Initiation Request: {paymentService} {paymentProduct} {paymentRequest}"
        let paymentId = Guid.NewGuid().ToString()
        {   PaymentId = paymentId
            PaymentService = paymentService
            PaymentProduct = paymentProduct
            PaymentRequest = paymentRequest }
        |> database.postRequest

        status.services.createPaymentInitiationStatus paymentService paymentProduct paymentId
        |> Result.map(fun statusResponse -> 
            { TransactionStatus=statusResponse.TransactionStatus; PaymentId = paymentId })

    let getPaymentInformationRequest paymentService paymentProduct paymentId =
        printfn $"Get Payment Information Request: {paymentService} {paymentProduct} {paymentId}"
        let paymentRequest = database.getRequest paymentId

        {   DebtorAccount = paymentRequest.DebtorAccount
            InstructedAmount = $"{paymentRequest.Currency} {paymentRequest.Amount}"
            CreditorAccount = paymentRequest.CreditorAccount
            CreditorName = paymentRequest.CreditorName }
        |> Ok

    let paymentCancellationRequest paymentService paymentProduct paymentId =
        printfn $"Payment Cancellation Request: {paymentService} {paymentProduct} {paymentId}"
        database.deleteRequest paymentId
        |> fun deleteResult -> 
            if deleteResult.DeletedCount = 0L then
                Error "Payment not found"
            else
                Ok ()   
