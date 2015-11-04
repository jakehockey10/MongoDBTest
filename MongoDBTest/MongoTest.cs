using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MongoDBTest
{
    public class MongoTest
    {
        private static readonly string _url =
            "https://raw.githubusercontent.com/mongodb/docs-assets/primer-dataset/dataset.json";

        private static readonly string _destination =
            @"c:\users\jakeh\documents\visual studio 2015\Projects\MongoDBTest\dataset.json";

        protected static IMongoClient _client;

        internal IMongoCollection<T> GetCollection<T>(string name)
        {
            return _database.GetCollection<T>(name);
        }

        protected static IMongoDatabase _database;

        public MongoTest()
        {
            ConnectToTestDB();
        }

        public async Task<int> CountRestaurants()
        {
            IMongoCollection<BsonDocument> collection = _database.GetCollection<BsonDocument>("restaurants");
            var filter = new BsonDocument();
            var count = 0;
            using (IAsyncCursor<BsonDocument> cursor = await collection.FindAsync(filter))
            {
                while (await cursor.MoveNextAsync())
                {
                    IEnumerable<BsonDocument> batch = cursor.Current;
                    count += batch.Count();
                }
            }

            return count;
        }

        public async Task<List<BsonDocument>> QueryByTopLevelField()
        {
            IMongoCollection<BsonDocument> collection = GetCollection<BsonDocument>("restaurants");
            FilterDefinition<BsonDocument> filter = Builders<BsonDocument>.Filter.Eq("borough", "Manhattan");
            List<BsonDocument> result = await collection.Find(filter).ToListAsync();
            return result;
        }

        public async Task<List<BsonDocument>> QueryByAFieldInAnEmbeddedDocument()
        {
            IMongoCollection<BsonDocument> collection = GetCollection<BsonDocument>("restaurants");
            FilterDefinition<BsonDocument> filter = Builders<BsonDocument>.Filter.Eq("address.zipcode", "10075");
            List<BsonDocument> result = await collection.Find(filter).ToListAsync();
            return result;
        }

        public async Task<List<BsonDocument>> QueryByAFieldInAnArray()
        {
            IMongoCollection<BsonDocument> collection = GetCollection<BsonDocument>("restaurants");
            FilterDefinition<BsonDocument> filter = Builders<BsonDocument>.Filter.Eq("grades.grade", "B");
            List<BsonDocument> result = await collection.Find(filter).ToListAsync();
            return result;
        }

        public async Task<List<BsonDocument>> GreaterThanOperator()
        {
            IMongoCollection<BsonDocument> collection = _database.GetCollection<BsonDocument>("restaurants");
            FilterDefinition<BsonDocument> filter = Builders<BsonDocument>.Filter.Gt("grades.score", 30);
            List<BsonDocument> result = await collection.Find(filter).ToListAsync();
            return result;
        }

        public async Task<List<BsonDocument>> LessThanOperator()
        {
            IMongoCollection<BsonDocument> collection = _database.GetCollection<BsonDocument>("restaurants");
            FilterDefinition<BsonDocument> filter = Builders<BsonDocument>.Filter.Lt("grades.score", 10);
            List<BsonDocument> result = await collection.Find(filter).ToListAsync();
            return result;
        }

        public async Task<List<BsonDocument>> LogicalAnd()
        {
            IMongoCollection<BsonDocument> collection = _database.GetCollection<BsonDocument>("restaurants");
            FilterDefinitionBuilder<BsonDocument> builder = Builders<BsonDocument>.Filter;
            FilterDefinition<BsonDocument> filter = builder.Eq("cuisine", "Italian") & builder.Eq("address.zipcode", "10075");
            List<BsonDocument> result = await collection.Find(filter).ToListAsync();
            return result;
        }

        public async Task<List<BsonDocument>> LogicalOr()
        {
            IMongoCollection<BsonDocument> collection = _database.GetCollection<BsonDocument>("restaurants");
            FilterDefinitionBuilder<BsonDocument> builder = Builders<BsonDocument>.Filter;
            FilterDefinition<BsonDocument> filter = builder.Eq("cuisine", "Italian") | builder.Eq("address.zipcode", "10075");
            List<BsonDocument> result = await collection.Find(filter).ToListAsync();
            return result;
        }

        public async Task<List<BsonDocument>> SortQueryResults()
        {
            IMongoCollection<BsonDocument> collection = _database.GetCollection<BsonDocument>("restaurants");
            var filter = new BsonDocument();
            SortDefinition<BsonDocument> sort = Builders<BsonDocument>.Sort.Ascending("borough").Ascending("address.zipcode");
            List<BsonDocument> result = await collection.Find(filter).Sort(sort).ToListAsync();
            return result;
        }

        public void Disconnect()
        {
            _client = null;
            _database = null;
            GC.Collect();
        }

        private static void ConnectToTestDB()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            _client = new MongoClient();
            _database = _client.GetDatabase("test");
            stopwatch.Stop();
            Console.WriteLine($"Connected to the database in {stopwatch.ElapsedMilliseconds} ms.");
        }

        private static async void InsertADocument()
        {
            var document = new BsonDocument
            {
                {
                    "address", new BsonDocument
                    {
                        {"street", "2 Avenue"},
                        {"zipcode", "10075"},
                        {"building", "1480"},
                        {"coord", new BsonArray {73.9557413, 40.7720266}}
                    }
                },
                {"borough", "Manhattan"},
                {"cuisine", "Italian"},
                {
                    "grades", new BsonArray
                    {
                        new BsonDocument
                        {
                            {"date", new DateTime(2014, 10, 1, 0, 0, 0, DateTimeKind.Utc)},
                            {"grade", "A"},
                            {"score", 11}
                        },
                        new BsonDocument
                        {
                            {"date", new DateTime(2014, 1, 6, 0, 0, 0, DateTimeKind.Utc)},
                            {"grade", "B"},
                            {"score", 17}
                        }
                    }
                },
                {"name", "Vella"},
                {"restaurant_id", "41704620"}
            };

            IMongoCollection<BsonDocument> collection = _database.GetCollection<BsonDocument>("restaurants");
            await collection.InsertOneAsync(document);
        }

        /// <summary>
        ///     Download a file from a url.
        /// </summary>
        /// <param name="urlString">URL of file as a string</param>
        /// <param name="destinationFullName">Full path to where this file will go.</param>
        private static void DownloadFile(string urlString, string destinationFullName)
        {
            var webClient = new WebClient();
            webClient.DownloadFile(urlString, destinationFullName);
        }
    }
}