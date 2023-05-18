namespace payment

open MongoDB.Driver
open System
open Zeebe.Client

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
    open System.Text.Json
    let zeebeClient = 
        ZeebeClient.Builder()
            .UseGatewayAddress(Config.zeebeAddress)
            .UsePlainText()
            .Build()
    
    let startPaymentInitiationProcess paymentService paymentProduct paymentId =
        let fromObject (data:obj) = 
            JsonSerializer.Serialize(data)
            
        printfn $"Start Payment Initiation Process: {paymentService} {paymentProduct} {paymentId}"
        zeebeClient.NewCreateProcessInstanceCommand()
            .BpmnProcessId("payment-initiation-process")
            .LatestVersion()
            .Variables(fromObject {| PaymentService = paymentService; PaymentProduct = paymentProduct; PaymentId = paymentId |})
            .Send()
            .Result
    let paymentInitiationRequest paymentService paymentProduct paymentRequest =
        printfn $"Payment Initiation Request: {paymentService} {paymentProduct} {paymentRequest}"
        let paymentId = Guid.NewGuid().ToString()
        {   PaymentId = paymentId
            PaymentService = paymentService
            PaymentProduct = paymentProduct
            PaymentRequest = paymentRequest }
        |> database.postRequest

        status.services.createStatus paymentId "PDNG"
        |> Result.map(fun statusResponse -> 
            startPaymentInitiationProcess paymentService paymentProduct paymentId |> ignore
            { TransactionStatus=statusResponse.Status; PaymentId = paymentId })

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
