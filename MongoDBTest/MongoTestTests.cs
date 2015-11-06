using System;
using System.Collections.Generic;
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
            _mongoTest.LoadDataset();
        }

        [TestFixtureTearDown]
        public async void TestFixtureTearDown()
        {
            await _mongoTest.DropACollection();
            _mongoTest.KillServerAndCleanup();
        }

        [TearDown]
        public void TearDown()
        {
            _mongoTest.LoadDataset();
        }

        [Test]
        public async void ShouldBeAbleToQueryToplevelField()
        {
            List<BsonDocument> list = await _mongoTest.QueryByTopLevelField("borough", "Manhattan");
            Assert.That(list.Count, Is.EqualTo(10259));
        }

        [Test]
        public async void ShouldBeAbleToQueryByAFieldInAnEmbeddedDocument()
        {
            List<BsonDocument> list = await _mongoTest.QueryByAFieldInAnEmbeddedDocument("address.zipcode", "10075");
            Assert.That(list.Count, Is.EqualTo(99));
        }

        [Test]
        public async void ShouldBeAbleToQueryByAFieldInAnArray()
        {
            List<BsonDocument> list = await _mongoTest.QueryByAFieldInAnArray("grades.grade", "B");
            Assert.That(list.Count, Is.EqualTo(8280));
        }

        public async void ShouldBeAbleToUseGreaterThanOperator()
        {
            List<BsonDocument> list = await _mongoTest.GreaterThanOperator("grades.score", 30);
            Assert.That(list.Count, Is.EqualTo(1959));
        }

        public async void ShouldBeAbleToUseLessThanOperator()
        {
            List<BsonDocument> list = await _mongoTest.LessThanOperator("grades.score", 10);
            Assert.That(list.Count, Is.EqualTo(19065));
        }

        [Test]
        public async void ShouldBeAbleToUseLogicalAnd()
        {
            List<BsonDocument> list = await _mongoTest.LogicalAnd(new List<Tuple<string, string>>
            {
                new Tuple<string, string>("cuisine", "Italian"),
                new Tuple<string, string>("address.zipcode", "10075")
            });
            Assert.That(list.Count, Is.EqualTo(15));
        }

        [Test]
        public async void ShouldBeAbleToUseLogicalOr()
        {
            List<BsonDocument> list = await _mongoTest.LogicalOr(new List<Tuple<string, string>>
            {
                new Tuple<string, string>("cuisine", "Italian"),
                new Tuple<string, string>("address.zipcode", "10075")
            });
            Assert.That(list.Count, Is.EqualTo(1153));
        }

        //[Test]
        //public async void ShouldBeAbleToSortQueryResults()
        //{
        //    List<BsonDocument> list = await _mongoTest.SortQueryResults();

        //}

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

        [Test]
        public async void ShouldBeAbleToGroupDocumentsByAFieldAndCalculateCount()
        {
            var expectedResults = new[]
            {
                BsonDocument.Parse("{ _id : 'Staten Island', count : 969 }"),
                BsonDocument.Parse("{ _id : 'Brooklyn', count : 6086 }"),
                BsonDocument.Parse("{ _id : 'Manhattan', count : 10259 }"),
                BsonDocument.Parse("{ _id : 'Queens', count : 5656 }"),
                BsonDocument.Parse("{ _id : 'Bronx', count : 2338 }"),
                BsonDocument.Parse("{ _id : 'Missing', count : 51 }")
            };
            List<BsonDocument> list = await _mongoTest.GroupDocumentsByAFieldAndCalculateCount();
            Assert.That(list, Is.EquivalentTo(expectedResults));
        }

        [Test]
        public async void ShouldBeAbleToFilterAndGroupDocuments()
        {
            var expectedResults = new[]
            {
                BsonDocument.Parse("{ _id : '11368', count : 1 }"),
                BsonDocument.Parse("{ _id : '11106', count : 3 }"),
                BsonDocument.Parse("{ _id : '11377', count : 1 }"),
                BsonDocument.Parse("{ _id : '11103', count : 1 }"),
                BsonDocument.Parse("{ _id : '11101', count : 2 }")
            };
            List<BsonDocument> list = await _mongoTest.FilterAndGroupDocuments();
            Assert.That(list, Is.EquivalentTo(expectedResults));
        }

        [Test]
        public async void ShouldBeAbleToCreateASingleFieldIndex()
        {
            await _mongoTest.CreateASingleFieldIndex();
            using (IAsyncCursor<BsonDocument> cursor = await _mongoTest.ListIndexesAsync())
            {
                List<BsonDocument> indexes = await cursor.ToListAsync();
                Assert.That(indexes, Has.Exactly(1).Matches<BsonDocument>(index => index["name"] == "cuisine_1"));
            }
        }

        [Test]
        public async void ShouldBeAbleToCreateACompoundIndex()
        {
            await _mongoTest.CreateACompoundIndex();
            using (IAsyncCursor<BsonDocument> cursor = await _mongoTest.ListIndexesAsync())
            {
                List<BsonDocument> indexes = await cursor.ToListAsync();
                Assert.That(indexes, Has.Exactly(1).Matches<BsonDocument>(index => index["name"] == "cuisine_1_address.zipcode_1"));
            }
        }
    }
}
