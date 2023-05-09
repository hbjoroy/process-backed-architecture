namespace BankingBff

open System
open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.Hosting
open Microsoft.AspNetCore.Http
open System.Net.Http.Json
open MongoDB.Driver
open BankingBff.Datatypes

module PaymentApiClient =
    open System.Net.Http
    let updatePaymentStatus (payment:Datatypes.Payment) = 
        use client = new HttpClient()
        let response = client.GetAsync($"http://localhost:5158/v1/payments/norwegian-domestic-credit-transfer/{payment.PaymentID}/status").Result
        
        response.Content.ReadFromJsonAsync<Datatypes.ApiPaymentInitiationStatusResponse>().Result
        |> fun status -> 
            { payment with TransactionStatus = status.TransactionStatus }
           
    let createPayment(payment: Datatypes.DomesticPaymentRequest) = 
        use client = new HttpClient()
        
        {   Amount = payment.Amount
            Currency = payment.Currency
            DebtorAccount = payment.FromAccount
            CreditorAccount = payment.ToAccount
            CreditorName = payment.CreditorName }
        |> fun payment -> client.PostAsJsonAsync("http://localhost:5158/v1/payments/norwegian-domestic-credit-transfer", payment).Result
        |> fun response -> response.Content.ReadFromJsonAsync<Datatypes.ApiPaymentInitiationResponse>().Result

module Program = 
    [<EntryPoint>]
    let main args =
        let builder = WebApplication.CreateBuilder(args)
        let app = builder.Build()

        app.MapGet("/payments", Func<IResult>(fun () -> 
                let client = MongoClient("mongodb://localhost:27017")
                let db = client.GetDatabase("banking-bff")
                let collection = db.GetCollection<Datatypes.Payment>("payments")

                collection.Find(fun _ -> true).ToList()
                |> Seq.map (fun payment -> 
                    if payment.TransactionStatus <> "RJCT" then
                        let originalStatus = payment.TransactionStatus
                        let updatedPayment = PaymentApiClient.updatePaymentStatus payment

                        if originalStatus <> updatedPayment.TransactionStatus then
                            
                            let filter = Builders<Datatypes.Payment>.Filter.Eq((fun payment -> payment.PaymentID), payment.PaymentID)
                            let update = Builders<Datatypes.Payment>.Update.Set((fun payment -> payment.TransactionStatus), updatedPayment.TransactionStatus)
                            collection.UpdateOne(filter, update) |> ignore
                        
                        updatedPayment
                    else
                        payment)
                |> Results.Ok
            )) |> ignore

        app.MapPost("/payments", Func<HttpRequest, IResult>(fun (req) -> 
                let client = MongoClient("mongodb://localhost:27017")
                let db = client.GetDatabase("banking-bff")
                let collection = db.GetCollection<Payment>("payments")
                let payment = req.ReadFromJsonAsync<Datatypes.DomesticPaymentRequest>().Result
                let paymentResponse = PaymentApiClient.createPayment(payment)
                {   PaymentID = paymentResponse.PaymentId
                    PaymentRequest = payment 
                    TransactionStatus = paymentResponse.TransactionStatus}
                |> collection.InsertOne
                Results.Ok(paymentResponse.PaymentId)
            )) |> ignore
        app.Run()

        0 // Exit code