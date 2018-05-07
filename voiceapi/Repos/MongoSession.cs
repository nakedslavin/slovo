using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Security.Authentication;
using System.Text;

namespace ITB.Business.Repos
{
    // Database Name
    // Connection String
    // This is MongoDB representation of the db. This is the only coupling point with the db.
    public class MongoSession<T> where T : new()
    {
        private IMongoClient client;
        private IMongoCollection<T> collection;

        public MongoSession(string collectionName = null)
        {
            string connectionString =
   @"mongodb://writer:Slavin4254@cluster0-shard-00-00-qkhj5.mongodb.net:27017,cluster0-shard-00-01-qkhj5.mongodb.net:27017,cluster0-shard-00-02-qkhj5.mongodb.net:27017/osp?ssl=true&replicaSet=Cluster0-shard-0&authSource=admin";
            
            this.client = new MongoClient(connectionString);
            collectionName = collectionName ?? typeof(T).Name;
            this.collection = client.GetDatabase("osp")
                .GetCollection<T>(collectionName);
        }

        /// <summary>
        /// Gets data from the database
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns>
        /// Data of type <T>
        /// </returns>
        public List<T> Get(Expression<Func<T, bool>> predicate)
        {
            return collection.Find<T>(predicate).ToList();
        }

        /// <summary>
        /// Saves data to the database
        /// </summary>
        /// <param name="item"></param>
        public void Save(T item)
        {
            if (typeof(T).IsSubclassOf(typeof(BaseMongoId)))
            {
                BaseMongoId id = item as BaseMongoId;
                if (id.Id == null)
                    collection.InsertOne(item);
                else
                    collection.ReplaceOne(new BsonDocument("_id", ObjectId.Parse(id.Id)), item, new UpdateOptions { IsUpsert = true });
            }
            else
                collection.InsertOne(item);
        }

        /// <summary>
        /// Delete an item from the collection
        /// </summary>
        /// <param name="item"></param>
        public void Delete(BaseMongoId item)
        {
            collection.DeleteOne(new BsonDocument("_id", ObjectId.Parse(item.Id)));
        }
    }
    public class BaseMongoId
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
    }
}