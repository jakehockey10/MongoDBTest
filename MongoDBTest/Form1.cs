using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDBTest.Properties;

namespace MongoDBTest
{
    public partial class Form1 : Form
    {
        private readonly MongoTest _mongoTest;

        public enum Action
        {
            NoOp,
            CountRestaurants
        }

        private readonly Dictionary<Action, string> _comboBoxEntries = new Dictionary<Action, string>
        {
            {Action.CountRestaurants, "Count Restaurants"}
        };

        private Dictionary<Action, string> _selectedItem;

        public Form1()
        {
            InitializeComponent();
            _mongoTest = new MongoTest();
            StartTest();
        }

        private void StartTest()
        {
            if (!_mongoTest.IsServerRunning) ConsoleOutputter.OutputResultsOf(() => _mongoTest.StartServer());
            if (_mongoTest.IsServerRunning) ConsoleOutputter.OutputResultsOf(() => _mongoTest.ConnectToTestDB());
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            switch (SelectedItem.Key)
            {
                case Action.NoOp:
                    break;
                case Action.CountRestaurants:
                    await CountRestaurants();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public KeyValuePair<Action, string> SelectedItem => (KeyValuePair<Action, string>) comboBox1.SelectedItem;

        private async Task CountRestaurants()
        {
            label1.Text = Resources.Form1_CountRestaurants_working___;
            string output = await TaskCompletionTimeRecorder.RecordTaskCompletionTime(() => _mongoTest.CountRestaurants());
            label1.Text = output;
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

        private void Form1_Load(object sender, EventArgs e)
        {
            comboBox1.DataSource = _comboBoxEntries.ToList();
            comboBox1.DisplayMember = "Value";
            comboBox1.ValueMember = "Key";
        }

        private void comboBox1_SelectionChangeCommitted(object sender, EventArgs e)
        {
            _selectedItem = (Dictionary<Action, string>) comboBox1.SelectedItem;
        }
    }
}
