﻿using System;
using MarcelloDB.Records;
using MarcelloDB.Index;
using System.Collections.Generic;

namespace MarcelloDB.Collections
{
    public class CompoundIndexedValue<TObj, TAtt1, TAtt2> : BaseIndexedValue<TObj, CompoundValue<TAtt1 ,TAtt2>>
    {
        CompoundIndexedValue(): base(null){
        }

        internal CompoundIndexedValue(Func<TObj, CompoundValue<TAtt1, TAtt2>> valueFunction)
            :base(valueFunction){}

        public IEnumerable<TObj> Find(TAtt1 val1, TAtt2 val2)
        {
            return base.FindInternal(CompoundValue.Build(val1, val2));
        }
    }
}
