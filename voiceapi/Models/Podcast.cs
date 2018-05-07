using System;
using System.Collections.Generic;
using ITB.Business.Repos;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace voiceapi.Models {
    public class Podcast : BaseMongoId {
        public string Title { get; set; }
        public string Link { get; set; } // original link
        public string Name { get; set; } // unique friendly name for the url. one can construct the link through {address}/name
        public string Description { get; set; }
        public string Host { get; set; }
        public string Copyright { get; set; }
        public string Author { get; set; }
        public string Category { get; set; }
        public List<string> Keywords { get; set; }
        public string Image { get; set; }
        public string PubDate { get; set; }
        public string Language { get; set; }
        public List<Item> Items { get; set; }

        public List<string> Podcasts { get; set; }

        public string HumanId { get; set; } // creator
        public string HumanName { get; set; } // creator
        public string HumanFullName { get; set; } // creator
        public DateTime CreationTimestamp { get; set; } // creation
        public DateTime BuildTimestamp { get; set; } // last generation
    }
    public class Item {
        public string Title { get; set; }
        public string EncloseUrl { get; set; }
        public string EncloseType { get; set; }
        public string Description { get; set; }
        public string Guid { get; set; }
        public string Link { get; set; }
        public string PubDate { get; set; }
        public string Image { get; set; }
        public string Duration { get; set; }
        public long Length { get; set; }
        public List<string> Keywords { get; set; }
        public string Explicit { get; set; }
        public string GuestName { get; set; }
        public string GuestDescription { get; set; }
    }

    public class EnclosureLink : BaseMongoId {
        public string ShortUrl { get; set; }
        public string RealUrl { get; set; }
    }
}