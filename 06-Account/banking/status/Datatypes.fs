namespace status
open MongoDB.Bson.Serialization.Attributes

type Status = 
    {   Status: string
        [<BsonId>]
        StatusId : string }


type StatusResponse =
    {   Status: string }
