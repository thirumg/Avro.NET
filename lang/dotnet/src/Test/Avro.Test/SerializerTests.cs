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
            Encoder encoder = new BinaryEncoder();
            object[] data = new object[ITERATIONS];
            for (int i = 0; i < ITERATIONS; i++)
            {
                data[i] = random.Next();
            }

            TestData(schema, encoder, data);
        }
        [TestCase]
        public void LongTests()
        {
            PrimitiveSchema schema = new PrimitiveSchema("long");
            Encoder encoder = new BinaryEncoder();
            object[] data = new object[ITERATIONS];
            for (int i = 0; i < ITERATIONS; i++)
            {
                data[i] = (long)random.Next();
            }

            TestData(schema, encoder, data);
        }
        [TestCase]
        public void BooleanTests()
        {
            PrimitiveSchema schema = new PrimitiveSchema("boolean");
            Encoder encoder = new BinaryEncoder();
            object[] data = new object[ITERATIONS];
            for (int i = 0; i < ITERATIONS; i++)
            {
                data[i] = random.Next() % 2==1;
            }

            TestData(schema, encoder, data);
        }

        [TestCase]
        public void StringTests()
        {
            PrimitiveSchema schema = new PrimitiveSchema("string");
            Encoder encoder = new BinaryEncoder();
            object[] data = new object[ITERATIONS];
            for (int i = 0; i < ITERATIONS; i++)
            {
                System.Text.StringBuilder builder = new System.Text.StringBuilder();

                int length = random.Next(1, 5000);

                for (int j = 0; j < length; j++)
                {
                    builder.Append((char)random.Next(1, 255));
                }

                data[i] = builder.ToString();
            }

            TestData(schema, encoder, data);
        }


        void TestData(Schema schema, Encoder encoder, object[] data)
        {
            for (int i = 0; i < data.Length; i++)
            {
                object expected = data[i];

                using (MemoryStream iostr = new MemoryStream())
                {
                    Serializer.Serialize(PrefixStyle.None, schema, iostr, encoder, expected);
                }
            }


        }

    }
}
