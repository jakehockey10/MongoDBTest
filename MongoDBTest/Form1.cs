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
            StartTest();
        }

        private void StartTest()
        {
            if (!_mongoTest.IsServerRunning) OutputResultsOf(() => _mongoTest.StartServer());
            if (_mongoTest.IsServerRunning) OutputResultsOf(() => _mongoTest.ConnectToTestDB());
        }

        private static void OutputResultsOf(Action action)
        {
            Console.WriteLine(TaskCompletionTimeRecorder.RecordTaskCompletionTime(action));
        }

        private static void OutputResultsOf(Func<bool> action)
        {
            Console.WriteLine(TaskCompletionTimeRecorder.RecordTaskCompletionTime(() => action()));
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            await UpdateTopLevelFields();
        }

        private async Task CountRestaurants()
        {
            await ConsoleOutputter.OutputResultsOf(() => _mongoTest.CountRestaurants());
        }

        private async Task QueryByTopLevelField(string field, string value)
        {
            await ConsoleOutputter.OutputResultsOf(() => _mongoTest.QueryByTopLevelField(field, value));
        }

        private async Task QueryByAFieldInAnEmbeddedDocument(string field, string value)
        {
            await ConsoleOutputter.OutputResultsOf(() => _mongoTest.QueryByAFieldInAnEmbeddedDocument(field, value));
        }

        private async Task QueryByAFieldInAnArray(string field, string value)
        {
            await ConsoleOutputter.OutputResultsOf(() => _mongoTest.QueryByAFieldInAnArray(field, value));
        }

        private async Task GreaterThanOperator(string field, int value)
        {
            await ConsoleOutputter.OutputResultsOf(() => _mongoTest.GreaterThanOperator(field, value));
        }

        private async Task LessThanOperator(string field, int value)
        {
            await ConsoleOutputter.OutputResultsOf(() => _mongoTest.LessThanOperator(field, value));
        }

        private async Task LogicalAnd(List<Tuple<string, string>> fieldValuePairs)
        {
            await ConsoleOutputter.OutputResultsOf(() => _mongoTest.LogicalAnd(fieldValuePairs));
        }

        private async Task LogicalOr(List<Tuple<string, string>> fieldValuePairs)
        {
            await ConsoleOutputter.OutputResultsOf(() => _mongoTest.LogicalOr(fieldValuePairs));
        }

        private async Task SortQueryResults()
        {
            await ConsoleOutputter.OutputResultsOf(() => _mongoTest.SortQueryResults());
        }

        private async Task UpdateTopLevelFields()
        {
            await ConsoleOutputter.OutputResultsOf(() => _mongoTest.UpdateTopLevelFields());
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            _mongoTest.KillServerAndCleanup();
        }
    }
}
