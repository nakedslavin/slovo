using System;
using System.Collections.Generic;
using ITB.Business.Repos;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace voiceapi.Models {
    public class Stats : BaseMongoId
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string PodcastId { get; set; }
        public string PodcastName { get;set; }
        public DateTime Timestamp { get; set; }
        public int TotalSubscribersCount { get;set; } // including syndicates
        public int UniqueSubscribersCount { get; set; } // excluding syndicates
        public int SharesCount { get; set; } // recommend to someone
        public int ViewsCount { get; set; } // people viewd the page with the show
        public int ListenersCount { get; set; } // people tried to listen
    }
}