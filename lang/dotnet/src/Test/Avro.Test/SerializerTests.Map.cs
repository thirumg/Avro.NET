/**
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using System.IO;

namespace Avro.Test
{
    public partial class SerializerTests
    {
        /*
        const string MAPTESTING = "Map Serialization";
        [Category(MAPTESTING)]
        [TestCase]
        public void MapTests_Bool()
        {
            Dictionary<string, bool> expected = new Dictionary<string, bool>();

            for (int i = 0; i < ITERATIONS; i++)
            {
                string key = string.Format("Key{0:########0}", i);
                bool value = RandomDataHelper.GetRandomBool();
                expected.Add(key, value);
            }

            MapSchema mapSchema = new MapSchema(PrimitiveSchema.BOOLEAN);

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

            Schema valueSchema = new PrimitiveSchema("int");
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

            Schema valueSchema = new PrimitiveSchema("long");
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

            Schema valueSchema = new PrimitiveSchema("float");
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
         */
    }
}
