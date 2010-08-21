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
using System.IO;
using NUnit.Framework;

namespace Avro.Test
{
    public partial class SerializerTests
    {
        /*
        [TestCase]
        public void RecordTest_Simple()
        {
            RecordSchema schema = new RecordSchema(new Name("RecordTest", null));
            Field testStringField = new Field(new PrimitiveSchema("string"), "TestString", false);
            schema.AddField(testStringField);
            Field TestInt32Field = new Field(new PrimitiveSchema("int"), "TestInt32", false);
            schema.AddField(TestInt32Field);
            Field TestInt64Field = new Field(new PrimitiveSchema("long"), "TestInt64", false);
            schema.AddField(TestInt64Field);

            for (int i = 0; i < ITERATIONS; i++)
            {

                RecordTest expected = new RecordTest();
                expected.TestString = RandomDataHelper.GetString(50, 500);
                expected.TestInt32 = RandomDataHelper.GetRandomInt32();
                expected.TestInt64 = RandomDataHelper.GetRandomInt64();

                using (MemoryStream iostr = new MemoryStream())
                {
                    Serializer.Serialize(PrefixStyle.None, schema, iostr, BinaryEncoder.Instance, expected);
                    Assert.Greater(iostr.Length, 0);
                    iostr.Position = 0L;
                    RecordTest actual = Serializer.Deserialize<RecordTest>(PrefixStyle.None, schema, iostr, BinaryDecoder.Instance);
                    Assert.IsNotNull(actual);
                    Assert.AreEqual(expected.TestString, actual.TestString);
                    Assert.AreEqual(expected.TestInt32, actual.TestInt32);
                    Assert.AreEqual(expected.TestInt64, actual.TestInt64);
                }
            }


        }



        [Record]
        public class RecordTest
        {
            [Field]
            public string TestString { get; set; }
            [Field]
            public int TestInt32 { get; set; }
            [Field]
            public long TestInt64 { get; set; }
        }
         */

    }
}
