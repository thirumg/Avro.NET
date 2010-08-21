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

namespace Avro.Test
{
    [TestFixture]
    class TestSchema
    {
        // Primitive types - shorthand
        [TestCase("null")]
        [TestCase("boolean")]
        [TestCase("int")]
        [TestCase("long")]
        [TestCase("float")]
        [TestCase("double")]
        [TestCase("bytes")]
        [TestCase("string")]

        // Primitive types - longer
        [TestCase("{ \"type\": \"null\" }")]
        [TestCase("{ \"type\": \"boolean\" }")]
        [TestCase("{ \"type\": \"int\" }")]
        [TestCase("{ \"type\": \"long\" }")]
        [TestCase("{ \"type\": \"float\" }")]
        [TestCase("{ \"type\": \"double\" }")]
        [TestCase("{ \"type\": \"bytes\" }")]
        [TestCase("{ \"type\": \"string\" }")]

        // Record
        [TestCase("{\"type\": \"record\",\"name\": \"Test\",\"fields\": [{\"name\": \"f\",\"type\": \"long\"}]}")]
        [TestCase("{\"type\": \"record\",\"name\": \"Test\",\"fields\": " +
            "[{\"name\": \"f1\",\"type\": \"long\"},{\"name\": \"f2\", \"type\": \"int\"}]}")]
        [TestCase("{\"type\": \"error\",\"name\": \"Test\",\"fields\": " +
            "[{\"name\": \"f1\",\"type\": \"long\"},{\"name\": \"f2\", \"type\": \"int\"}]}")]
        [TestCase("{\"type\":\"record\",\"name\":\"LongList\"," +
            "\"fields\":[{\"name\":\"value\",\"type\":\"long\"},{\"name\":\"next\",\"type\":[\"LongList\",\"null\"]}]}")] // Recursive.
        [TestCase("{\"type\":\"record\",\"name\":\"LongList\"," +
            "\"fields\":[{\"name\":\"value\",\"type\":\"long\"},{\"name\":\"next\",\"type\":[\"LongListA\",\"null\"]}]}",
            Description = "Unknown name", ExpectedException = typeof(SchemaParseException))]
        [TestCase("{\"type\":\"record\",\"name\":\"LongList\"}",
            Description = "No fields", ExpectedException = typeof(SchemaParseException))]
        [TestCase("{\"type\":\"record\",\"name\":\"LongList\", \"fields\": \"hi\"}",
            Description = "Fields not an array", ExpectedException = typeof(SchemaParseException))]

        // Enum
        [TestCase("{\"type\": \"enum\", \"name\": \"Test\", \"symbols\": [\"A\", \"B\"]}")]
        [TestCase("{\"type\": \"enum\", \"name\": \"Status\", \"symbols\": \"Normal Caution Critical\"}",
            Description = "Symbols not an array", ExpectedException = typeof(SchemaParseException))]
        [TestCase("{\"type\": \"enum\", \"name\": [ 0, 1, 1, 2, 3, 5, 8 ], \"symbols\": [\"Golden\", \"Mean\"]}",
            Description = "Name not a string", ExpectedException = typeof(SchemaParseException))]
        [TestCase("{\"type\": \"enum\", \"symbols\" : [\"I\", \"will\", \"fail\", \"no\", \"name\"]}",
            Description = "No name", ExpectedException = typeof(SchemaParseException))]
        [TestCase("{\"type\": \"enum\", \"name\": \"Test\", \"symbols\" : [\"AA\", \"AA\"]}",
            Description = "Duplicate symbol", ExpectedException = typeof(SchemaParseException))]

        // Array
        [TestCase("{\"type\": \"array\", \"items\": \"long\"}")]
        [TestCase("{\"type\": \"array\",\"items\": {\"type\": \"enum\", \"name\": \"Test\", \"symbols\": [\"A\", \"B\"]}}")]

        // Map
        [TestCase("{\"type\": \"map\", \"values\": \"long\"}")]
        [TestCase("{\"type\": \"map\",\"values\": {\"type\": \"enum\", \"name\": \"Test\", \"symbols\": [\"A\", \"B\"]}}")]

        // Union
        [TestCase("[\"string\", \"null\", \"long\"]")]
        [TestCase("[\"string\", \"long\", \"long\"]",
            Description = "Duplicate type", ExpectedException = typeof(SchemaParseException))]
        [TestCase("[{\"type\": \"array\", \"items\": \"long\"}, {\"type\": \"array\", \"items\": \"string\"}]",
            Description = "Duplicate type", ExpectedException = typeof(SchemaParseException))]

        // Fixed
        [TestCase("{ \"type\": \"fixed\", \"name\": \"Test\", \"size\": 1}")]
        [TestCase("{\"type\": \"fixed\", \"name\": \"MyFixed\", \"namespace\": \"org.apache.hadoop.avro\", \"size\": 1}")]
        [TestCase("{ \"type\": \"fixed\", \"name\": \"Test\", \"size\": 1}")]
        [TestCase("{ \"type\": \"fixed\", \"name\": \"Test\", \"size\": 1}")]
        [TestCase("{\"type\": \"fixed\", \"name\": \"Missing size\"}", ExpectedException = typeof(SchemaParseException))]
        [TestCase("{\"type\": \"fixed\", \"size\": 314}",
            Description = "No name", ExpectedException = typeof(SchemaParseException))]
        public void TestBasic(string s)
        {
            Schema.Parse(s);
        }

        [TestCase("null", Schema.Type.NULL)]
        [TestCase("boolean", Schema.Type.BOOLEAN)]
        [TestCase("int", Schema.Type.INT)]
        [TestCase("long", Schema.Type.LONG)]
        [TestCase("float", Schema.Type.FLOAT)]
        [TestCase("double", Schema.Type.DOUBLE)]
        [TestCase("bytes", Schema.Type.BYTES)]
        [TestCase("string", Schema.Type.STRING)]
        
        [TestCase("{ \"type\": \"null\" }", Schema.Type.NULL)]
        [TestCase("{ \"type\": \"boolean\" }", Schema.Type.BOOLEAN)]
        [TestCase("{ \"type\": \"int\" }", Schema.Type.INT)]
        [TestCase("{ \"type\": \"long\" }", Schema.Type.LONG)]
        [TestCase("{ \"type\": \"float\" }", Schema.Type.FLOAT)]
        [TestCase("{ \"type\": \"double\" }", Schema.Type.DOUBLE)]
        [TestCase("{ \"type\": \"bytes\" }", Schema.Type.BYTES)]
        [TestCase("{ \"type\": \"string\" }", Schema.Type.STRING)]
        public void TestPrimitive(string s, Schema.Type type)
        {
            Schema sc = Schema.Parse(s);
            Assert.IsTrue(sc is PrimitiveSchema);
            Assert.AreEqual(type, sc.type);
        }

        [TestCase("{\"type\":\"record\",\"name\":\"LongList\"," +
            "\"fields\":[{\"name\":\"value\",\"type\":\"long\", \"default\": \"100\"}," +
            "{\"name\":\"next\",\"type\":[\"LongList\",\"null\"]}]}",
            new string[] { "value", "long", "100", "next", "union", null })]
        public void testRecord(string s, string[] kv)
        {
            Schema sc = Schema.Parse(s);
            Assert.AreEqual(Schema.Type.RECORD, sc.type);
            RecordSchema rs = sc as RecordSchema;
            Assert.AreEqual(kv.Length / 3, rs.Fields.Count);
            for (int i = 0; i < kv.Length; i += 3)
            {
                Field f = rs.Fields[kv[i]];
                Assert.AreEqual(kv[i + 1], f.schema.GetName());
                if (kv[i + 2] != null)
                {
                    Assert.IsTrue(f.hasDefault);
                    Assert.AreEqual(kv[i + 2], f.defaultValue);
                }
                else
                {
                    Assert.IsFalse(f.hasDefault);
                }
            }
        }

        [TestCase("{\"type\": \"enum\", \"name\": \"Test\", \"symbols\": [\"A\", \"B\"]}",
            new string[] { "A", "B" })]
        public void testEnum(string s, string[] symbols)
        {
            Schema sc = Schema.Parse(s);
            Assert.AreEqual(Schema.Type.ENUM, sc.type);
            EnumSchema es = sc as EnumSchema;
            Assert.AreEqual(symbols.Length, es.symbols.Count);

            int i = 0;
            foreach (String str in es.symbols)
            {
                Assert.AreEqual(symbols[i++], str);
            }
        }

        [TestCase("{\"type\": \"array\", \"items\": \"long\"}", "long")]
        public void testArray(string s, string item)
        {
            Schema sc = Schema.Parse(s);
            Assert.AreEqual(Schema.Type.ARRAY, sc.type);
            ArraySchema ars = sc as ArraySchema;
            Assert.AreEqual(item, ars.itemSchema.GetName());
        }

        [TestCase("{\"type\": \"map\", \"values\": \"long\"}", "long")]
        public void testMap(string s, string value)
        {
            Schema sc = Schema.Parse(s);
            Assert.AreEqual(Schema.Type.MAP, sc.type);
            MapSchema ms = sc as MapSchema;
            Assert.AreEqual(value, ms.valueSchema.GetName());
        }

        [TestCase("[\"string\", \"null\", \"long\"]", new string[] { "string", "null", "long" })]
        public void testUnion(string s, string[] types)
        {
            Schema sc = Schema.Parse(s);
            Assert.AreEqual(Schema.Type.UNION, sc.type);
            UnionSchema us = sc as UnionSchema;
            Assert.AreEqual(types.Length, us.schemas.Count);

            int i = 0;
            foreach (Schema sch in us.schemas)
            {
                Assert.AreEqual(types[i++], sch.GetName());
            }
        }

        [TestCase("{ \"type\": \"fixed\", \"name\": \"Test\", \"size\": 1}", 1)]
        public void testFixed(string s, int size)
        {
            Schema sc = Schema.Parse(s);
            Assert.AreEqual(Schema.Type.FIXED, sc.type);
            FixedSchema fs = sc as FixedSchema;
            Assert.AreEqual(size, fs.size);
        }

        [TestCase("a", "o.a.h", Result = "o.a.h.a")]
        public string testFullname(string s1, string s2)
        {
            return Name.make_fullname(s1, s2).full;
        }
    }
}
