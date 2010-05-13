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
        const int ITERATIONS = 10000;
        [TestCase]
        public void IntTests()
        {
            PrimitiveSchema schema = new PrimitiveSchema("int");
            Encoder encoder = new BinaryEncoder();
            Decoder decoder = new BinaryDecoder();
            object[] data = new object[ITERATIONS];
            for (int i = 0; i < ITERATIONS; i++)
            {
                data[i] = random.Next();
            }

            TestData(schema, encoder, decoder, data);
        }
        [TestCase]
        public void LongTests()
        {
            PrimitiveSchema schema = new PrimitiveSchema("long");
            Encoder encoder = new BinaryEncoder();
            Decoder decoder = new BinaryDecoder();
            object[] data = new object[ITERATIONS];
            for (int i = 0; i < ITERATIONS; i++)
            {
                data[i] = (long)random.Next();
            }

            TestData(schema, encoder, decoder, data);
        }
        [TestCase]
        public void BooleanTests()
        {
            PrimitiveSchema schema = new PrimitiveSchema("boolean");
            Encoder encoder = new BinaryEncoder();
            Decoder decoder = new BinaryDecoder();
            object[] data = new object[ITERATIONS];
            for (int i = 0; i < ITERATIONS; i++)
            {
                data[i] = random.Next() % 2==1;
            }

            TestData(schema, encoder, decoder, data);
        }

        [TestCase]
        public void StringTests()
        {
            PrimitiveSchema schema = new PrimitiveSchema("string");
            Encoder encoder = new BinaryEncoder();
            Decoder decoder = new BinaryDecoder();
            object[] data = new object[ITERATIONS];
            for (int i = 0; i < ITERATIONS; i++)
            {
                data[i] = RandomDataHelper.GetString(1, 5000);
            }

            TestData(schema, encoder, decoder, data);
        }
        [TestCase]
        public void MapTests_String()
        {
            Dictionary<string, string> expected = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            Encoder encoder = new BinaryEncoder();
            Decoder decoder = new BinaryDecoder();
            for (int i = 0; i < ITERATIONS; i++)
            {
                string key = string.Format("Key{0:########0}", i);
                string value = RandomDataHelper.GetString(50, 5000);
                expected.Add(key, value);
            }

            Schema valueSchema = new PrimitiveSchema("string");
            MapSchema mapSchema = new MapSchema(valueSchema);

            System.Diagnostics.Stopwatch watch = System.Diagnostics.Stopwatch.StartNew();

            using (MemoryStream iostr = new MemoryStream())
            {
                //IDictionary<string, string> actual = Serializer.Deserialize<IDictionary<string, string>>(PrefixStyle.None, mapSchema, iostr, decoder);
                Serializer.Serialize(PrefixStyle.None, mapSchema, iostr, encoder, expected);
                iostr.Position = 0;
                Assert.Greater(iostr.Length, 0, "Serialized length should be greater than 0.");
                IDictionary<string, string> actual = Serializer.Deserialize < IDictionary<string, string>>(PrefixStyle.None, mapSchema, iostr, decoder);
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

            watch.Stop();





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
