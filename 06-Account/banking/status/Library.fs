namespace status
open MongoDB.Driver
open System

module database =
    let collection =
        MongoClient(Config.connectionString)
            .GetDatabase(Config.databaseName)
            .GetCollection<Status>(Config.collectionName)

    let postStatus status =
        collection.InsertOne(status)
    
    let getStatus statusId =
        collection.Find(fun x -> x.StatusId = statusId).FirstOrDefault()

    let deleteStatus statusId =
        collection.DeleteOne(fun x -> x.StatusId = statusId)

    let updateStatus statusId status =
        let currentStatus = getStatus statusId
        let updatedPaymentStatus = 
            {   StatusId = statusId
                Status = status }
        printfn $"Updating Payment Status: {currentStatus} to {updatedPaymentStatus}"
        deleteStatus statusId |> ignore
        postStatus updatedPaymentStatus

module services =
    let createStatus statusId initialStatus=        
        {   StatusId = statusId
            Status = initialStatus }
        |> database.postStatus
        Ok { Status = initialStatus; StatusId = statusId }
    
    let getStatus statusId =        
        let status = database.getStatus statusId
        { Status = status.Status }
        |> Ok   
    
    let updateStatus statusId status =          
        database.updateStatus statusId status
        Ok {Status = status; StatusId = statusId }