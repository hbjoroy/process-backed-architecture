open System
open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.Hosting
open Microsoft.AspNetCore.Http

// Start with very simple request and response types
type PaymentRequest = 
    {   Amount: decimal
        Currency: string
        DebtorAccount: string
        CreditorAccount: string }
type PaymentInitiationResponse =
    {   TransactionStatus: string
        PaymentId : string }
type PaymentInformationResponse =
    {   DebtorAccount: string
        InstructedAmount : string 
        CreditorAccount: string
        CreditorName: string }
type PaymentInitiationStatusResponse =
    {   TransactionStatus: string }

module services =
    let paymentInitiationRequest paymentService paymentProduct paymentRequest =
        printfn $"Payment Initiation Request: {paymentService} {paymentProduct} {paymentRequest}"
        Results.Ok({TransactionStatus = "PDNG"; PaymentId = "123456789"})

    let getPaymentInformationRequest paymentService paymentProduct paymentId =
        printfn $"Get Payment Information Request: {paymentService} {paymentProduct} {paymentId}"
        Results.Ok({DebtorAccount = "NL02ABNA0123456789"; InstructedAmount = "EUR 100.00"; CreditorAccount = "NL02ABNA0123456789"; CreditorName = "John Doe"})

    let paymentCancellationRequest paymentService paymentProduct paymentId =
        printfn $"Delete Payment Request: {paymentService} {paymentProduct} {paymentId}"
        Results.NoContent()

    let paymentInitiationStatusRequest paymentService paymentProduct paymentRequest =
        printfn $"Payment Initiation Status Request: {paymentService} {paymentProduct} {paymentRequest}"
        Results.Ok({TransactionStatus = "ACCP"})

[<EntryPoint>]
let main args =
    let builder = 
        WebApplication.CreateBuilder(args)

    let app = builder.Build()
    
    app.MapPost("/v1/{paymentService}/{paymentProduct}", 
        Func<string,string,PaymentRequest, IResult>(
            fun paymentService paymentProduct paymentRequest-> 
                services.paymentInitiationRequest paymentService paymentProduct paymentRequest)) |> ignore

    app.MapGet("/v1/{paymentService}/{paymentProduct}/{paymentId}", 
        Func<string,string,string, IResult>(
            fun paymentService paymentProduct paymentId -> 
                services.getPaymentInformationRequest paymentService paymentProduct paymentId )) |> ignore
    
    app.MapDelete("/v1/{paymentService}/{paymentProduct}/{paymentId}", 
        Func<string,string,string, IResult>(
            fun paymentService paymentProduct paymentId -> 
                services.paymentCancellationRequest paymentService paymentProduct paymentId)) |> ignore

    app.MapGet("/v1/{paymentService}/{paymentProduct}/{paymentId}/status", 
        Func<string,string,string, IResult>(
            fun paymentService paymentProduct paymentId -> 
                services.paymentInitiationStatusRequest paymentService paymentProduct paymentId )) |> ignore
    
    
    app.Run()

    0 // Exit code

