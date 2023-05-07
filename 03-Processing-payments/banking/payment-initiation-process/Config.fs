namespace payment_initiation_process

module Config =
    type ZeebeConfig = {
//        ConnectionString: string
//        DatabaseName: string
//        CollectionName: string
        ZeebeAddress: string
        Diagram: string
    }

    let zeebeConfig = {
//        ConnectionString = "mongodb://localhost:27017"
//        DatabaseName = "banking"
//        CollectionName = "payment"
        ZeebeAddress = "localhost:26500"
        Diagram = "PaymentInitiationProcess.bpmn"
    }
