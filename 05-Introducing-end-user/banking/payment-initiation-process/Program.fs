namespace payment_initiation_process
open Zeebe.Client
open Zeebe.Client.Api.Worker
open Zeebe.Client.Api.Responses
open System
open System.Text.Json
open MongoDB.Driver

module Handlers =
    let paymentsTaskHandler (job: IJob) =
        printfn $"Payments Task Handler {job.Variables}"
        Some {| TransactionStatus = "ACCC" |}

    let bulkPaymentsTaskHandler (job: IJob) =
        printfn $"Bulk Payments Task Handler {job.Variables}"
        Some {| TransactionStatus = "ACCC" |}

    let periodicPaymentsTaskHandler (job: IJob) =
        printfn $"Periodic Payments Task Handler {job.Variables}"
        Some {| TransactionStatus = "ACCC" |}

    let invalidPaymentServiceTaskHandler (job: IJob) =
        printfn $"Invalid Payment Service Task Handler {job.Variables}"
        Some {| TransactionStatus = "RJCT" |}

    let updateStatusTaskHandler (job: IJob) =
        printfn $"Update Status Task Handler {job.Variables}"
        let paymentId, transactionStatus = 
            job.Variables
            |> JsonDocument.Parse
            |> fun json -> 
                json.RootElement.GetProperty("PaymentId").GetString(),
                json.RootElement.GetProperty("TransactionStatus").GetString()

        status.services.updatePaymentInitiationStatus paymentId transactionStatus |> ignore
        
        None

    let handlers = [
        "payments-task", paymentsTaskHandler
        "bulk-payments-task", bulkPaymentsTaskHandler
        "periodic-payments-task", periodicPaymentsTaskHandler
        "invalid-payment-service-task", invalidPaymentServiceTaskHandler
        "update-status-task", updateStatusTaskHandler
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

