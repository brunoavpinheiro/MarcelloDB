﻿using System.Linq;
using NUnit.Framework;
using System.Collections.Generic;
using System;
using Marcello.Index;

namespace Marcello.Test.Index
{
    public class TestDataProvider : IBTreeDataProvider<int, int>
    {
        Int64 _lastAddress = 1;
        Dictionary<Int64, Node<int,int>> _nodes = new Dictionary<long, Node<int, int>>();

        Node<int, int> _rootNode;

        public Node<int, int> GetRootNode(int degree)
        {
            if (_rootNode == null)
            {
                _rootNode = new Node<int, int>(degree);
                _rootNode.Address = 0;
                AddNode(_rootNode);
            }
            return _rootNode;
        }

        public Node<int, int> GetNode(Int64 address)
        {
            return _nodes[address];
        }

        public Node<int, int> CreateNode(int degree)
        {
            var node = new Node<int, int>(degree);
            node.Address = _lastAddress++;
            AddNode(node);
            return node;
        }                               

        void AddNode(Node<int,int> node)
        {
            _nodes.Add(node.Address, node);
        }


        public void Flush()
        {
        
        }
    }

    [TestFixture]
    public class BTreeTest
    {
        private TestDataProvider _testDataProvider;
        private const int Degree = 2;

        private readonly int[] testKeyData = new int[] { 10, 20, 30, 50 };
        private readonly int[] testPointerData = new int[] { 50, 60, 40, 20 };


        [SetUp]
        public void Initialize()
        {
            _testDataProvider = new TestDataProvider();
        }

        [Test]
        public void CreateBTree()
        {
            var btree = new BTree<int, int>(_testDataProvider, Degree);

            Node<int, int> root = btree.Root;
            Assert.IsNotNull(root);
            Assert.IsNotNull(root.Entries);
            Assert.IsNotNull(root.ChildrenAddresses);
            Assert.AreEqual(0, root.Entries.Count);
            Assert.AreEqual(0, root.ChildrenAddresses.Count);
        }

        [Test]
        public void InsertOneNode()
        {
            var btree = new BTree<int, int>(_testDataProvider, Degree);
            this.InsertTestDataAndValidateTree(btree, 0);
            Assert.AreEqual(1, btree.Height);
        }

        [Test]
        public void InsertMultipleNodesToSplit()
        {
            var btree = new BTree<int, int>(_testDataProvider, Degree);

            for (int i = 0; i < this.testKeyData.Length; i++)
            {
                this.InsertTestDataAndValidateTree(btree, i);
            }

            Assert.AreEqual(2, btree.Height);
        }

        [Test]
        public void DeleteNodes()
        {
            var btree = new BTree<int, int>(_testDataProvider, Degree);

            for (int i = 0; i < this.testKeyData.Length; i++)
            {
                this.InsertTestData(btree, i);
            }

            for (int i = 0; i < this.testKeyData.Length; i++)
            {
                btree.Delete(this.testKeyData[i]);
                ValidateTree(btree.Root, Degree, this.testKeyData.Skip(i + 1).ToArray());
            }

            Assert.AreEqual(1, btree.Height);
        }

        [Test]
        public void DeleteNonExistingNode()
        {
            var btree = new BTree<int, int>(_testDataProvider, Degree);

            for (int i = 0; i < this.testKeyData.Length; i++)
            {
                this.InsertTestData(btree, i);
            }

            btree.Delete(99999);
            ValidateTree(btree.Root, Degree, this.testKeyData.ToArray());
        }

        [Test]
        public void SearchNodes()
        {
            var btree = new BTree<int, int>(_testDataProvider, Degree);

            for (int i = 0; i < this.testKeyData.Length; i++)
            {
                this.InsertTestData(btree, i);
                this.SearchTestData(btree, i);
            }
        }

        [Test]
        public void SearchNonExistingNode()
        {
            var btree = new BTree<int, int>(_testDataProvider, Degree);

            // search an empty tree
            Entry<int, int> nonExisting = btree.Search(9999);
            Assert.IsNull(nonExisting);

            for (int i = 0; i < this.testKeyData.Length; i++)
            {
                this.InsertTestData(btree, i);
                this.SearchTestData(btree, i);
            }

            // search a populated tree
            nonExisting = btree.Search(9999);
            Assert.IsNull(nonExisting);
        }

        #region Private Helper Methods
        private void InsertTestData(BTree<int, int> btree, int testDataIndex)
        {
            btree.Insert(this.testKeyData[testDataIndex], this.testPointerData[testDataIndex]);
        }

        private void InsertTestDataAndValidateTree(BTree<int, int> btree, int testDataIndex)
        {
            btree.Insert(this.testKeyData[testDataIndex], this.testPointerData[testDataIndex]);
            ValidateTree(btree.Root, Degree, this.testKeyData.Take(testDataIndex + 1).ToArray());
        }

        private void SearchTestData(BTree<int, int> btree, int testKeyDataIndex)
        {
            for (int i = 0; i <= testKeyDataIndex; i++)
            {
                Entry<int, int> entry = btree.Search(this.testKeyData[i]);
                Assert.IsNotNull(this.testKeyData[i]);
                Assert.AreEqual(this.testKeyData[i], entry.Key);
                Assert.AreEqual(this.testPointerData[i], entry.Pointer);
            }
        }
                   
        public void ValidateTree(Node<int, int> tree, int degree, params int[] expectedKeys)
        {
            var foundKeys = new Dictionary<int, List<Entry<int, int>>>();
            ValidateSubtree(tree, tree, degree, int.MinValue, int.MaxValue, foundKeys);

            Assert.AreEqual(0, expectedKeys.Except(foundKeys.Keys).Count());
            foreach (var keyValuePair in foundKeys)
            {
                Assert.AreEqual(1, keyValuePair.Value.Count);
            }
        }

        private void UpdateFoundKeys(Dictionary<int, List<Entry<int, int>>> foundKeys, Entry<int, int> entry)
        {
            List<Entry<int, int>> foundEntries;
            if (!foundKeys.TryGetValue(entry.Key, out foundEntries))
            {
                foundEntries = new List<Entry<int, int>>();
                foundKeys.Add(entry.Key, foundEntries);
            }

            foundEntries.Add(entry);
        }

        private void ValidateSubtree(Node<int, int> root, Node<int, int> node, int degree, int nodeMin, int nodeMax, Dictionary<int, List<Entry<int, int>>> foundKeys)
        {
            if (root != node)
            {
                Assert.IsTrue(node.Entries.Count >= degree - 1);
                Assert.IsTrue(node.Entries.Count <= (2 * degree) - 1);
            }

            for (int i = 0; i <= node.Entries.Count; i++)
            {
                int subtreeMin = nodeMin;
                int subtreeMax = nodeMax;

                if (i < node.Entries.Count)
                {
                    var entry = node.Entries[i];
                    UpdateFoundKeys(foundKeys, entry);
                    Assert.IsTrue(entry.Key >= nodeMin && entry.Key <= nodeMax);

                    subtreeMax = entry.Key;
                }

                if (i > 0)
                {
                    subtreeMin = node.Entries[i - 1].Key;
                }

                if (!node.IsLeaf)
                {
                    Assert.IsTrue(node.ChildrenAddresses.Count >= degree);
                    ValidateSubtree(root, _testDataProvider.GetNode(node.ChildrenAddresses[i]), degree, subtreeMin, subtreeMax, foundKeys);
                }
            }
        }

        #endregion
    }
}