﻿using System;
using System.Diagnostics;
using System.Collections.Generic;
using MarcelloDB.BenchmarkTool.DataClasses;
using System.IO;
using MarcelloDB.netfx;

namespace MarcelloDB.BenchmarkTool.Benchmarks
{
    public class Base
    {
        protected MarcelloDB.Collections.Collection<Person, int, PersonIndexes> Collection {get; set;}

        public Base()
        {

        }

        protected virtual void OnSetup(){}

        protected virtual void OnRun()
        {
        }

        public TimeSpan Run()
        {
            Stopwatch w;
            var dataPath = Path.Combine(Environment.CurrentDirectory, "data");
            EnsureFolder(dataPath);
            var platform = new MarcelloDB.netfx.Platform();
            using (var session = new Session(platform, dataPath))
            {
                this.Collection = session["data"].Collection<Person, int, PersonIndexes>("persons", p => p.ID);
                OnSetup();

                w = Stopwatch.StartNew();
                OnRun();
            }

            w.Stop();
            return w.Elapsed;
        }

        private void EnsureFolder(string path)
        {
            if(System.IO.Directory.Exists(path))
            {
                foreach( var file in System.IO.Directory.EnumerateFiles(path))
                {
                    System.IO.File.Delete(file);
                }
            }
            else
            {
                System.IO.Directory.CreateDirectory(path);
            }
        }
    }
}

