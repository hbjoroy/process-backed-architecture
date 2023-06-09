namespace payment_initiation_process
open Zeebe.Client
open Zeebe.Client.Api.Worker
open Zeebe.Client.Api.Responses
open System
open System.Text.Json
open MongoDB.Driver
open payment
module ProcessSupport =
    let paymentVariables (variables:string) =
        variables
        |> JsonDocument.Parse
        |> fun json -> 
            json.RootElement.GetProperty("PaymentService").GetString(),
            json.RootElement.GetProperty("PaymentProduct").GetString(),
            json.RootElement.GetProperty("PaymentId").GetString()

module Handlers =
    let paymentsTaskHandler (job: IJob) =
        printfn $"Payments Task Handler {job.Variables}"
        Some {| TransactionStatus = "ACCC"; Message=None |}

    let bulkPaymentsTaskHandler (job: IJob) =
        printfn $"Bulk Payments Task Handler {job.Variables}"
        Some {| TransactionStatus = "ACCC"; Message=None |}

    let periodicPaymentsTaskHandler (job: IJob) =
        printfn $"Periodic Payments Task Handler {job.Variables}"
        Some {| TransactionStatus = "ACCC"; Message=None |}

    let invalidPaymentServiceTaskHandler (job: IJob) =
        printfn $"Invalid Payment Service Task Handler {job.Variables}"
        Some {| TransactionStatus = "RJCT"; Message=None |}
    let verifyOrder (job:IJob) = 
        printf $"Verify Order {job.Variables}"
        let paymentService, paymentProduct, paymentId = 
            job.Variables
            |> ProcessSupport.paymentVariables

//        let request = services.getPaymentInformationRequest paymentService paymentProduct paymentId
            
        let serviceOk = 
            match paymentService with
            | "payments" -> Ok None
            | "bulk-payments" -> Ok None
            | "periodic-payments" -> Ok None
            | _ -> Error (Some "Payment serivce not supported, must be one of payments, bulk-payments, periodic-payments")

        let productOk = 
            match paymentProduct with
            | "norwegian-domestic-credit-transfer" -> Ok None
            | _ -> Error (Some "Payment product not supported, must be norwegian-domestic-credit-transfer")

        match serviceOk, productOk with
        | Ok _, Ok _ -> 
            Some {| TransactionStatus = "ACTC"; Message = None |}
        | Error (Some service), Error (Some product) -> 
            Some {| TransactionStatus = "RJCT"; Message = Some $"{service} {product}" |}
        | Error (Some e), _ -> 
            Some {| TransactionStatus = "RJCT"; Message = Some e |}
        | _, Error (Some e) ->
            Some {| TransactionStatus = "RJCT"; Message = Some e |}
        | _ -> Some {| TransactionStatus = "RJCT"; Message = Some "Unknown error" |}

    let updateStatusTaskHandler (job: IJob) =
        printfn $"Update Status Task Handler {job.Variables}"
        let paymentId, transactionStatus = 
            job.Variables
            |> JsonDocument.Parse
            |> fun json -> 
                json.RootElement.GetProperty("PaymentId").GetString(),
                json.RootElement.GetProperty("TransactionStatus").GetString()

        status.services.updateStatus paymentId transactionStatus |> ignore
        
        None 

    let handlers = [
        "payments-task", paymentsTaskHandler
        "bulk-payments-task", bulkPaymentsTaskHandler
        "periodic-payments-task", periodicPaymentsTaskHandler
        "invalid-payment-service-task", invalidPaymentServiceTaskHandler
        "update-status-task", updateStatusTaskHandler
        "verify-order", verifyOrder
    ]

module ZeebeWorker =
    let mutable storedZeebeClient:IZeebeClient option = None

    let zeebeClient () =
        match storedZeebeClient with
        | Some client -> client
        | None -> failwith "Zeebe Client not initialized"

    let initialize (zeebeConfig:Config.ZeebeConfig) =
        storedZeebeClient <- Some ( 
            ZeebeClient.Builder()
                .UseGatewayAddress(zeebeConfig.ZeebeAddress)
                .UsePlainText()
                .Build())

        zeebeClient().TopologyRequest().Send().Result
        |> printfn "Topology: %A" 

        zeebeClient().NewDeployCommand()
            .AddResourceFile(zeebeConfig.Diagram)
            .Send()
            .Result
        |> fun workflow -> workflow.Processes.[0].Version
        |> printfn "Workflow deployed. Version: %d" 

    let handleTask (client:IJobClient) (job:IJob) handler =
        handler job
        |> function 
            | Some variables ->
                client.NewCompleteJobCommand(job.Key)
                    .Variables(JsonSerializer.Serialize(variables))
                    .Send()
            | None -> client.NewCompleteJobCommand(job.Key).Send()
        |> ignore

    let startWorkers handlers = 
        handlers
        |> List.map (fun (jobType, handler) -> 
            zeebeClient().NewWorker()
                .JobType(jobType)
                .Handler(fun client job -> handleTask client job handler)
                .MaxJobsActive(5)
                .Name(jobType)
                .PollInterval(TimeSpan.FromSeconds(1))
                .Timeout(TimeSpan.FromSeconds(10))
                .Open() |> ignore
            printfn "Worker for %s started" jobType
        ) |> ignore

module Program =
    [<EntryPoint>]
    let main argv =
        printfn "Payment Initiation Process Workers"

        ZeebeWorker.initialize Config.zeebeConfig

        Handlers.handlers
        |> ZeebeWorker.startWorkers 

        printfn  "Press any key to exit ... "
        Console.ReadKey() |> ignore
        0 // return an integer exit code

