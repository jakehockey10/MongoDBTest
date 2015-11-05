using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MongoDBTest
{
    public class TaskCompletionTimeRecorder
    {
        public static string RecordTaskCompletionTime(Action action)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            action();
            stopwatch.Stop();
            return $"action took {stopwatch.ElapsedMilliseconds} ms.";
        }

        public static async Task<string> RecordTaskCompletionTime(Func<Task<int>> action)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            int result = await action();
            stopwatch.Stop();
            return $"{result} restaurants in the test db ({stopwatch.ElapsedMilliseconds} ms).";
        }

        public static async Task<string> RecordTaskCompletionTime(Func<Task<List<BsonDocument>>> action)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            List<BsonDocument> result = await action();
            stopwatch.Stop();
            return $"Found {result.Count} restaurants with that query ({stopwatch.ElapsedMilliseconds} ms).";
        }

        public static async Task<string> RecordTaskCompletionTime(Func<Task<UpdateResult>> action)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            UpdateResult result = await action();
            stopwatch.Stop();
            return $"Found {result.MatchedCount} matching restaurants; Updated {result.ModifiedCount} restaurants.";
        }
    }
}