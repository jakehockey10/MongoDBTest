using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MongoDBTest
{
    public class MongoTest
    {
        #region Private Fields

        private const string ServerExecutable = @"C:\Program Files\MongoDB\Server\3.0\bin\mongod.exe";
        private const string ServerProcessName = "mongod";
        private const string MongoImportExecutable = @"C:\Program Files\MongoDB\Server\3.0\bin\mongoimport.exe";
        private const string MongoImportArguments = "--db test --collection restaurants --drop --file \"C:\\Users\\jakeh\\Documents\\Visual Studio 2015\\Projects\\MongoDBTest\\dataset.json\"";

        #endregion

        #region Protected Fields

        protected static IMongoClient Client;
        protected static IMongoDatabase Database;

        #endregion

        #region Public Properties

        public bool IsServerRunning => Process.GetProcessesByName(Path.GetFileName(ServerProcessName)).Length > 0;

        #endregion

        #region Public Methods

        public async Task<int> CountRestaurants()
        {
            IMongoCollection<BsonDocument> collection = GetRestaurants();
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

        public async Task<List<BsonDocument>> QueryByTopLevelField(string field, string value)
        {
            IMongoCollection<BsonDocument> collection = GetRestaurants();
            FilterDefinition<BsonDocument> filter = Builders<BsonDocument>.Filter.Eq(field, value);
            return await collection.Find(filter).ToListAsync();
        }

        public async Task<List<BsonDocument>> QueryByAFieldInAnEmbeddedDocument(string field, string value)
        {
            IMongoCollection<BsonDocument> collection = GetRestaurants();
            FilterDefinition<BsonDocument> filter = Builders<BsonDocument>.Filter.Eq(field, value);
            return await collection.Find(filter).ToListAsync();
        }

        public async Task<List<BsonDocument>> QueryByAFieldInAnArray(string field, string value)
        {
            IMongoCollection<BsonDocument> collection = GetRestaurants();
            FilterDefinition<BsonDocument> filter = Builders<BsonDocument>.Filter.Eq(field, value);
            return await collection.Find(filter).ToListAsync();
        }

        public async Task<List<BsonDocument>> GreaterThanOperator(string field, int value)
        {
            IMongoCollection<BsonDocument> collection = GetRestaurants();
            FilterDefinition<BsonDocument> filter = Builders<BsonDocument>.Filter.Gt(field, value);
            return await collection.Find(filter).ToListAsync();
        }

        public async Task<List<BsonDocument>> LessThanOperator(string field, int value)
        {
            IMongoCollection<BsonDocument> collection = GetRestaurants();
            FilterDefinition<BsonDocument> filter = Builders<BsonDocument>.Filter.Lt(field, value);
            return await collection.Find(filter).ToListAsync();
        }

        public async Task<List<BsonDocument>> LogicalAnd(List<Tuple<string, string>> fieldValuePairs)
        {
            IMongoCollection<BsonDocument> collection = GetRestaurants();
            FilterDefinitionBuilder<BsonDocument> builder = Builders<BsonDocument>.Filter;
            Tuple<string, string> first = fieldValuePairs.First();
            FilterDefinition<BsonDocument> filter = builder.Eq(first.Item1, first.Item2);
            filter = fieldValuePairs.Skip(1).Aggregate(filter, (current, pair) => current & builder.Eq(pair.Item1, pair.Item2));
            return await collection.Find(filter).ToListAsync();
        }
        
        public async Task<List<BsonDocument>> LogicalOr(List<Tuple<string, string>> fieldValuePairs)
        {
            IMongoCollection<BsonDocument> collection = GetRestaurants();
            FilterDefinitionBuilder<BsonDocument> builder = Builders<BsonDocument>.Filter;
            Tuple<string, string> first = fieldValuePairs.First();
            FilterDefinition<BsonDocument> filter = builder.Eq(first.Item1, first.Item2);
            filter = fieldValuePairs.Skip(1).Aggregate(filter, (current, pair) => current | builder.Eq(pair.Item1, pair.Item2));
            return await collection.Find(filter).ToListAsync();
        }

        public async Task<List<BsonDocument>> SortQueryResults()
        {
            IMongoCollection<BsonDocument> collection = GetRestaurants();
            var filter = new BsonDocument();
            SortDefinition<BsonDocument> sort =
                Builders<BsonDocument>.Sort.Ascending("borough").Ascending("address.zipcode");
            return await collection.Find(filter).Sort(sort).ToListAsync();
        }

        public static async void InsertADocument(BsonDocument document)
        {
            IMongoCollection<BsonDocument> collection = GetRestaurants();
            await collection.InsertOneAsync(document);
        }

        public async Task<UpdateResult> UpdateTopLevelFields()
        {
            IMongoCollection<BsonDocument> collection = GetRestaurants();
            FilterDefinition<BsonDocument> filter = Builders<BsonDocument>.Filter.Eq("name", "Juni");
            UpdateDefinition<BsonDocument> update = Builders<BsonDocument>.Update
                .Set("cuisine", "American (New)")
                .CurrentDate("lastModified");
            return await collection.UpdateOneAsync(filter, update);
        }

        public async Task<UpdateResult> UpdateAnEmbeddedField()
        {
            IMongoCollection<BsonDocument> collection = GetRestaurants();
            FilterDefinition<BsonDocument> filter = Builders<BsonDocument>.Filter.Eq("restaurant_id", "41156888");
            //UpdateDefinition<BsonDocument> update = Builders<BsonDocument>.Update.Set("address.street", "East 31st Street");
            UpdateDefinition<BsonDocument> update = Builders<BsonDocument>.Update.Set("address.street",
                Guid.NewGuid().ToString());
            return await collection.UpdateOneAsync(filter, update);
        }

        public async Task<UpdateResult> UpdateMultipleDocuments()
        {
            IMongoCollection<BsonDocument> collection = GetRestaurants();
            FilterDefinitionBuilder<BsonDocument> builder = Builders<BsonDocument>.Filter;
            FilterDefinition<BsonDocument> filter = builder.Eq("address.zipcode", "10016");
            UpdateDefinition<BsonDocument> update = Builders<BsonDocument>.Update
                .Set("cuisine", Guid.NewGuid().ToString())
                .CurrentDate("lastModified");
            return await collection.UpdateManyAsync(filter, update);
        }

        public async Task<ReplaceOneResult> ReplaceOneDocument()
        {
            IMongoCollection<BsonDocument> collection = GetRestaurants();
            await collection.InsertOneAsync(new BsonDocument {{"banana", "pookie-dookies"}});
            FilterDefinitionBuilder<BsonDocument> builder = Builders<BsonDocument>.Filter;
            FilterDefinition<BsonDocument> filter = builder.Eq("banana", "pookie-dookies");
            var bsonDocument = new BsonDocument {{"bananas", "best fruit for monkeys fo' sho'"}};
            return await collection.ReplaceOneAsync(filter, bsonDocument);
        }

        public async Task<DeleteResult> DeleteAllDocumentsThatMatchACondition()
        {
            IMongoCollection<BsonDocument> collection = GetRestaurants();
            FilterDefinition<BsonDocument> filter = Builders<BsonDocument>.Filter.Eq("borough", "Manhattan");
            return await collection.DeleteManyAsync(filter);
        }

        public async Task<DeleteResult> DeleteAllDocuments()
        {
            IMongoCollection<BsonDocument> collection = GetRestaurants();
            var filter = new BsonDocument();
            return await collection.DeleteManyAsync(filter);
        }

        public async Task<List<BsonDocument>> GroupDocumentsByAFieldAndCalculateCount()
        {
            IMongoCollection<BsonDocument> collection = GetRestaurants();
            IAggregateFluent<BsonDocument> aggregate =
                collection.Aggregate()
                    .Group(new BsonDocument {{"_id", "$borough"}, {"count", new BsonDocument("$sum", 1)}});
            return await aggregate.ToListAsync();
        }

        public async Task<List<BsonDocument>> FilterAndGroupDocuments()
        {
            IMongoCollection<BsonDocument> collection = Database.GetCollection<BsonDocument>("restaurants");
            IAggregateFluent<BsonDocument> aggregate = collection.Aggregate()
                .Match(new BsonDocument {{"borough", "Queens"}, {"cuisine", "Brazilian"}})
                .Group(new BsonDocument {{"_id", "$address.zipcode"}, {"count", new BsonDocument("$sum", 1)}});
            return await aggregate.ToListAsync();
        }

        public async Task<string> CreateASingleFieldIndex()
        {
            IMongoCollection<BsonDocument> collection = Database.GetCollection<BsonDocument>("restaurants");
            IndexKeysDefinition<BsonDocument> keys = Builders<BsonDocument>.IndexKeys.Ascending("cuisine");
            return await collection.Indexes.CreateOneAsync(keys);
        }

        public async Task<string> CreateACompoundIndex()
        {
            IMongoCollection<BsonDocument> collection = GetRestaurants();
            IndexKeysDefinition<BsonDocument> keys = Builders<BsonDocument>.IndexKeys.Ascending("cuisine").Ascending("address.zipcode");
            return await collection.Indexes.CreateOneAsync(keys);
        }

        public async Task DropACollection()
        {
            await Database.DropCollectionAsync("restaurants");
        }

        public async Task<IAsyncCursor<BsonDocument>> ListCollectionsAsync()
        {
            return await Database.ListCollectionsAsync();
        }

        public void KillServerAndCleanup()
        {
            while (Process.GetProcessesByName(ServerProcessName).Length > 0)
            {
                foreach (Process process in Process.GetProcessesByName(ServerProcessName))
                {
                    try
                    {
                        process.Kill();
                    }
                    catch (Win32Exception e)
                    {
                        // can't tell it to terminate if terminating.
                        Console.WriteLine(e.Message);
                    }
                    catch (InvalidOperationException ioe)
                    {
                        // can't tell it to terminate if terminated.
                        Console.WriteLine(ioe.Message);
                    }
                }
            }
            Client = null;
            Database = null;
            GC.Collect();
        }

        public void StartServer()
        {
            var process = new Process {StartInfo = {FileName = ServerExecutable}};
            bool start = process.Start();
            Thread.Sleep(1000);
        }

        public void ConnectToTestDB()
        {
            Client = new MongoClient();
            Database = Client.GetDatabase("test");
        }

        public async Task<IAsyncCursor<BsonDocument>> ListIndexesAsync()
        {
            return await GetRestaurants().Indexes.ListAsync();
        }

        /// <summary>
        ///     Download a file from a url.
        /// </summary>
        /// <param name="urlString">URL of file as a string</param>
        /// <param name="destinationFullName">Full path to where this file will go.</param>
        public static void DownloadFile(string urlString, string destinationFullName)
        {
            var webClient = new WebClient();
            webClient.DownloadFile(urlString, destinationFullName);
        }

        public void LoadDataset()
        {
            var process = new Process
            {
                StartInfo =
                {
                    FileName = MongoImportExecutable,
                    Arguments = MongoImportArguments
                }
            };
            process.Start();
            process.WaitForExit();
        }

        #endregion

        #region Private Methods

        private static IMongoCollection<T> GetCollection<T>(string name)
        {
            return Database.GetCollection<T>(name);
        }

        private static IMongoCollection<BsonDocument> GetRestaurants()
        {
            return GetCollection<BsonDocument>("restaurants");
        }

        #endregion
    }
}