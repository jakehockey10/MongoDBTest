using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Forms;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MongoDBTest
{
    public partial class Form1 : Form
    {
        private readonly MongoTest _mongoTest;

        public Form1()
        {
            InitializeComponent();
            _mongoTest = new MongoTest();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            await QueryByAFieldInAnArray();
        }
        
        private static async Task<string> RecordTaskCompletionTime(Func<Task<List<BsonDocument>>> action)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            List<BsonDocument> result = await action();
            stopwatch.Stop();
            return $"Found {result.Count} restaurants with that query ({stopwatch.ElapsedMilliseconds} ms).";
        }

        private static async Task<string> RecordTaskCompletionTime(Func<Task<int>> action)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            int result = await action();
            stopwatch.Stop();
            return $"{result} restaurants in the test db ({stopwatch.ElapsedMilliseconds} ms).";
        }

        private async Task CountRestaurants()
        {
            await OutputResultsOf(() => _mongoTest.CountRestaurants());
        }

        private async Task QueryByTopLevelField()
        {
            await OutputResultsOf(() => _mongoTest.QueryByTopLevelField());
        }

        private async Task QueryByAFieldInAnEmbeddedDocument()
        {
            await OutputResultsOf(() => _mongoTest.QueryByAFieldInAnEmbeddedDocument());
        }

        private async Task QueryByAFieldInAnArray()
        {
            await OutputResultsOf(() => _mongoTest.QueryByAFieldInAnArray());
        }

        private async Task GreaterThanOperator()
        {
            await OutputResultsOf(() => _mongoTest.GreaterThanOperator());
        }

        private async Task LessThanOperator()
        {
            await OutputResultsOf(() => _mongoTest.LessThanOperator());
        }

        private async Task LogicalAnd()
        {
            await OutputResultsOf(() => _mongoTest.LogicalAnd());
        }

        private async Task LogicalOr()
        {
            await OutputResultsOf(() => _mongoTest.LogicalOr());
        }

        private async Task SortQueryResults()
        {
            await OutputResultsOf(() => _mongoTest.SortQueryResults());
        }
        
        private static async Task OutputResultsOf(Func<Task<int>> action)
        {
            Console.WriteLine(await RecordTaskCompletionTime(async () => await action()));
        }

        private static async Task OutputResultsOf(Func<Task<List<BsonDocument>>> action)
        {
            Console.WriteLine(await RecordTaskCompletionTime(async () => await action()));
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            _mongoTest.Disconnect();
        }
    }
}
