using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using NUnit.Framework;

namespace MongoDBTest
{
    [TestFixture]
    internal class MongoTestTests
    {
        private MongoTest _mongoTest;

        [TestFixtureSetUp]
        public void TestFixtureSetup()
        {
            _mongoTest = new MongoTest();
            _mongoTest.KillServerAndCleanup();
            Assert.True(_mongoTest.StartServer());
            _mongoTest.ConnectToTestDB();
        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            _mongoTest.KillServerAndCleanup();
        }
        
        [TearDown]
        public void TearDown()
        {
            _mongoTest.LoadDataset();
        }
        
        [Test]
        public async void ShouldBeAbleToUpdateTopLevelFields()
        {
            UpdateResult updateResult = await _mongoTest.UpdateTopLevelFields();
            Assert.That(updateResult.MatchedCount, Is.EqualTo(1));
            if (updateResult.IsModifiedCountAvailable)
            {
                Assert.That(updateResult.ModifiedCount, Is.EqualTo(1));
            }
        }

        [Test]
        public async void ShouldBeAbleToUpdateAnEmbeddedField()
        {
            UpdateResult updateResult = await _mongoTest.UpdateAnEmbeddedField();
            Assert.That(updateResult.MatchedCount, Is.EqualTo(1));
            if (updateResult.IsModifiedCountAvailable)
            {
                Assert.That(updateResult.ModifiedCount, Is.EqualTo(1));
            }
        }

        [Test]
        public async void ShouldBeAbleToUpdateMultipleDocuments()
        {
            UpdateResult updateResult = await _mongoTest.UpdateMultipleDocuments();
            Assert.That(updateResult.MatchedCount, Is.EqualTo(433));
            if (updateResult.IsModifiedCountAvailable)
            {
                Assert.That(updateResult.ModifiedCount, Is.EqualTo(433));
            }
        }

        [Test]
        public async void ShouldBeAbleToReplaceOneDocument()
        {
            ReplaceOneResult replaceOneResult = await _mongoTest.ReplaceOneDocument();
            Assert.That(replaceOneResult.MatchedCount, Is.EqualTo(1));
            if (replaceOneResult.IsModifiedCountAvailable)
            {
                Assert.That(replaceOneResult.ModifiedCount, Is.EqualTo(1));
            }
        }

        [Test]
        public async void ShouldBeAbleToDeleteAllDocumentsThatMatchACondition()
        {
            DeleteResult deleteResult = await _mongoTest.DeleteAllDocumentsThatMatchACondition();
            Assert.That(deleteResult.DeletedCount, Is.EqualTo(10259));
        }

        [Test]
        public async void ShouldBeAbleToDeleteAllDocuments()
        {
            DeleteResult deleteResult = await _mongoTest.DeleteAllDocuments();
            Assert.That(deleteResult.DeletedCount, Is.EqualTo(25359));
        }

        [Test]
        public async void ShouldBeAbleToDropACollection()
        {
            await _mongoTest.DropACollection();
            using (IAsyncCursor<BsonDocument> cursor = await _mongoTest.ListCollectionsAsync())
            {
                List<BsonDocument> collections = await cursor.ToListAsync();
                Assert.That(collections, Has.None.Matches<BsonDocument>(document => document["name"] == "restaurants"));
            }
        }
    }
}
