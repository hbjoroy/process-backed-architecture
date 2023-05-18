namespace bankaccounts
open MongoDB.Driver
open MongoDB.Bson

module services =
    let client = new MongoClient(Config.connectionString)
    let database = client.GetDatabase(Config.databaseName)
    let collection = database.GetCollection<BankAccount>(Config.collectionName)

    let deleteAccount (accountId:string) =
        query {
            for account in collection.AsQueryable() do
            where (account.AccountId = accountId)            
        } |> Seq.tryHead
        |> Option.map (fun x -> collection.DeleteOne(BsonDocument("AccountId", x.AccountId)))

    let createAccount (account:BankAccount) : BankAccount option=
        try
            collection.InsertOne(account) |> ignore
            status.services.createStatus account.AccountId "created"
            |> function
                | Ok _ -> Some account
                | Error _ -> 
                    collection.DeleteOne(BsonDocument("AccountId", account.AccountId)) |> ignore
                    None
        with
            | ex -> 
                printfn "Error: %s" ex.Message
                None

    
    let getAccount (accountId:string) =
        query {
            for account in collection.AsQueryable() do
            where (account.AccountId = accountId)
            select account
        } |> Seq.tryHead
        
    let updateAccount (account:BankAccount) =
        query {
            for account in collection.AsQueryable() do
            where (account.AccountId = account.AccountId)            
        } |> Seq.tryHead
        |> Option.map (fun x -> collection.ReplaceOne(BsonDocument("AccountId", x.AccountId), account))


