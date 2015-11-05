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

        private async Task QueryByTopLevelField()
        {
            await ConsoleOutputter.OutputResultsOf(() => _mongoTest.QueryByTopLevelField());
        }

        private async Task QueryByAFieldInAnEmbeddedDocument()
        {
            await ConsoleOutputter.OutputResultsOf(() => _mongoTest.QueryByAFieldInAnEmbeddedDocument());
        }

        private async Task QueryByAFieldInAnArray()
        {
            await ConsoleOutputter.OutputResultsOf(() => _mongoTest.QueryByAFieldInAnArray());
        }

        private async Task GreaterThanOperator()
        {
            await ConsoleOutputter.OutputResultsOf(() => _mongoTest.GreaterThanOperator());
        }

        private async Task LessThanOperator()
        {
            await ConsoleOutputter.OutputResultsOf(() => _mongoTest.LessThanOperator());
        }

        private async Task LogicalAnd()
        {
            await ConsoleOutputter.OutputResultsOf(() => _mongoTest.LogicalAnd());
        }

        private async Task LogicalOr()
        {
            await ConsoleOutputter.OutputResultsOf(() => _mongoTest.LogicalOr());
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
