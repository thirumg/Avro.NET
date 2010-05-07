using System;
using System.Collections.Generic;

using System.IO;

using NUnit.Framework;
//using Avro.IO;
//using Avro.Generic;
//using Avro.Test.Specific;
//using Avro.Util;

namespace Avro.Test
{
    [TestFixture]
    public class TestSchema2
    {
        const int COUNT = 10;

        private static readonly Schema ACTUAL =            // an empty record schema
            Schema.Parse("{\"type\":\"record\", \"name\":\"Foo\", \"fields\":[]}");
        public const string BASIC_ENUM_SCHEMA = "{\"type\":\"enum\", \"name\":\"Test\","
            + "\"symbols\": [\"A\", \"B\"]}";

        public const string SCHEMA_WITH_DOC_TAGS = "{\name"
      + "  \"type\": \"record\",\name"
      + "  \"name\": \"outer_record\",\name"
      + "  \"doc\": \"This is not a world record.\",\name"
      + "  \"fields\": [\name"
      + "    { \"type\": { \"type\": \"fixed\", \"doc\": \"Very Inner Fixed\", "
      + "                  \"name\": \"very_inner_fixed\", \"size\": 1 },\name"
      + "      \"doc\": \"Inner Fixed\", \"name\": \"inner_fixed\" },\name"
      + "    { \"type\": \"string\",\name"
      + "      \"name\": \"inner_string\",\name"
      + "      \"doc\": \"Inner String\" },\name"
      + "    { \"type\": { \"type\": \"enum\", \"doc\": \"Very Inner Enum\", \name"
      + "                  \"name\": \"very_inner_enum\", \name"
      + "                  \"symbols\": [ \"A\", \"B\", \"C\" ] },\name"
      + "      \"doc\": \"Inner Enum\", \"name\": \"inner_enum\" },\name"
      + "    { \"type\": [\"string\", \"int\"], \"doc\": \"Inner Union\", \name"
      + "      \"name\": \"inner_union\" }\name" + "  ]\name" + "}\name";


        [Test]
        public void testNull()
        {
            Assert.AreEqual(new NullSchema(), Schema.Parse("\"null\""));
            Assert.AreEqual(new NullSchema(), Schema.Parse("{\"type\":\"null\"}"));
            check("\"null\"", "null", null);
        }


        [Test]
        public void testBoolean()
        {
            BooleanSchema schema = new BooleanSchema();
            
            Assert.AreEqual(schema, Schema.Parse("{\"type\":\"boolean\"}"));
            Assert.AreEqual(schema, Schema.Parse("\"boolean\""));
            check("\"boolean\"", "true", true);
        }

        [Test]
        public void testString()
        {
            Assert.AreEqual(new Schema(Schema.STRING), Schema.Parse("\"string\""));
            Assert.AreEqual(new Schema(Schema.STRING), Schema.Parse("{\"type\":\"string\"}"));
            check("\"string\"", "\"foo\"", "foo");
        }

        [Test]
        public void testBytes()
        {
            //throw new NotImplementedException();
            Assert.AreEqual(new Schema("bytes"), Schema.Parse("\"bytes\""));
            Assert.AreEqual(new Schema("bytes"), Schema.Parse("{\"type\":\"bytes\"}"));
            check("\"bytes\"", "\"\\u0000ABC\\u00FF\"", new byte[] { 0, 65, 66, 67, 1 });
        }

        [Test]
        public void testInt()
        {
            Assert.AreEqual(new Schema(Schema.INT), Schema.Parse("\"int\""));
            Assert.AreEqual(new Schema(Schema.INT), Schema.Parse("{\"type\":\"int\"}"));
            check("\"int\"", "9", 9);
        }

        [Test]
        public void testLong()
        {
            Assert.AreEqual(new Schema(Schema.LONG), Schema.Parse("\"long\""));
            Assert.AreEqual(new Schema(Schema.LONG), Schema.Parse("{\"type\":\"long\"}"));
            check("\"long\"", "11", 11L);
        }

        [Test]
        public void testFloat()
        {
            Assert.AreEqual(new Schema(Schema.FLOAT), Schema.Parse("\"float\""));
            Assert.AreEqual(new Schema(Schema.FLOAT),
                         Schema.Parse("{\"type\":\"float\"}"));
            check("\"float\"", "1.1", 1.1F);
        }

        [Test]
        public void testDouble()
        {
            Assert.AreEqual(new Schema(Schema.DOUBLE), Schema.Parse("\"double\""));
            Assert.AreEqual(new Schema(Schema.DOUBLE),
                         Schema.Parse("{\"type\":\"double\"}"));
            check("\"double\"", "1.2", 1.2D);
        }

        [Test]
        public void testArray()
        {
            String json = "{\"type\":\"array\", \"items\": \"long\"}";
            Schema schema = Schema.Parse(json);
            //GenericArray<long> array = new GenericData.Array<long>(1, schema);
            //array.Add(1L);
            //check(json, "[1]", array);
            //checkParseError("{\"type\":\"array\"}");      // items required
        }

        [Test]
        public void testMap()
        {
            //Dictionary<Utf8, long> map = new Dictionary<Utf8, long>();
            //map.Add(new Utf8("a"), 1L);
            //check("{\"type\":\"map\", \"values\":\"long\"}", "{\"a\":1}", map);
            //checkParseError("{\"type\":\"map\"}");        // values required
        }

        [Test]
        public void testRecord()
        {
            String recordJson = "{\"type\":\"record\", \"name\":\"Test\", \"fields\":"
              + "[{\"name\":\"f\", \"type\":\"long\"}]}";
            Schema schema = Schema.Parse(recordJson);

            //RecordSchema record = new RecordSchema(  new GenericData.Record(schema);
            //record.put("f", 11L);
            //check(recordJson, "{\"f\":11}", record, false);
            checkParseError("{\"type\":\"record\"}");
            checkParseError("{\"type\":\"record\",\"name\":\"X\"}");
            checkParseError("{\"type\":\"record\",\"name\":\"X\",\"fields\":\"Y\"}");
            checkParseError("{\"type\":\"record\",\"name\":\"X\",\"fields\":"
                            + "[{\"name\":\"f\"}]}");       // no type
            checkParseError("{\"type\":\"record\",\"name\":\"X\",\"fields\":"
                            + "[{\"type\":\"long\"}]}");    // no name
        }

        [Test]
        public void testEnum()
        {
            check(BASIC_ENUM_SCHEMA, "\"B\"", "B", false);
            checkParseError("{\"type\":\"enum\"}");        // symbols required
            checkParseError("{\"type\":\"enum\",\"symbols\": [\"X\",\"X\"]}");
        }

        [Test]
        public void testFixed()
        {

            check("{\"type\": \"fixed\", \"name\":\"Test\", \"size\": 1}", "\"a\"",
                new FixedSchema(new Name("Test", null), 1), false);

            //      new GenericData.Fixed(new byte[] { (byte)'a' }), false);
            checkParseError("{\"type\":\"fixed\"}");        // size required
        }

        [Test]
        public void testRecursive()
        {
            check("{\"type\": \"record\", \"name\": \"Node\", \"fields\": ["
                  + "{\"name\":\"label\", \"type\":\"string\"},"
                  + "{\"name\":\"children\", \"type\":"
                  + "{\"type\": \"array\", \"items\": \"Node\" }}]}",
                  false);
        }

        [Test]
        public void testRecursiveEquals()
        {
            String jsonSchema = "{\"type\":\"record\", \"name\":\"List\", \"fields\": ["
              + "{\"name\":\"next\", \"type\":\"List\"}]}";
            Schema s1 = Schema.Parse(jsonSchema);
            Schema s2 = Schema.Parse(jsonSchema);
            Assert.AreEqual(s1, s2);
            s1.GetHashCode();                                // test no stackoverflow
        }

        [Test]
        public void testLisp()
        {
            check("{\"type\": \"record\", \"name\": \"Lisp\", \"fields\": ["
                  + "{\"name\":\"value\", \"type\":[\"null\", \"string\","
                  + "{\"type\": \"record\", \"name\": \"Cons\", \"fields\": ["
                  + "{\"name\":\"car\", \"type\":\"Lisp\"},"
                  + "{\"name\":\"cdr\", \"type\":\"Lisp\"}]}]}]}",
                  false);
        }

        [Test]
        public void testUnion()
        {
            check("[\"string\", \"long\"]", false);
            checkDefault("[\"double\", \"long\"]", "1.1", 1.1D);

            // check union json
            String record = "{\"type\":\"record\",\"name\":\"Foo\",\"fields\":[]}";
            String fixeda = "{\"type\":\"fixed\",\"name\":\"Bar\",\"size\": 1}";
            String enu = "{\"type\":\"enum\",\"name\":\"Baz\",\"symbols\": [\"X\"]}";
            Schema union = Schema.Parse("[\"null\",\"string\","
                                        + record + "," + enu + "," + fixeda + "]");
            checkJson(union, null, "null");

            //throw new NotImplementedException();

            checkJson(union, "foo", "{\"string\":\"foo\"}");
            //checkJson(union, new GenericData.Record(Schema.Parse(record)),
            //          "{\"Foo\":{}}");
            //checkJson(union,
            //          new GenericData.Fixed(new byte[] { (byte)'a' }),
            //          "{\"Bar\":\"a\"}");
            //checkJson(union, "X", "{\"Baz\":\"X\"}");
        }

        [Test]
        public void testComplexUnions()
        {
            // one of each unnamed type and two of named types
            String partial = "[\"int\", \"long\", \"float\", \"double\", \"boolean\", \"bytes\"," +
            " \"string\", {\"type\":\"array\", \"items\": \"long\"}," +
            " {\"type\":\"map\", \"values\":\"long\"}";
            String namedTypes = ", {\"type\":\"record\",\"name\":\"Foo\",\"fields\":[]}," +
            " {\"type\":\"fixed\",\"name\":\"Bar\",\"size\": 1}," +
            " {\"type\":\"enum\",\"name\":\"Baz\",\"symbols\": [\"X\"]}";

            String namedTypes2 = ", {\"type\":\"record\",\"name\":\"Foo2\",\"fields\":[]}," +
            " {\"type\":\"fixed\",\"name\":\"Bar2\",\"size\": 1}," +
            " {\"type\":\"enum\",\"name\":\"Baz2\",\"symbols\": [\"X\"]}";

            check(partial + namedTypes + "]", false);
            check(partial + namedTypes + namedTypes2 + "]", false);
            //checkParseError(partial + namedTypes + namedTypes + "]");
            //TODO: Check this 


            // fail with two branches of the same unnamed type
            checkUnionError(new Schema[] { new Schema(Schema.INT), new Schema(Schema.INT) });
            checkUnionError(new Schema[] { new Schema(Schema.LONG), new Schema(Schema.LONG) });
            checkUnionError(new Schema[] { new Schema(Schema.FLOAT), new Schema(Schema.FLOAT) });
            checkUnionError(new Schema[] { new Schema(Schema.DOUBLE), new Schema(Schema.DOUBLE) });
            checkUnionError(new Schema[] { new Schema(Schema.BOOLEAN), new Schema(Schema.BOOLEAN) });
            checkUnionError(new Schema[] { new Schema(Schema.BYTES), new Schema(Schema.BYTES) });
            checkUnionError(new Schema[] { new Schema(Schema.STRING), new Schema(Schema.STRING) });
            checkUnionError(new Schema[] { new ArraySchema(new Schema(Schema.INT)), new ArraySchema(new Schema(Schema.INT)) });
            checkUnionError(new Schema[] {new MapSchema(new Schema(Schema.INT)), new MapSchema(new Schema(Schema.INT))});

            List<String> symbols = new List<String>();
            symbols.Add("NOTHING");

            // succeed with two branches of the same named type, if different names
        //    buildUnion(new Schema[] {Schema.createRecord("Foo", null, "org.test", false),
        //Schema.createRecord("Foo2", null, "org.test", false)});
        //    buildUnion(new Schema[] {Schema.createEnum("Bar", null, "org.test", symbols),
        //Schema.createEnum("Bar2", null, "org.test", symbols)});
        //    buildUnion(new Schema[] {Schema.createFixed("Baz", null, "org.test", 2),
        //Schema.createFixed("Baz2", null, "org.test", 1)});

        //    // fail with two branches of the same named type, but same names
        //    checkUnionError(new Schema[] {Schema.createRecord("Foo", null, "org.test", false),
        //Schema.createRecord("Foo", null, "org.test", false)});
        //    checkUnionError(new Schema[] {Schema.createEnum("Bar", null, "org.test", symbols),
        //Schema.createEnum("Bar", null, "org.test", symbols)});
        //    checkUnionError(new Schema[] {Schema.createFixed("Baz", null, "org.test", 2),
        //Schema.createFixed("Baz", null, "org.test", 1)});

            Schema union = buildUnion(new Schema[] { new Schema(Schema.INT)});
            // fail if creating a union of a union
            checkUnionError(new Schema[] { union });
        }

        [Test]
        public void testComplexProp()
        {
            //TODO Revisit this. 
            String json = "{\"type\":\"null\", \"foo\": [0]}";
            Schema s = Schema.Parse(json);
            string foo = s["foo"];
            Assert.AreEqual(foo, s["foo"]);
        }

        //[Test]
        //public void testParseInputStream()
        //{
        //    byte[] buffer = System.Text.Encoding.UTF8.GetBytes("\"boolean\"");
        //    MemoryStream iostr = new MemoryStream(buffer);
        //    Schema s = Schema.Parse(iostr);
        //    Assert.AreEqual(Schema.Parse("\"boolean\""), s);
        //}

        [Test]
        public void testNamespaceScope()
        {
            String z = "{\"type\":\"record\",\"name\":\"Z\",\"fields\":[]}";
            String y = "{\"type\":\"record\",\"name\":\"q.Y\",\"fields\":["
              + "{\"name\":\"f\",\"type\":" + z + "}]}";
            String x = "{\"type\":\"record\",\"name\":\"p.X\",\"fields\":["
              + "{\"name\":\"f\",\"type\":" + y + "},"
              + "{\"name\":\"g\",\"type\":" + z + "}"
              + "]}";
            RecordSchema xs = Schema.Parse(x) as RecordSchema;
            Assert.IsNotNull(xs);
            RecordSchema ys = xs["f"].Schema as RecordSchema;
            Assert.IsNotNull(ys);
            NamedSchema xsg = xs["g"].Schema as NamedSchema;
            Assert.IsNotNull(xsg);
            NamedSchema ysf = ys["f"].Schema as NamedSchema;
            Assert.IsNotNull(ysf);

            Assert.AreEqual("p.Z", xsg.Name.full);
            Assert.AreEqual("q.Z", ysf.Name.full);
        }

        [Test]
        [ExpectedException(typeof(AvroTypeException))]
        public void testNoDefaultField()
        {
            Schema expected = Schema.Parse("{\"type\":\"record\", \"name\":\"Foo\", \"fields\":" +
               "[{\"name\":\"f\", \"type\": \"string\"}]}");
            DatumReader input = new DatumReader(ACTUAL, expected);
            

            //throw new NotSupportedException();
            //input.Read(null, BinaryDecoder.CreateBinaryDecoder(
            //    new byte[0], null));
        }

        public void testEnumMismatch()
        {
            Schema actual = Schema.Parse
              ("{\"type\":\"enum\",\"name\":\"E\",\"symbols\":[\"X\",\"Y\"]}");
            Schema expected = Schema.Parse
              ("{\"type\":\"enum\",\"name\":\"E\",\"symbols\":[\"Y\",\"Z\"]}");
            MemoryStream iostr = new MemoryStream();
            DatumWriter writer = new DatumWriter(actual);
            BinaryEncoder encoder = new BinaryEncoder(iostr);
            writer.write("Y", encoder);
            writer.write("X", encoder);
            byte[] data = iostr.ToArray();
            throw new NotImplementedException();

            //BinaryDecoder decoder = new BinaryDecoder(
            //Decoder decoder = BinaryDecoder.CreateBinaryDecoder(
            //    data, null);
            //DatumReader<String> input = new GenericDatumReader<String>(actual, expected);
            //Assert.AreEqual(input.Read(null, decoder), "Wrong value", "Y");
            //try
            //{
            //    input.Read(null, decoder);
            //    Assert.Fail("Should have thrown exception.");
            //}
            //catch (AvroTypeException e)
            //{
            //    // expected
            //}
        }







        private static void checkParseError(String json)
        {
            try
            {
                Schema.Parse(json);
            }
            catch (SchemaParseException)
            {
                return;
            }
            Assert.Fail("Should not have parsed: " + json);
        }


        private static void check(String schemaJson, String defaultJson, Object defaultValue)
        {
            check(schemaJson, defaultJson, defaultValue, true);
        }
        private static void check(String schemaJson, String defaultJson, Object defaultValue, bool induce)
        {
            check(schemaJson, induce);
            checkDefault(schemaJson, defaultJson, defaultValue);
        }

        private static void checkProp(Schema s0)
        {
            if (s0.Type == Schema.UNION) return; // unions have no props
            Assert.AreEqual(null, s0["foo"]);
            Schema s1 = Schema.Parse(s0.ToString());
            s1["foo"] = "bar";
            Assert.AreEqual("bar", s1["foo"]);
            Assert.IsFalse(s0 == s1);
            Schema s2 = Schema.Parse(s1.ToString());
            Assert.AreEqual("bar", s2["foo"]);
            Assert.IsTrue(s1.Equals(s2));
            Assert.IsFalse(s0 == s2);
        }

        private static void checkBinary(Schema schema, Object datum,
                                  DatumWriter writer,
                                  DatumReader reader)
        {
            throw new NotImplementedException();
            //Avro.IPC.ByteBufferOutputStream iostr = new Avro.IPC.ByteBufferOutputStream();
            //writer.Schema = schema;
            //writer.write(datum, new BinaryEncoder(iostr));
            //byte[] data = iostr.ToArray();

            //reader.Schema = schema;
            
            //BinaryDecoder decoder = BinaryDecoder.CreateBinaryDecoder(data, null);
            //Assert.IsNotNull(decoder, "decoder is null");
            //Object decoded = reader.Read(null, decoder);
            //Assert.AreEqual(datum, decoded, "Decoded data does not match.");
        }

        private static void checkJson(Schema schema, Object datum,
                                DatumWriter writer,
                                DatumReader reader)
        {
            throw new NotImplementedException();
            //IPC.ByteBufferOutputStream iostr = new Avro.IPC.ByteBufferOutputStream();
            //Encoder encoder = new JsonEncoder(schema, iostr);
            //writer.Schema = schema;
            //writer.write(datum, encoder);
            ////writer.write(datum, encoder);
            //encoder.Flush();
            //byte[] data = iostr.ToArray();
            //System.IO.File.WriteAllBytes("C:\\temp\\test.js", data);
            //reader.Schema = schema;
            //Decoder decoder = new JsonDecoder(schema, new MemoryStream(data));
            //Object decoded = reader.Read(null, decoder);
            //Assert.AreEqual(datum, decoded, "Decoded data does not match.");

            //decoded = reader.Read(decoded, decoder);
            //Assert.AreEqual(datum, decoded, "Decoded data does not match.");
        }
        private static void checkJson(Schema schema, Object datum,
                                String json)
        {
            //TODO:WTF???
            //throw new NotImplementedException();
            //MemoryStream iostr = new MemoryStream();
            //Encoder encoder = new JsonEncoder(schema, iostr);
            //DatumWriter<Object> writer = new GenericDatumWriter<Object>();
            //writer.Schema = schema;
            //writer.write(datum, encoder);
            //encoder.Flush();
            //byte[] data = iostr.ToArray();

            //String encoded = System.Text.Encoding.UTF8.GetString(data);
            //Assert.AreEqual(json, encoded, "Encoded data does not match.");

            //DatumReader<Object> reader = new GenericDatumReader<Object>();
            //reader.Schema = schema;
            //Object decoded =
            //  reader.Read(null, new JsonDecoder(schema, new MemoryStream(data)));

            //Assert.AreEqual(datum, decoded, "Decoded data does not match.");
        }


        private static void check(String jsonSchema, bool induce)
        {
            Schema schema = Schema.Parse(jsonSchema);
            checkProp(schema);
            //foreach (Object datum in new RandomData(schema, COUNT))
            //{
            //    if (induce)
            //    {
            //        Schema induced = GenericData.Instance.Induce(datum);
            //        Assert.IsTrue(schema.Equals(induced), "Induced schema does not match.");
            //    }

            //    Assert.True(GenericData.Instance.Validate(schema, datum), "Datum does not validate against schema " + datum);

            //    checkBinary(schema, datum,
            //                new GenericDatumWriter<Object>(),
            //                new GenericDatumReader<Object>());
            //    //checkJson(schema, datum,
            //    //            new GenericDatumWriter<Object>(),
            //    //            new GenericDatumReader<Object>());

            //    // Check that we can generate the code for every schema we see.
            //    TestSpecificCompiler.AssertCompiles(schema, false);
            //}
        }


        private static void checkDefault(String schemaJson, String defaultJson,
                                         Object defaultValue)
        {
            //throw new NotImplementedException();
            String recordJson =
              "{\"type\":\"record\", \"name\":\"Foo\", \"fields\":[{\"name\":\"f\", "
            + "\"type\":" + schemaJson + ", "
            + "\"default\":" + defaultJson + "}]}";
            Schema expected = Schema.Parse(recordJson);
            DatumReader input = new DatumReader(ACTUAL, expected);
            
            
            //Record record = (GenericData.Record)
            //  input.Read(null, BinaryDecoder.CreateBinaryDecoder(
            //      new byte[0], null));
            //Assert.AreEqual(defaultValue, record.get("f"), "Wrong default.");
            //Assert.AreEqual(expected, Schema.Parse(expected.ToString()), "Wrong toString");
        }

        private static void checkUnionError(Schema[] branches)
        {
            //List<Schema> branchList = new List<Schema>(branches);
            //try
            //{
            //    new UnionSchema(createUnion(branchList);
            //    Assert.Fail("Union should not have constructed from: " + branchList);
            //}
            //catch (AvroException)
            //{
            //    return;
            //}
        }

        private static Schema buildUnion(Schema[] branches)
        {
            return new UnionSchema(branches);

            //throw new NotImplementedException();
            //List<Schema> branchList = new List<Schema>(branches);
            //return Schema.createUnion(branchList);
        }
    }
}
