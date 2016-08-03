﻿using System;
using MarcelloDB.Collections;
using MarcelloDB.Records;
using MarcelloDB.Index;

namespace MarcelloDB.Collections
{
    public class IndexedIDValue<TObj, TID> : IndexedValue<TObj, TID>
    {
        internal IndexedIDValue():base(null)
        {
            this.PropertyName = "ID";
        }

        internal Func<TObj, TID> IDValueFunction { get; set; }

        internal override object GetKey(object o, long address)
        {
            return IDValueFunction((TObj)o);
        }

        internal  override void RegisterKey(object key,
            Int64 address,
            Session session,
            RecordManager recordManager,
            string indexName)
        {
            var index = new RecordIndex<TID>(
                this.Session,
                recordManager,
                indexName,
                this.Session.SerializerResolver.SerializerFor<Node<TID>>()
            );

            index.Register((TID)key, address);
        }

        internal  override void UnRegisterKey(object key,
            Int64 address,
            Session session,
            RecordManager recordManager,
            string indexName)
        {
            var index = new RecordIndex<TID>(
                this.Session,
                recordManager,
                indexName,
                this.Session.SerializerResolver.SerializerFor<Node<TID>>()
            );

            index.UnRegister((TID)key);
        }
    }
}
