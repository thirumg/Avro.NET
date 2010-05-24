using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using NUnit.Framework;

namespace Avro.Test
{
    public partial class SerializerTests
    {
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

    }
}
