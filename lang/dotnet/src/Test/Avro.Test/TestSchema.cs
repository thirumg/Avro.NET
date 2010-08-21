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

        // Record
        [TestCase("{\"type\": \"record\",\"name\": \"Test\",\"fields\": [{\"name\": \"f\",\"type\": \"long\"}]}")]
        [TestCase("{\"type\": \"record\",\"name\": \"Test\",\"fields\": " +
            "[{\"name\": \"f1\",\"type\": \"long\"},{\"name\": \"f2\", \"type\": \"int\"}]}")]
        [TestCase("{\"type\": \"error\",\"name\": \"Test\",\"fields\": " +
            "[{\"name\": \"f1\",\"type\": \"long\"},{\"name\": \"f2\", \"type\": \"int\"}]}")]
        // FIXME: Add negative test cases for records

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

        [TestCase("{ \"type\": \"null\" }")]
        [TestCase("{ \"type\": \"boolean\" }")]
        [TestCase("{ \"type\": \"int\" }")]
        [TestCase("{ \"type\": \"long\" }")]
        [TestCase("{ \"type\": \"float\" }")]
        [TestCase("{ \"type\": \"double\" }")]
        [TestCase("{ \"type\": \"bytes\" }")]
        [TestCase("{ \"type\": \"string\" }")]
        public void TestPrimitive(string s)
        {
            Schema sc = Schema.Parse(s);
            Assert.IsTrue(sc is PrimitiveSchema);
        }

        [TestCase("a", "o.a.h", Result = "o.a.h.a")]
        public string testFullname(string s1, string s2)
        {
            return Name.make_fullname(s1, s2).full;
        }
    }
}
