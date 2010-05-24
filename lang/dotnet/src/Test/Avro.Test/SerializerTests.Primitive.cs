using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace Avro.Test
{
	public partial class SerializerTests
	{
        [TestCase]
        public void IntTests()
        {
            PrimitiveSchema schema = new PrimitiveSchema("int");


            object[] data = new object[ITERATIONS];
            for (int i = 0; i < ITERATIONS; i++)
            {
                data[i] = RandomDataHelper.GetRandomInt32();
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
                data[i] = RandomDataHelper.GetRandomInt64();
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
                data[i] = RandomDataHelper.GetRandomBool();
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
	}
}
