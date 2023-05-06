open System
open Zeebe.Client
open Zeebe.Client.Api.Responses
open System.Text.Json
open MongoDB.Bson.Serialization.Attributes
open MongoDB.Driver
open Zeebe.Client.Api.Worker

type HelloWorld =
    { [<BsonId>] JobId: int64
      ProcessInstanceKey: int64
      OrderTime: DateTime
      ProcessTime: DateTime }

[<EntryPoint>]

printfn "Zeebe F# Client Example"

//let mongoClient = MongoClient("mongodb://root:EGq1Rxh3nF@localhost:27017")
let mongoClient = MongoClient("mongodb://localhost:27017")
let database = mongoClient.GetDatabase("hello-world")
let collection = database.GetCollection<HelloWorld>("helloCollection")

let zeebeClient = 
    ZeebeClient.Builder()
        .UseGatewayAddress("localhost:26500")
        .UsePlainText()
        .Build()

// Deploy workflow
let workflow = 
    zeebeClient.NewDeployCommand()
        .AddResourceFile("hello-world.bpmn")
        .Send()
        .Result

workflow.Processes.[0].Version
|> printfn "Workflow deployed. Version: %d" 

// Create workflow instance 
let variables = {| Time = DateTime.Now|}
let workflowInstance = 
    zeebeClient.NewCreateProcessInstanceCommand()
        .BpmnProcessId("HelloWorldProcess")
        .LatestVersion()
        .Variables(JsonSerializer.Serialize(variables))
        .Send()
        .Result
// Define the handler to be used by the worker
let helloWorldHandler (client:IJobClient) (job:IJob) = 
    printfn "Hello from F#! %A" job.Variables

    let element:JsonElement = JsonSerializer.Deserialize(job.Variables)
    
    let time = element.GetProperty("Time").GetDateTime()

    {   JobId = job.Key
        ProcessInstanceKey=workflowInstance.ProcessInstanceKey
        OrderTime = time
        ProcessTime = DateTime.Now }
    |> collection.InsertOne

    client.NewCompleteJobCommand(job.Key).Send() |> ignore
    
// Create worker - listens for jobs on the task type "helloWorldTask"
let worker = 
    zeebeClient.NewWorker()
        .JobType("helloWorldTask")
        .Handler(helloWorldHandler)
        .MaxJobsActive(5)
        .Name("F# Worker")
        .PollInterval(TimeSpan.FromSeconds(1.0))
        .Timeout(TimeSpan.FromSeconds(10.0))
        .Open()

// Keep the worker running until key press
printfn  "Press any key to exit (wait until worker has processed the task)... "
Console.ReadKey() |> ignore

// Confirm that the data is in the database
collection.Find(fun element -> element.ProcessInstanceKey=workflowInstance.ProcessInstanceKey).ToList()
|> printfn "\nRetrieved from database:\n %A"