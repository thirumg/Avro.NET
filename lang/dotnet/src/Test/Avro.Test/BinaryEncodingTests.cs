using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using System.IO;

namespace Avro.Test
{
    [TestFixture]
    public class BinaryEncodingTests
    {
        const int ITERATIONS = 10000;
        Random random = new Random();

        [Test]
        public void TestInt32()
        {
            

            for (int i = 0; i < ITERATIONS; i++)
            {
                int expectedValue = random.Next();
                MemoryStream iostr = new MemoryStream();
                BinaryEncoder encoder = new BinaryEncoder();
                encoder.WriteInt(iostr, expectedValue);
                iostr.Flush();
                iostr.Position = 0;
                BinaryDecoder decoder = new BinaryDecoder();
                int actual = decoder.ReadInt(iostr);
                Assert.AreEqual(expectedValue, actual, "Iteration {0:###,###,###,##0}", i);
            }
        }

        [Test]
        public void TestInt64()
        {
            

            for (int i = 0; i < ITERATIONS; i++)
            {
                long expectedValue = random.Next();
                MemoryStream iostr = new MemoryStream();
                BinaryEncoder encoder = new BinaryEncoder();
                encoder.WriteLong(iostr, expectedValue);
                iostr.Flush();
                iostr.Position = 0;

                BinaryDecoder decoder = new BinaryDecoder();
                long actual = decoder.ReadLong(iostr);
                Assert.AreEqual(expectedValue, actual, "Iteration {0:###,###,###,##0}", i);
            }
        }

        [Test]
        public void TestString()
        {
            

            for (int i = 0; i < ITERATIONS; i++)
            {
                byte[] buffers = new byte[100];
                random.NextBytes(buffers);

                string expectedValue = Convert.ToBase64String(buffers);
                MemoryStream iostr = new MemoryStream();
                BinaryEncoder encoder = new BinaryEncoder();
                encoder.WriteString(iostr, expectedValue);


                iostr.Position = 0;
                BinaryDecoder decoder = new BinaryDecoder();

                string actual = decoder.ReadString(iostr);
                Assert.AreEqual(expectedValue, actual, "Iteration {0:###,###,###,##0}", i);
            }
        }
        [Test]
        public void TestBoolean()
        {
            

            for (int i = 0; i < ITERATIONS; i++)
            {
                bool expectedValue = random.Next() % 2 ==0;
                MemoryStream iostr = new MemoryStream();
                BinaryEncoder encoder = new BinaryEncoder();
                encoder.WriteBoolean(iostr, expectedValue);
                
                iostr.Position = 0;
                BinaryDecoder decoder = new BinaryDecoder();
                bool actual = decoder.ReadBool(iostr);
                Assert.AreEqual(expectedValue, actual, "Iteration {0:###,###,###,##0}", i);
            }
        }
        [Test]
        public void TestDouble()
        {
            

            for (int i = 0; i < ITERATIONS; i++)
            {
                double expectedValue = random.NextDouble();
                MemoryStream iostr = new MemoryStream();
                BinaryEncoder encoder = new BinaryEncoder();
                encoder.WriteDouble(iostr, expectedValue);
                iostr.Position = 0;
                BinaryDecoder decoder = new BinaryDecoder();
                double actual = decoder.ReadDouble(iostr);
                Assert.AreEqual(expectedValue, actual, "Iteration {0:###,###,###,##0}", i);

               
            }
        }
        [Test]
        public void TestFloat()
        {
            

            for (int i = 0; i < ITERATIONS; i++)
            {
                float expectedValue = (float)random.NextDouble();
                MemoryStream iostr = new MemoryStream();
                BinaryEncoder encoder = new BinaryEncoder();
                encoder.WriteFloat(iostr, expectedValue);
                iostr.Position = 0;
                BinaryDecoder decoder = new BinaryDecoder();
                float actual = decoder.ReadFloat(iostr);
                Assert.AreEqual(expectedValue, actual, "Iteration {0:###,###,###,##0}", i);
            }
        }

        [Test]
        public void TestBytes()
        {
            
            for (int i = 0; i < ITERATIONS; i++)
            {
                int length = random.Next(5, 5000);
                byte[] expectedValue = new byte[length];
                random.NextBytes(expectedValue);
                MemoryStream iostr = new MemoryStream();
                BinaryEncoder encoder = new BinaryEncoder();
                encoder.WriteBytes(iostr, expectedValue);
                iostr.Position = 0;
                BinaryDecoder decoder = new BinaryDecoder();
                byte[] actual = decoder.ReadBytes(iostr);
                Assert.IsTrue(
                    ArrayHelper<byte>.Equals(expectedValue, actual)
                    , "Iteration {0:###,###,###,##0}", i);
            }
        }
    }
}
