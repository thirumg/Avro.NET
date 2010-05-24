using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using Avro.IO;

namespace Avro.Test
{
    public partial class SerializerTests
    {
        [TestCase]
        public void Array_String()
        {
            Schema schema = new ArraySchema(PrimitiveSchema.String);
            string[] expected = new string[ITERATIONS];
            for (int i = 0; i < expected.Length; i++)
                expected[i] = RandomDataHelper.GetString(50, 1000);

            using (MemoryStream iostr = new MemoryStream())
            {
                Serializer.Serialize(PrefixStyle.None, schema, iostr, BinaryEncoder.Instance, expected);
                Assert.Greater(iostr.Length, 0);
                iostr.Position = 0L;
                string[] actual = Serializer.Deserialize<string[]>(PrefixStyle.None, schema, iostr, BinaryDecoder.Instance);
                Assert.IsNotNull(actual);
                Assert.AreEqual(expected.LongLength, actual.LongLength);

                for (int i = 0; i < expected.Length; i++)
                    Assert.AreEqual(expected[i], actual[i], "Index {0} does not match", i);
            }
        }
        [TestCase]
        //[Ignore]
        public void Array_Int32()
        {
            Schema schema = new ArraySchema(PrimitiveSchema.Int);
            int[] expected = new int[ITERATIONS];
            for (int i = 0; i < expected.Length; i++)
            {
                expected.SetValue(RandomDataHelper.GetRandomInt32(), i);
            }
                

            using (MemoryStream iostr = new MemoryStream())
            {
                Serializer.Serialize(PrefixStyle.None, schema, iostr, BinaryEncoder.Instance, expected);
                Assert.Greater(iostr.Length, 0);
                iostr.Position = 0L;
                int[] actual = Serializer.Deserialize<int[]>(PrefixStyle.None, schema, iostr, BinaryDecoder.Instance);
                Assert.IsNotNull(actual);
                Assert.AreEqual(expected.LongLength, actual.LongLength);

                for (int i = 0; i < expected.Length; i++)
                    Assert.AreEqual(expected[i], actual[i], "Index {0} does not match", i);
            }
        }
        [TestCase]
        [Ignore]
        public void Array_Int64()
        {
            Schema schema = new ArraySchema(PrimitiveSchema.Long);
            long[] expected = new long[ITERATIONS];
            for (int i = 0; i < expected.Length; i++)
                expected[i] = RandomDataHelper.GetRandomInt32();

            using (MemoryStream iostr = new MemoryStream())
            {
                Serializer.Serialize(PrefixStyle.None, schema, iostr, BinaryEncoder.Instance, expected);
                Assert.Greater(iostr.Length, 0);
                iostr.Position = 0L;
                long[] actual = Serializer.Deserialize<long[]>(PrefixStyle.None, schema, iostr, BinaryDecoder.Instance);
                Assert.IsNotNull(actual);
                Assert.AreEqual(expected.LongLength, actual.LongLength);

                for (int i = 0; i < expected.Length; i++)
                    Assert.AreEqual(expected[i], actual[i], "Index {0} does not match", i);
            }
        }
        [TestCase]
        [Ignore]
        public void Array_Float()
        {
            Schema schema = new ArraySchema(PrimitiveSchema.Float);
            float[] expected = new float[ITERATIONS];
            for (int i = 0; i < expected.Length; i++)
                expected[i] = RandomDataHelper.GetRandomInt32();

            using (MemoryStream iostr = new MemoryStream())
            {
                Serializer.Serialize(PrefixStyle.None, schema, iostr, BinaryEncoder.Instance, expected);
                Assert.Greater(iostr.Length, 0);
                iostr.Position = 0L;
                float[] actual = Serializer.Deserialize<float[]>(PrefixStyle.None, schema, iostr, BinaryDecoder.Instance);
                Assert.IsNotNull(actual);
                Assert.AreEqual(expected.LongLength, actual.LongLength);

                for (int i = 0; i < expected.Length; i++)
                    Assert.AreEqual(expected[i], actual[i], "Index {0} does not match", i);
            }
        }
    }
}
