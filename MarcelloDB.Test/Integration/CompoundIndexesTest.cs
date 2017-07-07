﻿using System;
using NUnit.Framework;
using MarcelloDB.Collections;
using MarcelloDB.Index;
using System.Linq;
using MarcelloDB.netfx;

namespace MarcelloDB.Test.Integration.CompoundIndexesTest
{
    public class Indexable
    {
        public int     ID    { get; set; }
        public int     Prop1 { get; set; }
        public long    Prop2 { get; set; }
        public double  Prop3 { get; set; }
        public string  Prop4 { get; set; }
        public int     Prop5 { get; set; }
        public long    Prop6 { get; set; }
        public double  Prop7 { get; set; }
        public string  Prop8 { get; set; }

        public static Indexable CreateIndexable(int i)
        {
            return new Indexable
            {
                ID    = i,
                Prop1 = i,
                Prop2 = i,
                Prop3 = i,
                Prop4 = i.ToString(),
                Prop5 = i,
                Prop6 = i,
                Prop7 = i,
                Prop8 = i.ToString()
            };
        }
    }

    [TestFixture]
    public class CompoundIndexesTest
    {
        Session _session;
        CollectionFile _collectionFile;

        TestPlatform _platform;

        [SetUp]
        public void Setup()
        {
            _platform = new TestPlatform();
            _session = new Session(_platform, "/");
            _collectionFile = _session["indexables"];
        }

        class IndexableIndexDefinition2 : IndexDefinition<Indexable>
        {
            public IndexedValue<Indexable, int, long> Compound1And2
            {
                get{
                    return IndexedValue((indexable) =>
                        {
                            return CompoundValue(indexable.Prop1, indexable.Prop2);
                        });
                }
            }
        }

        [Test]
        public void InsertCompoundOf2()
        {
            var indexables =
                _collectionFile.Collection<Indexable, int, IndexableIndexDefinition2>("indexables2", i => i.ID);
            indexables.Persist(Indexable.CreateIndexable(1));
            indexables.Persist(Indexable.CreateIndexable(2));
            indexables.Persist(Indexable.CreateIndexable(3));

            var item2 = indexables.Indexes.Compound1And2.Equals(2, 2).First();
            Assert.AreEqual(2, item2.ID);
        }

        class IndexableIndexDefinition3 : IndexDefinition<Indexable>
        {
            public IndexedValue<Indexable, int, long, double> Compound1And2And3
            {
                get{
                    return IndexedValue((indexable) =>
                        {
                            return CompoundValue(indexable.Prop1, indexable.Prop2, indexable.Prop3);
                        });
                }
            }
        }
        [Test]
        public void InsertCompoundOf3()
        {
            var indexables =
                _collectionFile.Collection<Indexable, int, IndexableIndexDefinition3>("indexables3", i => i.ID);
            indexables.Persist(Indexable.CreateIndexable(1));
            indexables.Persist(Indexable.CreateIndexable(2));
            indexables.Persist(Indexable.CreateIndexable(3));

            var item2 = indexables.Indexes.Compound1And2And3.Equals(2, 2, 2).First();
            Assert.AreEqual(2, item2.ID);
        }

        class IndexableIndexDefinition4 : IndexDefinition<Indexable>
        {
            public IndexedValue<Indexable, int, long, double, string> Compound1And2And3And4
            {
                get{
                    return IndexedValue((indexable) =>
                        {
                            return CompoundValue(
                                indexable.Prop1,
                                indexable.Prop2,
                                indexable.Prop3,
                                indexable.Prop4
                            );
                        });
                }
            }
        }

        [Test]
        public void InsertCompoundOf4()
        {
            var indexables =
                _collectionFile.Collection<Indexable, int, IndexableIndexDefinition4>("indexables3", i => i.ID);
            indexables.Persist(Indexable.CreateIndexable(1));
            indexables.Persist(Indexable.CreateIndexable(2));
            indexables.Persist(Indexable.CreateIndexable(3));

            var item2 = indexables.Indexes.Compound1And2And3And4.Equals(2, 2, 2, "2").First();
            Assert.AreEqual(2, item2.ID);
        }

        [Test]
        public void All()
        {
            var indexables =
                _collectionFile.Collection<Indexable, int, IndexableIndexDefinition4>("indexables4", i => i.ID);

            indexables.Persist(Indexable.CreateIndexable(3));
            indexables.Persist(Indexable.CreateIndexable(2));
            indexables.Persist(Indexable.CreateIndexable(1));
            var items = indexables.Indexes.Compound1And2And3And4.All.ToList();
                Assert.AreEqual(3, items.Count);

            Assert.AreEqual(1, items[0].ID);
            Assert.AreEqual(2, items[1].ID);
            Assert.AreEqual(3, items[2].ID);
        }

        [Test]
        public void GreaterThan3Compound_3Params()
        {
            var indexables =
                _collectionFile.Collection<Indexable, int, IndexableIndexDefinition3>("indexables3", i => i.ID);
            indexables.Persist(Indexable.CreateIndexable(1));
            indexables.Persist(Indexable.CreateIndexable(2));
            indexables.Persist(Indexable.CreateIndexable(3));

            var items = indexables.Indexes.Compound1And2And3.GreaterThan(2, 2, 1).ToList();

            Assert.AreEqual(2, items.Count);
            Assert.AreEqual(2, items[0].ID);
            Assert.AreEqual(3, items[1].ID);
        }

        [Test]
        public void GreaterThan3Compound_2Params()
        {
            var indexables =
                _collectionFile.Collection<Indexable, int, IndexableIndexDefinition3>("indexables3", i => i.ID);
            indexables.Persist(Indexable.CreateIndexable(1));
            indexables.Persist(Indexable.CreateIndexable(2));
            indexables.Persist(Indexable.CreateIndexable(3));

            var items = indexables.Indexes.Compound1And2And3.GreaterThan(2, 2).ToList();

            Assert.AreEqual(1, items.Count);
            Assert.AreEqual(3, items[0].ID);
        }

        [Test]
        public void GreaterOrEqualThan3Compound_3Params()
        {
            var indexables =
                _collectionFile.Collection<Indexable, int, IndexableIndexDefinition3>("indexables3", i => i.ID);
            indexables.Persist(Indexable.CreateIndexable(1));
            indexables.Persist(Indexable.CreateIndexable(2));
            indexables.Persist(Indexable.CreateIndexable(3));

            var items = indexables.Indexes.Compound1And2And3.GreaterThanOrEqual(2, 2, 1).ToList();

            Assert.AreEqual(2, items.Count);
            Assert.AreEqual(2, items[0].ID);
            Assert.AreEqual(3, items[1].ID);
        }

        [Test]
        public void GreaterThanOrEqual3Compound_2Params()
        {
            var indexables =
                _collectionFile.Collection<Indexable, int, IndexableIndexDefinition3>("indexables3", i => i.ID);
            indexables.Persist(Indexable.CreateIndexable(1));
            indexables.Persist(Indexable.CreateIndexable(2));
            indexables.Persist(Indexable.CreateIndexable(3));

            var items = indexables.Indexes.Compound1And2And3.GreaterThanOrEqual(2, 2).ToList();

            Assert.AreEqual(2, items.Count);
            Assert.AreEqual(2, items[0].ID);
            Assert.AreEqual(3, items[1].ID);
        }

        [Test]
        public void SmallerThan3Compound_3Params()
        {
            var indexables =
                _collectionFile.Collection<Indexable, int, IndexableIndexDefinition3>("indexables3", i => i.ID);
            indexables.Persist(Indexable.CreateIndexable(1));
            indexables.Persist(Indexable.CreateIndexable(2));
            indexables.Persist(Indexable.CreateIndexable(3));

            var items = indexables.Indexes.Compound1And2And3.SmallerThan(2, 2, 3).ToList();

            Assert.AreEqual(2, items.Count);
            Assert.AreEqual(1, items[0].ID);
            Assert.AreEqual(2, items[1].ID);
        }

        [Test]
        public void SmallerThan3Compound_2Params()
        {
            var indexables =
                _collectionFile.Collection<Indexable, int, IndexableIndexDefinition3>("indexables3", i => i.ID);
            indexables.Persist(Indexable.CreateIndexable(1));
            indexables.Persist(Indexable.CreateIndexable(2));
            indexables.Persist(Indexable.CreateIndexable(3));

            var items = indexables.Indexes.Compound1And2And3.SmallerThan(2, 2).ToList();

            Assert.AreEqual(1, items.Count);
            Assert.AreEqual(1, items[0].ID);
        }

        [Test]
        public void SmallerOrEqualThan3Compound_3Params()
        {
            var indexables =
                _collectionFile.Collection<Indexable, int, IndexableIndexDefinition3>("indexables3", i => i.ID);
            indexables.Persist(Indexable.CreateIndexable(1));
            indexables.Persist(Indexable.CreateIndexable(2));
            indexables.Persist(Indexable.CreateIndexable(3));

            var items = indexables.Indexes.Compound1And2And3.SmallerThanOrEqual(2, 2, 2).ToList();

            Assert.AreEqual(2, items.Count);
            Assert.AreEqual(1, items[0].ID);
            Assert.AreEqual(2, items[1].ID);
        }

        [Test]
        public void SmallerThanOrEqual3Compound_2Params()
        {
            var indexables =
                _collectionFile.Collection<Indexable, int, IndexableIndexDefinition3>("indexables3", i => i.ID);
            indexables.Persist(Indexable.CreateIndexable(1));
            indexables.Persist(Indexable.CreateIndexable(2));
            indexables.Persist(Indexable.CreateIndexable(3));

            var items = indexables.Indexes.Compound1And2And3.SmallerThanOrEqual(2, 2).ToList();
            Assert.AreEqual(2, items.Count);
            Assert.AreEqual(1, items[0].ID);
            Assert.AreEqual(2, items[1].ID);
        }

        [Test]
        public void Between3Compound_3Params()
        {
            var indexables =
                _collectionFile.Collection<Indexable, int, IndexableIndexDefinition3>("indexables3", i => i.ID);
            indexables.Persist(Indexable.CreateIndexable(1));
            indexables.Persist(Indexable.CreateIndexable(2));
            indexables.Persist(Indexable.CreateIndexable(3));

            var items = indexables.Indexes.Compound1And2And3.Between(1, 1, 1).And(3,3,3).ToList();

            Assert.AreEqual(1, items.Count);
            Assert.AreEqual(2, items[0].ID);
        }

        [Test]
        public void SmallerThan_Descending()
        {
            var indexables =
                _collectionFile.Collection<Indexable, int, IndexableIndexDefinition3>("indexables3", i => i.ID);
            indexables.Persist(Indexable.CreateIndexable(1));
            indexables.Persist(Indexable.CreateIndexable(2));
            indexables.Persist(Indexable.CreateIndexable(3));

            var items = indexables.Indexes.Compound1And2And3.SmallerThan(3).Descending.ToList();

            Assert.AreEqual(2, items.Count);
            Assert.AreEqual(2, items[0].ID);
            Assert.AreEqual(1, items[1].ID);
        }

        [Test]
        public void SmallerThan_Keys()
        {
            var indexables =
                _collectionFile.Collection<Indexable, int, IndexableIndexDefinition3>("indexables3", i => i.ID);
            indexables.Persist(Indexable.CreateIndexable(1));
            indexables.Persist(Indexable.CreateIndexable(2));
            indexables.Persist(Indexable.CreateIndexable(3));

            var keys = indexables.Indexes.Compound1And2And3.SmallerThan(3).Keys.ToList();

            Assert.AreEqual(2, keys.Count);
            Assert.AreEqual(1, keys[0].P1);
            Assert.AreEqual(2, keys[1].P1);
        }

        [Test]
        public void GreaterThan_Descending()
        {
            var indexables =
                _collectionFile.Collection<Indexable, int, IndexableIndexDefinition3>("indexables3", i => i.ID);
            indexables.Persist(Indexable.CreateIndexable(1));
            indexables.Persist(Indexable.CreateIndexable(2));
            indexables.Persist(Indexable.CreateIndexable(3));

            var items = indexables.Indexes.Compound1And2And3.GreaterThan(1).Descending.ToList();

            Assert.AreEqual(2, items.Count);
            Assert.AreEqual(3, items[0].ID);
            Assert.AreEqual(2, items[1].ID);
        }

        [Test]
        public void GreaterThan_Keys()
        {
            var indexables =
                _collectionFile.Collection<Indexable, int, IndexableIndexDefinition3>("indexables3", i => i.ID);
            indexables.Persist(Indexable.CreateIndexable(1));
            indexables.Persist(Indexable.CreateIndexable(2));
            indexables.Persist(Indexable.CreateIndexable(3));

            var keys = indexables.Indexes.Compound1And2And3.GreaterThanOrEqual(2).Keys.ToList();

            Assert.AreEqual(2, keys.Count);
            Assert.AreEqual(2, keys[0].P1);
            Assert.AreEqual(3, keys[1].P1);
        }
        [Test]
        public void Between3Compound_2Params()
        {
            var indexables =
                _collectionFile.Collection<Indexable, int, IndexableIndexDefinition3>("indexables3", i => i.ID);
            indexables.Persist(Indexable.CreateIndexable(1));
            indexables.Persist(Indexable.CreateIndexable(2));
            indexables.Persist(Indexable.CreateIndexable(3));

            var items = indexables.Indexes.Compound1And2And3.Between(1, 1).And(3,3).ToList();

            Assert.AreEqual(1, items.Count);
            Assert.AreEqual(2, items[0].ID);
        }

        [Test]
        public void BetweenIncluding3Compound_3Params()
        {
            var indexables =
                _collectionFile.Collection<Indexable, int, IndexableIndexDefinition3>("indexables3", i => i.ID);
            indexables.Persist(Indexable.CreateIndexable(1));
            indexables.Persist(Indexable.CreateIndexable(2));
            indexables.Persist(Indexable.CreateIndexable(3));

            var items = indexables.Indexes.Compound1And2And3.BetweenIncluding(1, 1, 1).AndIncluding(3,3,3).ToList();

            Assert.AreEqual(3, items.Count);
            Assert.AreEqual(1, items[0].ID);
            Assert.AreEqual(2, items[1].ID);
            Assert.AreEqual(3, items[2].ID);
        }

        [Test]
        public void BetweenIncluding3Compound_2Params()
        {
            var indexables =
                _collectionFile.Collection<Indexable, int, IndexableIndexDefinition3>("indexables3", i => i.ID);
            indexables.Persist(Indexable.CreateIndexable(1));
            indexables.Persist(Indexable.CreateIndexable(2));
            indexables.Persist(Indexable.CreateIndexable(3));
            var items = indexables.Indexes.Compound1And2And3.BetweenIncluding(1, 1).AndIncluding(3,3).ToList();

            Assert.AreEqual(3, items.Count);
            Assert.AreEqual(1, items[0].ID);
            Assert.AreEqual(2, items[1].ID);
            Assert.AreEqual(3, items[2].ID);
        }

        class IndexableIndexDefinitionUnique2 : IndexDefinition<Indexable>
        {
            public UniqueIndexedValue<Indexable, int, long> UniqueCompound1And2
            {
                get{
                    return UniqueIndexedValue((indexable) =>
                        {
                            return CompoundValue(indexable.Prop1, indexable.Prop2);
                        });
                }
            }
        }

        [Test]
        public void Insert_Unique_Compound_Of2()
        {
            var indexables =
                _collectionFile.Collection<Indexable, int, IndexableIndexDefinitionUnique2>("indexables_unique_compound", i => i.ID);
            var indexable = Indexable.CreateIndexable(1);
            indexables.Persist(indexable);
            var foundIndexable = indexables.Indexes.UniqueCompound1And2.Find(indexable.Prop1, indexable.Prop2);
            Assert.AreEqual(indexable.ID, foundIndexable.ID);
        }

        [Test]
        public void Allow_Updating_Object_In_Unique_Index()
        {
            var indexables =
                _collectionFile.Collection<Indexable, int, IndexableIndexDefinitionUnique2>("indexables_unique_compound", i => i.ID);
            var indexable = Indexable.CreateIndexable(1);
            indexables.Persist(indexable);
            indexable.Prop8 = "Updated";
            indexables.Persist(indexable);
            var foundIndexable = indexables.Indexes.UniqueCompound1And2.Find(indexable.Prop1, indexable.Prop2);
            Assert.AreEqual(indexable.ID, foundIndexable.ID);
        }

        [Test]
        public void Throws_DuplicateIndexEntryException_When_Inserting_Duplicate_Keys_In_Unique_Index()
        {
            var indexables =
                _collectionFile.Collection<Indexable, int, IndexableIndexDefinitionUnique2>("indexables_unique_compound", i => i.ID);
            var indexable = Indexable.CreateIndexable(1);
            indexables.Persist(indexable);

            var conflictingIndexable = Indexable.CreateIndexable(2);
            conflictingIndexable.Prop1 = indexable.Prop1;
            conflictingIndexable.Prop2 = indexable.Prop2;
            Assert.Throws<ArgumentException>(
                () => indexables.Persist(conflictingIndexable)
            );
        }

        class IndexDefinitionWithPredicateCompound2 : IndexDefinition<Indexable>
        {
            public IndexedValue<Indexable, string, string> Compound
            {
                get
                {
                    return IndexedValue(
                        indexable => CompoundValue(indexable.Prop4, indexable.Prop8),
                        indexable => indexable.Prop4 != null //only index if Prop4 isn't null
                    );
                }
            }
        }

        [Test]
        public void Insert_Optional_Compound_Of2()
        {
            var indexables =
                _collectionFile.Collection<Indexable, int, IndexDefinitionWithPredicateCompound2>("indexables_unique_compound", i => i.ID);
            for (int i = 1; i <= 3; i++)
            {
                var indexable = Indexable.CreateIndexable(i);
                indexables.Persist(indexable);
            }

            var indexableNotToIndex = new Indexable{ID = 123};
            indexables.Persist(indexableNotToIndex);

            var foundIndexables = indexables.Indexes.Compound.All;
            CollectionAssert.AreEquivalent(new int[]{ 1, 2, 3 }, foundIndexables.Select(i => i.ID).ToArray());

        }
    }
}

