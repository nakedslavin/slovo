using System;
using ITB.Business.Repos;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace voiceapi.Models {

    public class Human : BaseMongoId {
        public string Name { get; set; }
        public string Email { get; set; }
        public string IP { get; set; }
    }

    public class Login : BaseMongoId {
        public string HumanName { get; set; }
        // [BsonRepresentation(BsonType.ObjectId)]
        public string HumanId { get; set; }
        public string IP { get; set; }
        public string UserAgent { get; set; }
    }
}