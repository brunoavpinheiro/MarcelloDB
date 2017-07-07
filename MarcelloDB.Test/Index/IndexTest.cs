﻿using System;
using NUnit.Framework;
using MarcelloDB.Index;
using System.Collections.Generic;
using MarcelloDB.netfx;

namespace MarcelloDB.Test.Index
{
    [TestFixture]
    public class IndexTest
    {
        MockBTree<object> _mockTree;
        MockBTreeDataProvider<object> _mockProvider;

        RecordIndex<object> _index;
        Session _session;

        [SetUp]
        public void Initialize()
        {
            _session = new Session(new TestPlatform(), "");
            _mockTree = new MockBTree<object>();
            _mockProvider = new MockBTreeDataProvider<object>();
            _index = new RecordIndex<object>(_session, _mockTree, _mockProvider);
        }

        [Test]
        public void Search_Returns_Zero_When_Index_Empty()
        {
            Assert.AreEqual(0, _index.Search(123));
        }

        [Test]
        public void Search_Returns_Zero_When_Key_Not_Found()
        {
            _mockTree.Insert(1, 2);
            Assert.AreEqual(0, _index.Search(123));
        }

        [Test]
        public void Search_Returns_Record_Address_When_Key_Found()
        {
            _mockTree.Insert(1, 2);
            Assert.AreEqual(2, _index.Search(1));
        }

        [Test]
        public void Register_Inserts_When_Key_Not_Present()
        {
            _index.Register(1, 2);
            Assert.AreEqual(2, _mockTree.Inserted[1]);
        }

        [Test]
        public void Register_Inserts_New_Value_On_Update()
        {
            _mockTree.Insert(1, 2);
            _index.Register(1, 3);
            Assert.AreEqual(3, _mockTree.Inserted[1]);
            Assert.AreEqual("Insert", _mockTree.LastAction);
        }

        [Test]
        public void Register_Flushes_DataProvider()
        {
            _index.Register(1, 123);
            Assert.IsTrue(_mockProvider.WasFlushed);
        }

        [Test]
        public void UnRegister_Deletes_Original_Value()
        {
            _mockTree.Insert(1, 2);
            _index.UnRegister(1);
            Assert.AreEqual(1, _mockTree.Deleted[0]);
        }

        [Test]
        public void UnRegister_Flushes_DataProvider()
        {
            _index.UnRegister(1);
            Assert.IsTrue(_mockProvider.WasFlushed);
        }
    }
}

