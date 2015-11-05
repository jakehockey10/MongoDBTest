using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MongoDBTest
{
    public class ConsoleOutputter
    {
        public static async Task OutputResultsOf(Func<Task<int>> action)
        {
            Console.WriteLine(await TaskCompletionTimeRecorder.RecordTaskCompletionTime(async () => await action()));
        }

        public static async Task OutputResultsOf(Func<Task<List<BsonDocument>>> action)
        {
            Console.WriteLine(await TaskCompletionTimeRecorder.RecordTaskCompletionTime(async () => await action()));
        }

        public static async Task OutputResultsOf(Func<Task<UpdateResult>> action)
        {
            Console.WriteLine(await TaskCompletionTimeRecorder.RecordTaskCompletionTime(async () => await action()));
        }
    }
}