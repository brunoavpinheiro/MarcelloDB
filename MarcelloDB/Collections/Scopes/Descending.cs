﻿using System;
using MarcelloDB.Records;
using MarcelloDB.Collections;

namespace MarcelloDB.Collections.Scopes
{
    public class Descending<TObj, TAttribute> : BaseScope<TObj, TAttribute>
    {
        BaseScope <TObj, TAttribute> OriginalScope { get; set; }

        internal Descending(BaseScope <TObj, TAttribute> originalScope)
        {
            this.OriginalScope = originalScope;
        }

        internal override CollectionEnumerator<TObj, ValueWithAddressIndexKey<TAttribute>> BuildEnumerator(bool descending)
        {
            return OriginalScope.BuildEnumerator(true); //force descending enumerator
        }
    }
}

