open NUnit.Framework
open Zeebe.Client.Api.Responses
open Zeebe.Client.Api.Worker

[<Test>]
let ``Periodic Payments Task Handler returns correct response`` () =
    // Arrange
    let job = { new IJob with
                    member this.Variables = """{ "PaymentService": "periodic-payments", "PaymentProduct": "norwegian-domestic-credit-transfer", "PaymentId": "123" }""" 
                    member this.Complete _ = null
                    member this.Fail _ = null
                    member this.ThrowError _ = null
                    member this.UpdateRetries _ = null
                    member this.Retries = 0
                    member this.Type = "periodic-payments-task"
                    member this.WorkflowInstanceKey = 123
                    member this.ElementId = "some-element-id"
                    member this.BpmnProcessId = "some-bpmn-process-id"
                    member this.CustomHeaders = null
                    member this.JobHeaders = null
                    member this.Timeout = null
                    member this.Worker = null
                    member this.VariablesAsMap = null
                    member this.VariablesAsObject = null
                    member this.VariablesAs<T>() = null
                    member this.VariablesAsJsonElement = null
            }

    // Act
    let result = Program.Handlers.periodicPaymentsTaskHandler job

    // Assert
    Assert.IsNotNull result
    Assert.AreEqual "ACCC" result.Value.TransactionStatus
    Assert.IsNull result.Value.Message