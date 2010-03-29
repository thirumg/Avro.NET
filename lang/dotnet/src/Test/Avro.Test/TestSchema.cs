using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace Avro.Test
{
    [TestFixture]
    class TestSchema
    {
        [Test]
        public void TestPrimitive()
        {
            List<ExampleSchema> examples = new List<ExampleSchema>();
            examples.Add(new ExampleSchema("True", false));
            examples.Add(new ExampleSchema("True", false));
            examples.Add(new ExampleSchema("{\"no_type\": \"test\"", false));
            examples.Add(new ExampleSchema("{\"type\": \"panther\"}", false));
            foreach (SchemaType type in PrimitiveSchema.SupportedTypes)
            {
                examples.Add(new ExampleSchema()
                {
                    Valid = true,
                    SchemaString =
                        string.Format("{{\"type\": \"{0}\"}}", EnumHelper<SchemaType>.ToStringLower(type))
                });
            }


            testExamples(examples);
        }
        [Test]
        public void TestFixed()
        {
            ExampleSchema[] examples = new ExampleSchema[]{
                new ExampleSchema("{\"type\": \"fixed\", \"name\": \"Test\", \"size\": 1}", true),
                new ExampleSchema("{\"type\": \"fixed\", \"name\": \"MyFixed\", \"namespace\": \"org.apache.hadoop.avro\", \"size\": 1}", true),
                new ExampleSchema("{\"type\": \"fixed\", \"name\": \"Missing size\"}", false),
                new ExampleSchema("{\"type\": \"fixed\", \"size\": 314}", false)
            };

            testExamples(examples);
        }

        [Test]
        public void TestEnum()
        {
            ExampleSchema[] examples = new ExampleSchema[]{
                  new ExampleSchema("{\"type\": \"enum\", \"name\": \"Test\", \"symbols\": [\"A\", \"B\"]}", true),
              new ExampleSchema("{\"type\": \"enum\", \"name\": \"Status\", \"symbols\": \"Normal Caution Critical\"}", false),
              new ExampleSchema("{\"type\": \"enum\", \"name\": [ 0, 1, 1, 2, 3, 5, 8 ], \"symbols\": [\"Golden\", \"Mean\"]}", false),
              new ExampleSchema("{\"type\": \"enum\", \"symbols\" : [\"I\", \"will\", \"fail\", \"no\", \"name\"]}", false),
              new ExampleSchema("{\"type\": \"enum\", \"name\": \"Test\" \"symbols\" : [\"AA\", \"AA\"]}", false)
            };

            testExamples(examples);

        }

        [Test]
        public void TestArray()
        {
            ExampleSchema[] examples = new ExampleSchema[]{
                  new ExampleSchema("{\"type\": \"array\", \"items\": \"long\"}", true),
                  new ExampleSchema("{\"type\": \"array\",\"items\": {\"type\": \"enum\", \"name\": \"Test\", \"symbols\": [\"A\", \"B\"]}}", true),
            };

            testExamples(examples);
        }
        [Test]
        public void TestMap()
        {
            ExampleSchema[] examples = new ExampleSchema[]{
                new ExampleSchema("{\"type\": \"map\", \"values\": \"long\"}", true),
                new ExampleSchema("{\"type\": \"map\", \"values\": {\"type\": \"enum\", \"name\": \"Test\", \"symbols\": [\"A\", \"B\"]}}", true)
            };

            testExamples(examples);
        }
        [Test]
        public void TestUnion()
        {
            ExampleSchema[] examples = new ExampleSchema[]{
                new ExampleSchema("[\"string\", \"null\", \"long\"]", true),
                new ExampleSchema("[\"null\", \"null\"]", false),
                new ExampleSchema("[\"long\", \"long\"]", false),
                new ExampleSchema("[{\"type\": \"array\", \"items\": \"long\"} {\"type\": \"array\", \"items\": \"string\"}]", false),
            };

            testExamples(examples);
        }
        [Test]
        public void TestRecord()
        {
            ExampleSchema[] examples = new ExampleSchema[]{
new ExampleSchema("{\"type\": \"record\",\"name\": \"Test\",\"fields\": [{\"name\": \"f\",\"type\": \"long\"}]}", true),
  new ExampleSchema("{\"type\": \"error\",\"name\": \"Test\",\"fields\": [{\"name\": \"f\",\"type\": \"long\"}]}", true),
  //new ExampleSchema("{\"type\": \"record\",\"name\": \"Node\",\"fields\": [{\"name\": \"label\", \"type\": \"string\"},{\"name\": \"children\",\"type\": {\"type\": \"array\", \"items\": \"Node\"}}]}", true),
  //new ExampleSchema("{\"type\": \"record\",\"name\": \"Lisp\",\"fields\": [{\"name\": \"value\",\"type\": [\"null\", \"string\",{\"type\": \"record\",\"name\": \"Cons\",\"fields\": [{\"name\": \"car\", \"type\": \"Lisp\"},{\"name\": \"cdr\", \"type\": \"Lisp\"}]}]}]}", true),
  new ExampleSchema("{\"type\": \"record\",\"name\": \"HandshakeRequest\",\"namespace\": \"org.apache.avro.ipc\",\"fields\": [{\"name\": \"clientHash\",\"type\": {\"type\": \"fixed\", \"name\": \"MD5\", \"size\": 16}},{\"name\": \"clientProtocol\", \"type\": [\"null\", \"string\"]},{\"name\": \"serverHash\", \"type\": \"MD5\"},{\"name\": \"meta\", \"type\": [\"null\", {\"type\": \"map\", \"values\": \"bytes\"}]}]}", true),
  new ExampleSchema("{\"type\": \"record\",\"name\": \"HandshakeResponse\",\"namespace\": \"org.apache.avro.ipc\",\"fields\": [{\"name\": \"match\",\"type\": {\"type\": \"enum\",\"name\": \"HandshakeMatch\",\"symbols\": [\"BOTH\", \"CLIENT\", \"NONE\"]}},{\"name\": \"serverProtocol\", \"type\": [\"null\", \"string\"]},{\"name\": \"serverHash\",\"type\": [\"null\",{\"name\": \"MD5\", \"size\": 16, \"type\": \"fixed\"}]},{\"name\": \"meta\",\"type\": [\"null\", {\"type\": \"map\", \"values\": \"bytes\"}]}]}", true),
  new ExampleSchema("{\"type\": \"record\",\"name\": \"Interop\",\"namespace\": \"org.apache.avro\",\"fields\": [{\"name\": \"intField\", \"type\": \"int\"},{\"name\": \"longField\", \"type\": \"long\"},{\"name\": \"stringField\", \"type\": \"string\"},{\"name\": \"boolField\", \"type\": \"boolean\"},{\"name\": \"floatField\", \"type\": \"float\"},{\"name\": \"doubleField\", \"type\": \"double\"},{\"name\": \"bytesField\", \"type\": \"bytes\"},{\"name\": \"nullField\", \"type\": \"null\"},{\"name\": \"arrayField\",\"type\": {\"type\": \"array\", \"items\": \"double\"}},{\"name\": \"mapField\",\"type\": {\"type\": \"map\",\"values\": {\"name\": \"Foo\",\"type\": \"record\",\"fields\": [{\"name\": \"label\",\"type\": \"string\"}]}}},{\"name\": \"unionField\",\"type\": [\"boolean\",\"double\",{\"type\": \"array\", \"items\": \"bytes\"}]},{\"name\": \"enumField\",\"type\": {\"type\": \"enum\",\"name\": \"Kind\",\"symbols\": [\"A\", \"B\", \"C\"]}},{\"name\": \"fixedField\",\"type\": {\"type\": \"fixed\", \"name\": \"MD5\", \"size\": 16}},{\"name\": \"recordField\",\"type\": {\"type\": \"record\",\"name\": \"Node\",\"fields\": [{\"name\": \"label\", \"type\": \"string\"}, {\"name\": \"children\", \"type\": {\"type\": \"array\", \"items\": \"Node\"}}]}}]}", true),
  new ExampleSchema("{\"type\": \"record\",\"name\": \"ipAddr\",\"fields\": [{\"name\": \"addr\", \"type\": [{\"name\": \"IPv6\", \"type\": \"fixed\", \"size\": 16},{\"name\": \"IPv4\", \"type\": \"fixed\", \"size\": 4}]}]}", true),
  new ExampleSchema("{\"type\": \"record\",\"name\": \"Address\",\"fields\": [{\"type\": \"string\"},{\"type\": \"string\", \"name\": \"City\"}]}", false),
  new ExampleSchema("{\"type\": \"record\",\"name\": \"Event\",\"fields\": [{\"name\": \"Sponsor\"},{\"name\": \"City\", \"type\": \"string\"}]}", false),
  new ExampleSchema("{\"type\": \"record\",\"fields\": \"His vision, from the constantly passing bars,\"\"name\", \"Rainer\"}", false),
  new ExampleSchema("{\"name\": [\"Tom\", \"Jerry\"],\"type\": \"record\",\"fields\": [{\"name\": \"name\", \"type\": \"string\"}]}", false)
            };

            testExamples(examples);

        }


        private static void testExamples(IEnumerable<ExampleSchema> examples)
        {
            foreach (ExampleSchema example in examples)
            {
                testExample(example);
            }
        }

        private static void testExample(ExampleSchema example)
        {
            try
            {
                Schema schema = Schema.Parse(example.SchemaString);
                Assert.IsTrue(example.Valid);

            }
            catch (Exception ex)
            {
                Assert.IsFalse(example.Valid, ex.Message);
            }
        }
    }
}
