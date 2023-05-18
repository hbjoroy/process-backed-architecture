namespace payment_initiation_process

module Config =
    type ZeebeConfig = {
        ZeebeAddress: string
        Diagram: string
    }
    type MongoConfig = {
        ConnectionString: string
        DatabaseName: string
        CollectionName: string
    }
    let zeebeConfig = {
        ZeebeAddress = "localhost:26500"
        Diagram = "PaymentInitiationProcess.bpmn"
    }

    let mongoConfig = {
        ConnectionString = "mongodb://localhost:27017"
        DatabaseName = "banking"
        CollectionName = "payments"
    }
