﻿using NUnit.Framework;
using System;
using System.Linq;

using Marcello;
using System.Collections.Generic;
using Marcello.Test.Classes;

namespace Marcello.Test
{
    [TestFixture ()]
    public class Test
    {
        Marcello _marcello;

        [SetUp]
        public void Setup()
        {
            _marcello = new Marcello(new InMemoryStreamProvider());
        }

        [Test]
        public void TestGetCollectionReturnsACollection()
        {
            var collection = _marcello.Collection<Article>();
            Assert.NotNull(collection, "Collection should not be null");
        }

        [Test]
        public void TestInsertObjectShouldFindObject()
        {
            var toiletPaper = Article.ToiletPaper;
            _marcello.Collection<Article>().Persist(toiletPaper);
            var article = _marcello.Collection<Article>().All.FirstOrDefault();
            Assert.AreEqual(toiletPaper.Name, article.Name, "First article");
        }

        [Test]
        public void Test2Objects()
        {
            var toiletPaper = Article.ToiletPaper;
            var spinalTapDvd = Article.SpinalTapDvd;

            _marcello.Collection<Article>().Persist (toiletPaper);
            _marcello.Collection<Article>().Persist (spinalTapDvd);

            var articleNames = _marcello.Collection<Article>().All.Select(a => a.Name).ToList();

            Assert.AreEqual(new List<string>{toiletPaper.Name, spinalTapDvd.Name}, articleNames, "Should return 2 article names");
        }

        [Test()]
        public void TestMultipleObjects()
        {
            var toiletPaper = Article.ToiletPaper;
            var spinalTapDvd = Article.SpinalTapDvd;
            var barbieDoll = Article.BarbieDoll;

            _marcello.Collection<Article>().Persist(toiletPaper);
            _marcello.Collection<Article>().Persist(spinalTapDvd);
            _marcello.Collection<Article>().Persist(barbieDoll);
        
            var articleNames = _marcello.Collection<Article>().All.Select(a => a.Name).ToList();
            Assert.AreEqual(new List<string>{toiletPaper.Name, spinalTapDvd.Name, barbieDoll.Name}, articleNames, "Should return multiple article names");
        }

        [Test]
        public void TestUpdate()
        {
            var toiletPaper = Article.ToiletPaper;
            var spinalTapDvd = Article.SpinalTapDvd;

            _marcello.Collection<Article>().Persist(toiletPaper);
            _marcello.Collection<Article>().Persist(spinalTapDvd);

            toiletPaper.Name += "Extra Strong ToiletPaper";

            _marcello.Collection<Article>().Persist(toiletPaper);
        
            var reloadedArticle = _marcello.Collection<Article>().All.Where (a => a.ID == Article.ToiletPaper.ID).FirstOrDefault ();

            Assert.AreEqual(toiletPaper.Name, reloadedArticle.Name, "Should return updated article name");
        }

        [Test]
        public void TestSmallUpdate()
        {
            var toiletPaper = Article.ToiletPaper;
            var spinalTapDvd = Article.SpinalTapDvd;

            _marcello.Collection<Article>().Persist(toiletPaper);
            _marcello.Collection<Article>().Persist(spinalTapDvd);
            toiletPaper.Name = "Short";
            _marcello.Collection<Article>().Persist(toiletPaper);

            var articleNames = _marcello.Collection<Article>().All.Select(a => a.Name).ToList();
            articleNames.Sort();

            Assert.AreEqual(new List<string>{spinalTapDvd.Name, toiletPaper.Name}, articleNames, "Should return updated article names");
        }

        [Test]
        public void TestLargeUpdate()
        {
            var toiletPaper = Article.ToiletPaper;
            var spinalTapDvd = Article.SpinalTapDvd;

            _marcello.Collection<Article>().Persist(toiletPaper);
            _marcello.Collection<Article>().Persist(spinalTapDvd);

            toiletPaper.Name += "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Phasellus in lorem porta, mollis odio sit amet, tincidunt leo. Aliquam suscipit sapien nec orci fermentum imperdiet. Sed id est ante. Aliquam nec nibh id purus fermentum lobortis. Morbi posuere ullamcorper diam, in tincidunt mi pulvinar ut. Nam imperdiet mi a viverra congue. Proin eros metus, vehicula tempus eros vitae, pulvinar posuere nisi. Sed volutpat laoreet tortor. Sed sagittis nunc sed dui sollicitudin porta. Donec non neque ut erat commodo convallis vel ac dolor. Quisque eu lectus dapibus, varius sem non, semper dolor. Morbi at venenatis tellus. Integer efficitur neque ornare, lobortis nisi suscipit, consequat purus. Aliquam erat volutpat.";

            _marcello.Collection<Article>().Persist(toiletPaper);

            var articleNames = _marcello.Collection<Article>().All.Select(a => a.Name).ToList();
            articleNames.Sort();

            Assert.AreEqual(new List<string>{spinalTapDvd.Name, toiletPaper.Name}, articleNames, "Should return updated article names");
        }

        [Test]
        public void TestLargeUpdateForLastObject()
        {
            var toiletPaper = Article.ToiletPaper;
            var spinalTapDvd = Article.SpinalTapDvd;

            _marcello.Collection<Article>().Persist(toiletPaper);
            _marcello.Collection<Article>().Persist(spinalTapDvd);

            spinalTapDvd.Name += "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Phasellus in lorem porta, mollis odio sit amet, tincidunt leo. Aliquam suscipit sapien nec orci fermentum imperdiet. Sed id est ante. Aliquam nec nibh id purus fermentum lobortis. Morbi posuere ullamcorper diam, in tincidunt mi pulvinar ut. Nam imperdiet mi a viverra congue. Proin eros metus, vehicula tempus eros vitae, pulvinar posuere nisi. Sed volutpat laoreet tortor. Sed sagittis nunc sed dui sollicitudin porta. Donec non neque ut erat commodo convallis vel ac dolor. Quisque eu lectus dapibus, varius sem non, semper dolor. Morbi at venenatis tellus. Integer efficitur neque ornare, lobortis nisi suscipit, consequat purus. Aliquam erat volutpat.";

            _marcello.Collection<Article>().Persist(spinalTapDvd);

            var articleNames = _marcello.Collection<Article>().All.Select(a => a.Name).ToList();
            articleNames.Sort();

            Assert.AreEqual(new List<string>{spinalTapDvd.Name, toiletPaper.Name}, articleNames, "Should return updated article names");
        }
            
        [Test]
        public void TestDeleteOnlyObject()
        {
            var toiletPaper = Article.ToiletPaper;

            _marcello.Collection<Article>().Persist(toiletPaper);

            _marcello.Collection<Article>().Destroy(toiletPaper);
        
            Assert.AreEqual (0, _marcello.Collection<Article>().All.Count());
        }

        [Test]
        public void TestDeleteFirstObject()
        {
            var toiletPaper = Article.ToiletPaper;
            var spinalTapDvd = Article.SpinalTapDvd;

            _marcello.Collection<Article>().Persist(toiletPaper);
            _marcello.Collection<Article>().Persist(spinalTapDvd);

            _marcello.Collection<Article> ().Destroy(toiletPaper);

            Assert.AreEqual(1, _marcello.Collection<Article>().All.Count());
            Assert.AreEqual(spinalTapDvd.ID, _marcello.Collection<Article>().All.First().ID);
        }

        [Test]
        public void TestDeleteMiddleObject()
        {
            var toiletPaper = Article.ToiletPaper;
            var spinalTapDvd = Article.SpinalTapDvd;
            var barbieDoll = Article.BarbieDoll;

            _marcello.Collection<Article>().Persist(toiletPaper);
            _marcello.Collection<Article>().Persist(spinalTapDvd);
            _marcello.Collection<Article>().Persist(barbieDoll);

            _marcello.Collection<Article> ().Destroy(spinalTapDvd);

            Assert.AreEqual(2, _marcello.Collection<Article>().All.Count());
            Assert.AreEqual(toiletPaper.ID, _marcello.Collection<Article>().All.First().ID);
            Assert.AreEqual(barbieDoll.ID, _marcello.Collection<Article>().All.Last().ID);
        }

        [Test]
        public void TestDeleteLastObject()
        {

            var toiletPaper = Article.ToiletPaper;
            var spinalTapDvd = Article.SpinalTapDvd;
            var barbieDoll = Article.BarbieDoll;

            _marcello.Collection<Article>().Persist(toiletPaper);
            _marcello.Collection<Article>().Persist(spinalTapDvd);
            _marcello.Collection<Article>().Persist(barbieDoll);

            _marcello.Collection<Article>().Destroy(barbieDoll);

            Assert.AreEqual(2, _marcello.Collection<Article> ().All.Count());
            Assert.AreEqual(toiletPaper.ID, _marcello.Collection<Article>().All.First().ID);
            Assert.AreEqual(spinalTapDvd.ID, _marcello.Collection<Article>().All.Last().ID);
        }

        [Test]
        public void DeleteLastInsertNew()
        {
            var toiletPaper = Article.ToiletPaper;
            var spinalTapDvd = Article.SpinalTapDvd;

            _marcello.Collection<Article>().Persist(toiletPaper);
            _marcello.Collection<Article>().Persist(spinalTapDvd);
            _marcello.Collection<Article> ().Destroy(spinalTapDvd);

            var barbieDoll = Article.BarbieDoll;
            _marcello.Collection<Article>().Persist(barbieDoll);

            Assert.AreEqual(2, _marcello.Collection<Article>().All.Count());
            Assert.AreEqual(toiletPaper.ID, _marcello.Collection<Article>().All.First().ID);
            Assert.AreEqual(barbieDoll.ID, _marcello.Collection<Article>().All.Last().ID);
        }

        [Test]
        public void DeleteOnlyOnlyInsertNew()
        {        
            var spinalTapDvd = Article.SpinalTapDvd;

            _marcello.Collection<Article>().Persist(spinalTapDvd);
            _marcello.Collection<Article>().Destroy(spinalTapDvd);

            var barbieDoll = Article.BarbieDoll;
            _marcello.Collection<Article>().Persist(barbieDoll);

            Assert.AreEqual(1, _marcello.Collection<Article>().All.Count());
            Assert.AreEqual(barbieDoll.ID, _marcello.Collection<Article>().All.First().ID);
            Assert.AreEqual(barbieDoll.ID, _marcello.Collection<Article>().All.Last().ID);
        }
    }
}

