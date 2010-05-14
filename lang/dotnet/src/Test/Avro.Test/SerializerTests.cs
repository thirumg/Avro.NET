using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using Avro.IO;
namespace Avro.Test
{
    [TestFixture]
    public class SerializerTests
    {
        [TestFixtureSetUp]
        public void Setup()
        {
            Logger.ConfigureForUnitTesting();
        }

        Random random = new Random();
        const int ITERATIONS = 1000;
        [TestCase]
        public void IntTests()
        {
            PrimitiveSchema schema = new PrimitiveSchema("int");
            
            
            object[] data = new object[ITERATIONS];
            for (int i = 0; i < ITERATIONS; i++)
            {
                data[i] = random.Next();
            }

            TestData(schema, BinaryEncoder.Instance, BinaryDecoder.Instance, data);
        }
        [TestCase]
        public void LongTests()
        {
            PrimitiveSchema schema = new PrimitiveSchema("long");

            object[] data = new object[ITERATIONS];
            for (int i = 0; i < ITERATIONS; i++)
            {
                data[i] = (long)random.Next();
            }

            TestData(schema, BinaryEncoder.Instance, BinaryDecoder.Instance, data);
        }
        [TestCase]
        public void BooleanTests()
        {
            PrimitiveSchema schema = new PrimitiveSchema("boolean");

            object[] data = new object[ITERATIONS];
            for (int i = 0; i < ITERATIONS; i++)
            {
                data[i] = random.Next() % 2==1;
            }

            TestData(schema, BinaryEncoder.Instance, BinaryDecoder.Instance, data);
        }

        [TestCase]
        public void StringTests()
        {
            PrimitiveSchema schema = new PrimitiveSchema("string");
            
            
            object[] data = new object[ITERATIONS];
            for (int i = 0; i < ITERATIONS; i++)
            {
                data[i] = RandomDataHelper.GetString(1, 5000);
            }

            TestData(schema, BinaryEncoder.Instance, BinaryDecoder.Instance, data);
        }

        const string MAPTESTING="Map Serialization";

        [Category(MAPTESTING)]
        [TestCase]
        public void MapTests_Bool()
        {
            Dictionary<string, bool> expected = new Dictionary<string, bool>();

            for (int i = 0; i < ITERATIONS; i++)
            {
                string key = string.Format("Key{0:########0}", i);
                bool value = RandomDataHelper.GetBool();
                expected.Add(key, value);
            }

            Schema valueSchema = new PrimitiveSchema("string");
            MapSchema mapSchema = new MapSchema(valueSchema);

            using (MemoryStream iostr = new MemoryStream())
            {
                Serializer.Serialize(PrefixStyle.None, mapSchema, iostr, BinaryEncoder.Instance, expected);
                iostr.Position = 0;
                Assert.Greater(iostr.Length, 0, "Serialized length should be greater than 0.");
                IDictionary<string, bool> actual = Serializer.Deserialize<IDictionary<string, bool>>(PrefixStyle.None, mapSchema, iostr, BinaryDecoder.Instance);
                Assert.NotNull(actual, "actual should not be null");
                Assert.Greater(actual.Count, 0, "Deserialized Length should be greater than 0.");
                Assert.AreEqual(expected.Count, actual.Count, "expect and actual should == {0}", actual.Count);

                foreach (KeyValuePair<string, bool> expectedEntry in expected)
                {
                    Assert.IsTrue(actual.ContainsKey(expectedEntry.Key), "actual does not contain key \"{0}\"", expectedEntry.Key);
                    bool actualValue = actual[expectedEntry.Key];
                    Assert.AreEqual(expectedEntry.Value, actualValue, "Value for key \"{0}\" did not match", expectedEntry.Key);
                }
            }
        }
        [Category(MAPTESTING)]
        [TestCase]
        public void MapTests_Int()
        {
            Dictionary<string, int> expected = new Dictionary<string, int>();

            for (int i = 0; i < ITERATIONS; i++)
            {
                string key = string.Format("Key{0:########0}", i);
                int value = RandomDataHelper.GetRandomInt32();
                expected.Add(key, value);
            }

            Schema valueSchema = new PrimitiveSchema("string");
            MapSchema mapSchema = new MapSchema(valueSchema);

            using (MemoryStream iostr = new MemoryStream())
            {
                Serializer.Serialize(PrefixStyle.None, mapSchema, iostr, BinaryEncoder.Instance, expected);
                iostr.Position = 0;
                Assert.Greater(iostr.Length, 0, "Serialized length should be greater than 0.");
                IDictionary<string, int> actual = Serializer.Deserialize<IDictionary<string, int>>(PrefixStyle.None, mapSchema, iostr, BinaryDecoder.Instance);
                Assert.NotNull(actual, "actual should not be null");
                Assert.Greater(actual.Count, 0, "Deserialized Length should be greater than 0.");
                Assert.AreEqual(expected.Count, actual.Count, "expect and actual should == {0}", actual.Count);

                foreach (KeyValuePair<string, int> expectedEntry in expected)
                {
                    Assert.IsTrue(actual.ContainsKey(expectedEntry.Key), "actual does not contain key \"{0}\"", expectedEntry.Key);
                    int actualValue = actual[expectedEntry.Key];
                    Assert.AreEqual(expectedEntry.Value, actualValue, "Value for key \"{0}\" did not match", expectedEntry.Key);
                }
            }
        }
        [Category(MAPTESTING)]
        [TestCase]
        public void MapTests_Long()
        {
            Dictionary<string, long> expected = new Dictionary<string, long>();

            for (int i = 0; i < ITERATIONS; i++)
            {
                string key = string.Format("Key{0:########0}", i);
                long value = RandomDataHelper.GetRandomInt64();
                expected.Add(key, value);
            }

            Schema valueSchema = new PrimitiveSchema("string");
            MapSchema mapSchema = new MapSchema(valueSchema);

            using (MemoryStream iostr = new MemoryStream())
            {
                Serializer.Serialize(PrefixStyle.None, mapSchema, iostr, BinaryEncoder.Instance, expected);
                iostr.Position = 0;
                Assert.Greater(iostr.Length, 0, "Serialized length should be greater than 0.");
                IDictionary<string, long> actual = Serializer.Deserialize<IDictionary<string, long>>(PrefixStyle.None, mapSchema, iostr, BinaryDecoder.Instance);
                Assert.NotNull(actual, "actual should not be null");
                Assert.Greater(actual.Count, 0, "Deserialized Length should be greater than 0.");
                Assert.AreEqual(expected.Count, actual.Count, "expect and actual should == {0}", actual.Count);

                foreach (KeyValuePair<string, long> expectedEntry in expected)
                {
                    Assert.IsTrue(actual.ContainsKey(expectedEntry.Key), "actual does not contain key \"{0}\"", expectedEntry.Key);
                    long actualValue = actual[expectedEntry.Key];
                    Assert.AreEqual(expectedEntry.Value, actualValue, "Value for key \"{0}\" did not match", expectedEntry.Key);
                }
            }
        }
        [Category(MAPTESTING)]
        [TestCase]
        public void MapTests_Float()
        {
            Dictionary<string, float> expected = new Dictionary<string, float>();

            for (int i = 0; i < ITERATIONS; i++)
            {
                string key = string.Format("Key{0:########0}", i);
                float value = RandomDataHelper.GetRandomFloat();
                expected.Add(key, value);
            }

            Schema valueSchema = new PrimitiveSchema("string");
            MapSchema mapSchema = new MapSchema(valueSchema);

            using (MemoryStream iostr = new MemoryStream())
            {
                Serializer.Serialize(PrefixStyle.None, mapSchema, iostr, BinaryEncoder.Instance, expected);
                iostr.Position = 0;
                Assert.Greater(iostr.Length, 0, "Serialized length should be greater than 0.");
                IDictionary<string, float> actual = Serializer.Deserialize<IDictionary<string, float>>(PrefixStyle.None, mapSchema, iostr, BinaryDecoder.Instance);
                Assert.NotNull(actual, "actual should not be null");
                Assert.Greater(actual.Count, 0, "Deserialized Length should be greater than 0.");
                Assert.AreEqual(expected.Count, actual.Count, "expect and actual should == {0}", actual.Count);

                foreach (KeyValuePair<string, float> expectedEntry in expected)
                {
                    Assert.IsTrue(actual.ContainsKey(expectedEntry.Key), "actual does not contain key \"{0}\"", expectedEntry.Key);
                    float actualValue = actual[expectedEntry.Key];
                    Assert.AreEqual(expectedEntry.Value, actualValue, "Value for key \"{0}\" did not match", expectedEntry.Key);
                }
            }
        }
        [TestCase]
        public void RecordTest_Simple()
        {
            RecordTest expected = new RecordTest();
            expected.Test = "This is a test value";

            RecordSchema schema = new RecordSchema(new Name("RecordTest", null));
            Field testField = new Field(new PrimitiveSchema("string"), "Test", false);
            schema.AddField(testField);

            

            using (MemoryStream iostr = new MemoryStream())
            {
                Serializer.Serialize(PrefixStyle.None, schema, iostr, BinaryEncoder.Instance, expected);

            }


        }

        [Record]
        public class RecordTest
        {
            [Field]
            public string Test { get; set; }
        }




        [Category(MAPTESTING)]
        [TestCase]
        public void MapTests_String()
        {
            Dictionary<string, string> expected = new Dictionary<string, string>();

            for (int i = 0; i < ITERATIONS; i++)
            {
                string key = string.Format("Key{0:########0}", i);
                string value = RandomDataHelper.GetString(50, 5000);
                expected.Add(key, value);
            }

            Schema valueSchema = new PrimitiveSchema("string");
            MapSchema mapSchema = new MapSchema(valueSchema);

            using (MemoryStream iostr = new MemoryStream())
            {
                Serializer.Serialize(PrefixStyle.None, mapSchema, iostr, BinaryEncoder.Instance, expected);
                iostr.Position = 0;
                Assert.Greater(iostr.Length, 0, "Serialized length should be greater than 0.");
                IDictionary<string, string> actual = Serializer.Deserialize<IDictionary<string, string>>(PrefixStyle.None, mapSchema, iostr, BinaryDecoder.Instance);
                Assert.NotNull(actual, "actual should not be null");
                Assert.Greater(actual.Count, 0, "Deserialized Length should be greater than 0.");
                Assert.AreEqual(expected.Count, actual.Count, "expect and actual should == {0}", actual.Count);

                foreach (KeyValuePair<string, string> expectedEntry in expected)
                {
                    Assert.IsTrue(actual.ContainsKey(expectedEntry.Key), "actual does not contain key \"{0}\"", expectedEntry.Key);
                    string actualValue = actual[expectedEntry.Key];
                    Assert.IsNotNullOrEmpty(actualValue, "value for key \"{0}\" is null or empty.", expectedEntry.Key);
                    Assert.AreEqual(expectedEntry.Value, actualValue, "Value for key \"{0}\" did not match", expectedEntry.Key); 
                }
            }
        }

        void TestData(Schema schema, Encoder encoder, Decoder decoder, object[] data)
        {
            for (int i = 0; i < data.Length; i++)
            {
                object expected = data[i];
                Type expectedType = data[i].GetType();

                using (MemoryStream iostr = new MemoryStream())
                {
                    Serializer.Serialize(PrefixStyle.None, schema, iostr, encoder, expected);
                    iostr.Position = 0;
                    object actual = Serializer.Deserialize(PrefixStyle.None, schema, iostr, decoder, expectedType);
                    Assert.AreEqual(expected, actual);


                }
            }


        }

    }
}
