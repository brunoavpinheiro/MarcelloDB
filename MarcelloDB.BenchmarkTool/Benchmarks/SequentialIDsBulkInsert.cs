﻿using System;
using System.Diagnostics;

namespace MarcelloDB.BenchmarkTool.Benchmarks
{
    public class SequentialIDsBulkInsert : Base
    {
       
        int _objectCount;

        public SequentialIDsBulkInsert(int objectCount)
        {
            _objectCount = objectCount;
        }
            
        protected override void OnRun()
        {
            for (int i = 1; i < _objectCount; i++)
            {
                var a = new TestClass{ID = i, Name = "Object " + i.ToString()};
                this.Collection.Persist(a);
            }
        }               
    }        
}




