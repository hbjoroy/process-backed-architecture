namespace BankingBff

open System
open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.Hosting
open Microsoft.AspNetCore.Http
open System.Net.Http.Json
open MongoDB.Driver
open BankingBff.Datatypes
open System.Net.Http

module PaymentApiClient =
    let updatePaymentStatus (payment:Datatypes.Payment) (psuId:string) = 
        use client = new HttpClient()
        client.DefaultRequestHeaders.Add("PSU-ID", psuId)

        let response = client.GetAsync($"http://localhost:5158/v1/payments/norwegian-domestic-credit-transfer/{payment.PaymentID}/status").Result
        
        response.Content.ReadFromJsonAsync<Datatypes.ApiPaymentInitiationStatusResponse>().Result
        |> fun status -> 
            { payment with TransactionStatus = status.TransactionStatus }

    let createPayment(payment: Datatypes.DomesticPaymentRequest) (psuId:string) = 
        use client = new HttpClient()
        client.DefaultRequestHeaders.Add("PSU-ID", psuId)

        {   Amount = payment.Amount
            Currency = payment.Currency
            DebtorAccount = payment.FromAccount
            CreditorAccount = payment.ToAccount
            CreditorName = payment.CreditorName }
        |> fun payment -> client.PostAsJsonAsync("http://localhost:5158/v1/payments/norwegian-domestic-credit-transfer", payment).Result
        |> fun response -> response.Content.ReadFromJsonAsync<Datatypes.ApiPaymentInitiationResponse>().Result

module ProfileDbClient =
    let getProfile (userId:string) = 
        let client = MongoClient("mongodb://localhost:27017")
        let db = client.GetDatabase("banking-bff")                                
        db.GetCollection<Datatypes.Profile>("profiles").AsQueryable()
        |> Seq.tryFind (fun profile -> profile.UserId = userId)


module Program = 
    [<EntryPoint>]
    let main args =
        let builder = WebApplication.CreateBuilder(args)
        let app = builder.Build()

        app.MapGet("/profile", Func<HttpRequest, IResult>(fun (req) -> 
                let userId = req.Headers.["x-user-id"].ToString()
                printfn "User: %s" userId
                ProfileDbClient.getProfile userId
                |> function
                    | None -> Results.Problem( "User profile not found", statusCode = 404)
                    | Some profile -> Results.Ok(profile)

            )) |> ignore

        app.MapPost("/profile", Func<HttpRequest, IResult>(fun (req) -> 
                let client = MongoClient("mongodb://localhost:27017")
                let db = client.GetDatabase("banking-bff")
                let collection = db.GetCollection<Datatypes.Profile>("profiles")
                let profile = req.ReadFromJsonAsync<Datatypes.Profile>().Result
                collection.InsertOne(profile)
                Results.Ok(profile)
            )) |> ignore
        app.MapGet("/payments", Func<HttpRequest, IResult>(fun (req) -> 
                let client = MongoClient("mongodb://localhost:27017")
                let db = client.GetDatabase("banking-bff")
                let collection = db.GetCollection<Datatypes.Payment>("payments")
                req.Headers.["x-user-id"].ToString()
                |> ProfileDbClient.getProfile
                |> function
                    | None -> Results.Problem( "Profile not found", statusCode = 404) 
                    | Some profile -> 
                        query {
                            for payment in collection.AsQueryable() do
                            where (payment.UserId = profile.UserId)
                            select payment
                        }
                        |> function
                            | payments when payments |> Seq.isEmpty
                                -> Results.Problem( "No payments found", statusCode = 404) 
                            | payments -> 
                                payments
                                |> Seq.map (fun payment -> 
                                    if payment.TransactionStatus <> "RJCT" then
                                        let originalStatus = payment.TransactionStatus
                                        let updatedPayment = PaymentApiClient.updatePaymentStatus payment profile.PsuId

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
                req.Headers.["x-user-id"].ToString()
                |> ProfileDbClient.getProfile
                |> function
                    | None -> Results.Problem( "User profile not found", statusCode = 400)
                    | Some userProfile ->
                        let collection = db.GetCollection<Payment>("payments")
                        let payment = req.ReadFromJsonAsync<Datatypes.DomesticPaymentRequest>().Result
                        let paymentResponse = PaymentApiClient.createPayment payment userProfile.PsuId
                        {   PaymentID = paymentResponse.PaymentId
                            UserId = userProfile.UserId
                            PaymentRequest = payment 
                            TransactionStatus = paymentResponse.TransactionStatus}
                        |> collection.InsertOne
                        Results.Ok(paymentResponse.PaymentId)
            )) |> ignore
        app.Run()

        0 // Exit code