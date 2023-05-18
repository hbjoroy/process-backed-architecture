open System
open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.Hosting
open Microsoft.AspNetCore.Http
open payment

module services =
    let mapResult f = function
        | Ok x -> f x
        | Error e -> Results.BadRequest(e)

module paymentInitiation =
    open services
    let paymentInitiationRequest paymentService paymentProduct paymentRequest =
        payment.services.paymentInitiationRequest paymentService paymentProduct paymentRequest
        |> mapResult Results.Ok

    let getPaymentInformationRequest paymentService paymentProduct paymentId =
        payment.services.getPaymentInformationRequest paymentService paymentProduct paymentId
        |> mapResult Results.Ok

    let paymentCancellationRequest paymentService paymentProduct paymentId =
        payment.services.paymentCancellationRequest paymentService paymentProduct paymentId
        |> mapResult Results.NoContent

    let paymentInitiationStatusRequest paymentService paymentProduct paymentId =
        let mapStatusResponse (status:status.StatusResponse) = 
            { TransactionStatus=status.Status; PaymentId = paymentId }
            
        status.services.getStatus paymentId
        |> mapResult (mapStatusResponse >> Results.Ok)

open paymentInitiation
[<EntryPoint>]
let main args =
    
    let builder = 
        WebApplication.CreateBuilder(args)

    let app = builder.Build()
    
    app.MapPost("/v1/{paymentService}/{paymentProduct}", 
        Func<string,string,PaymentRequest, IResult>(
            fun paymentService paymentProduct paymentRequest-> 
                paymentInitiationRequest paymentService paymentProduct paymentRequest)) |> ignore

    app.MapGet("/v1/{paymentService}/{paymentProduct}/{paymentId}", 
        Func<string,string,string, IResult>(
            fun paymentService paymentProduct paymentId -> 
                getPaymentInformationRequest paymentService paymentProduct paymentId )) |> ignore
    
    app.MapDelete("/v1/{paymentService}/{paymentProduct}/{paymentId}", 
        Func<string,string,string, IResult>(
            fun paymentService paymentProduct paymentId -> 
                paymentCancellationRequest paymentService paymentProduct paymentId)) |> ignore

    app.MapGet("/v1/{paymentService}/{paymentProduct}/{paymentId}/status", 
        Func<string,string,string, IResult>(
            fun paymentService paymentProduct paymentId -> 
                paymentInitiationStatusRequest paymentService paymentProduct paymentId )) |> ignore
    
    app.Run()

    0 // Exit code

