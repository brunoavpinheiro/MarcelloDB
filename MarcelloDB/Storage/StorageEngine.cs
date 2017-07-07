﻿using System;
using MarcelloDB;
using MarcelloDB.Storage.StreamActors;
using MarcelloDB.Transactions;

namespace MarcelloDB.Storage
{
    public class StorageEngine : SessionBoundObject
    {

        string StreamName { get; set; }

        public StorageEngine(Session session, string streamName) : base(session)
        {
            this.StreamName = streamName;
        }

        public byte[] Read(long address, int length)
        {
            return Reader().Read(address, length);
        }

        public void Write(long address, byte[] bytes)
        {
            Writer().Write(address, bytes);
        }

        #region reader/writer factories
        Writer Writer()
        {
            return new JournalledWriter(this.Session, this.StreamName);
        }

        Reader Reader()
        {
            return new JournalledReader(this.Session, this.StreamName);
        }
        #endregion
    }
}
