namespace payment

module Config = 
    let connectionString="mongodb://localhost:27017"
    let databaseName="banking"
    let collectionName="payment"
    let zeebeAddress="localhost:26500"