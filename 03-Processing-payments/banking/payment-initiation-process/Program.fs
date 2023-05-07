namespace payment_initiation_process
open Zeebe.Client
open Zeebe.Client.Api.Worker
open Zeebe.Client.Api.Responses
open System
open System.Text.Json

module Handlers =

    let serialize data =
        JsonSerializer.Serialize(data)
        
    let paymentsTaskHandler (client:IJobClient) (job: IJob) =
        printfn $"Payments Task Handler {job.Variables}"
        let nextStatus = {| TransactionStatus = "ACCC" |}
        client.NewCompleteJobCommand(job.Key)
            .Variables(serialize nextStatus)
            .Send()

    let bulkPaymentsTaskHandler (client:IJobClient) (job: IJob) =
        printfn $"Bulk Payments Task Handler {job.Variables}"
        let nextStatus = {| TransactionStatus = "ACCC" |}
        client.NewCompleteJobCommand(job.Key)            
            .Variables(serialize nextStatus)
            .Send()

    let periodicPaymentsTaskHandler (client:IJobClient) (job: IJob) =
        printfn $"Periodic Payments Task Handler {job.Variables}"
        let nextStatus = {| TransactionStatus = "ACCC" |}
        client.NewCompleteJobCommand(job.Key)
            .Variables(serialize nextStatus)
            .Send()

    let invalidPaymentServiceTaskHandler (client:IJobClient) (job: IJob) =
        printfn $"Invalid Payment Service Task Handler {job.Variables}"
        let nextStatus = {| TransactionStatus = "RJCT" |}
        client.NewCompleteJobCommand(job.Key)   
            .Variables(serialize nextStatus)
            .Send()
    let updateStatusTaskHandler (client:IJobClient) (job: IJob) =
        printfn $"Update Status Task Handler {job.Variables}"
        client.NewCompleteJobCommand(job.Key).Send()

    let handlers = [
        "payments-task", paymentsTaskHandler
        "bulk-payments-task", bulkPaymentsTaskHandler
        "periodic-payments-task", periodicPaymentsTaskHandler
        "invalid-payment-service-task", invalidPaymentServiceTaskHandler
        "update-status-task", updateStatusTaskHandler
    ]
module Program =
    [<EntryPoint>]
    let main argv =
        printfn "Payment Initiation Process Workers"
        let zeebeClient = 
            ZeebeClient.Builder()
                .UseGatewayAddress(Config.zeebeAddress)
                .UsePlainText()
                .Build()
                
        zeebeClient.TopologyRequest().Send().Result
        |> printfn "Topology: %A" 

        zeebeClient.NewDeployCommand()
            .AddResourceFile("PaymentInitiationProcess.bpmn")
            .Send()
            .Result
        |> fun workflow -> workflow.Processes.[0].Version
        |> printfn "Workflow deployed. Version: %d" 

        Handlers.handlers
        |> List.map (fun (jobType, handler) -> 
            zeebeClient.NewWorker()
                .JobType(jobType)
                .Handler(fun client job -> handler client job |> ignore)
                .MaxJobsActive(5)
                .Name(jobType)
                .PollInterval(TimeSpan.FromSeconds(1))
                .Timeout(TimeSpan.FromSeconds(10))
                .Open() |> ignore
            printfn "Worker for %s started" jobType
        ) |> ignore

        printfn  "Press any key to exit ... "
        Console.ReadKey() |> ignore
        0 // return an integer exit code

