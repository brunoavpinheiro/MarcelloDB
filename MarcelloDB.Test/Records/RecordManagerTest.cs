﻿using System;
using NUnit.Framework;
using MarcelloDB.Records;
using MarcelloDB.Test.Classes;
using MarcelloDB.Storage;
using MarcelloDB.AllocationStrategies;
using MarcelloDB.Platform;
using MarcelloDB.Collections;
using MarcelloDB.netfx;

namespace MarcelloDB.Test.Records
{
    [TestFixture]
    public class RecordManagerTest
    {
        public class TestAllocationStrategy : IAllocationStrategy
        {
            public int Size {get;set;}
            #region IAllocationStrategy implementation
            public int CalculateSize(int dataSize)
            {
                return this.Size;
            }
            #endregion
        }

        IPlatform _platform;
        InMemoryStreamProvider _streamProvider;
        RecordManager _recordManager;
        Session _session;
        IAllocationStrategy _allocationStrategy;

        [SetUp]
        public void Initialize()
        {
            _platform = new TestPlatform();
            _streamProvider = (InMemoryStreamProvider)_platform.CreateStorageStreamProvider("/");
            _session = new Session(_platform, "/");
            _allocationStrategy = _session.AllocationStrategyResolver.StrategyFor(new object());
            _recordManager = new RecordManager(
                _session,
                new StorageEngine(_session, "article"));
        }

        [Test]
        public void Append_Record_Returns_Record()
        {
            var record = _recordManager.AppendRecord(new byte[0], _allocationStrategy);
            Assert.NotNull(record);
        }

        [Test]
        public void Get_Record_Returns_Appended_Record()
        {
            var record = _recordManager.AppendRecord(new byte[3]{ 1, 2, 3 }, _allocationStrategy);
            var readRecord = _recordManager.GetRecord(record.Header.Address);
            Assert.AreEqual(new byte[3]{ 1, 2, 3 }, readRecord.Data);
        }

        [Test]
        public void Update_Record_Returns_Updated_Record()
        {
            var record = _recordManager.AppendRecord(new byte[3] { 1, 2, 3 }, _allocationStrategy);
            record = _recordManager.UpdateRecord(record, new byte[3]{ 4, 5, 6 }, _allocationStrategy);
            Assert.AreEqual(new byte[3]{ 4, 5, 6}, record.Data);
        }

        [Test]
        public void Append_Record_Assigns_Address()
        {
            var record = _recordManager.AppendRecord(new byte[0], _allocationStrategy);
            Assert.Greater(record.Header.Address, 0);
        }

        [Test]
        public void Uses_Overridden_AllocationStrategy()
        {
            var allocationStrategy = new TestAllocationStrategy { Size = 200 };
            var record = _recordManager.AppendRecord(new byte[10], allocationStrategy:  allocationStrategy);
            Assert.AreEqual(allocationStrategy.Size, record.Header.AllocatedDataSize);
        }

        [Test]
        public void Update_Record_Uses_Overridden_Allocation_Strategy()
        {
            var allocationStrategy = new TestAllocationStrategy { Size = 10 };
            var record = _recordManager.AppendRecord(new byte[10], allocationStrategy:  allocationStrategy);
            allocationStrategy.Size = 200;
            var updatedRecord = _recordManager.UpdateRecord(record, new byte[40], allocationStrategy: allocationStrategy);
            Assert.AreEqual(allocationStrategy.Size, updatedRecord.Header.AllocatedDataSize);
        }

        [Test]
        public void Update_Record_Updates_Record()
        {
            var record = _recordManager.AppendRecord(new byte[3]{ 1, 2, 3 }, _allocationStrategy);
            _recordManager.UpdateRecord(record, new byte[3] { 4, 5, 6 }, _allocationStrategy);
            var readRecord = _recordManager.GetRecord(record.Header.Address);
            Assert.AreEqual(new byte[3]{ 4, 5, 6}, readRecord.Data);
        }

        [Test]
        public void Update_Record_Doesnt_Increase_StorageSize()
        {
            var record = _recordManager.AppendRecord(new byte[3]{ 1, 2, 3 }, _allocationStrategy);
            var streamLength =  GetStreamLength();
            _recordManager.UpdateRecord(record, new byte[3] { 4, 5, 6 }, _allocationStrategy);
            var newStreamLength =  GetStreamLength();
            Assert.AreEqual(streamLength, newStreamLength);
        }

        [Test]
        public void Get_Named_Record_Address_Returns_Null_When_Not_Registered()
        {
            var result = _recordManager.GetNamedRecordAddress("Test");
            Assert.AreEqual(0, result, "Should be nul");
        }

        [Test]
        public void Stores_Named_Record_Address(){
            _recordManager.RegisterNamedRecordAddress("Test", 123);
            Assert.AreEqual(123, _recordManager.GetNamedRecordAddress("Test"));
        }

        [Test]
        public void Store_Multiple_Named_Record_Addresses()
        {
            _recordManager.RegisterNamedRecordAddress("Test1", 123);
            _recordManager.RegisterNamedRecordAddress("Test2", 456);
            Assert.AreEqual(123, _recordManager.GetNamedRecordAddress("Test1"));
            Assert.AreEqual(456, _recordManager.GetNamedRecordAddress("Test2"));
        }

        [Test]
        public void Append_Record_Reuses_Empty_Record()
        {
            var record = _recordManager.AppendRecord(new byte[3]{1, 2, 3}, _allocationStrategy);

            _recordManager.Recycle(record.Header.Address);

            var expectedLength = GetStreamLength();
            _recordManager.AppendRecord(new byte[3]{1, 2, 3}, _allocationStrategy);
            var newLength = GetStreamLength();
            Assert.AreEqual(expectedLength, newLength);
        }

        [Test]
        public void Record_Does_Not_Get_Recycled_Twice()
        {
            var record = _recordManager.AppendRecord(new byte[3]{ 1, 2, 3 }, _allocationStrategy);
            _recordManager.Recycle(record.Header.Address);

            var firstNewRecord = _recordManager.AppendRecord(new byte[3]{ 4, 5, 6 }, _allocationStrategy);
            var secondNewRecord = _recordManager.AppendRecord(new byte[3]{ 7, 8, 9 }, _allocationStrategy);
            Assert.AreNotEqual(firstNewRecord.Header.Address, secondNewRecord.Header.Address);
        }

        [Test]
        public void Update_Record_Reuses_Empty_Record()
        {
            var smallRecord = _recordManager.AppendRecord(new byte[1]{ 1 }, _allocationStrategy);
            var giantRecord = _recordManager.AppendRecord(new byte[100], _allocationStrategy);
            _recordManager.Recycle(giantRecord.Header.Address);
            _recordManager.SaveState();
            var expectedLength = GetStreamLength();
            _recordManager.UpdateRecord(smallRecord, new byte[20], _allocationStrategy);
            var newLength = GetStreamLength();
            Assert.AreEqual(expectedLength, newLength);

        }

        Int64 GetStreamLength()
        {
            return ((InMemoryStream)_streamProvider.GetStream("Article")).BackingStream.Length;
        }
    }
}

