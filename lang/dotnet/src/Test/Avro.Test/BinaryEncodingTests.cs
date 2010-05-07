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
        [Test]
        public void TestInt32()
        {
            Random random = new Random();

            for (int i = 0; i < 10000; i++)
            {
                int expectedValue = random.Next();
                MemoryStream iostr = new MemoryStream();
                BinaryEncoder encoder = new BinaryEncoder(iostr);
                encoder.write_int(expectedValue);
                iostr.Flush();
                iostr.Position = 0;
                BinaryDecoder decoder = new BinaryDecoder(iostr);
                int actual = decoder.ReadInt();
                Assert.AreEqual(expectedValue, actual, "Iteration {0:###,###,###,##0}", i);
            }
        }

        [Test]
        public void TestInt64()
        {
            Random random = new Random();

            for (int i = 0; i < 10000; i++)
            {
                long expectedValue = random.Next();
                MemoryStream iostr = new MemoryStream();
                BinaryEncoder encoder = new BinaryEncoder(iostr);
                encoder.WriteLong(expectedValue);
                iostr.Flush();
                iostr.Position = 0;

                BinaryDecoder decoder = new BinaryDecoder(iostr);
                long actual = decoder.ReadLong();
                Assert.AreEqual(expectedValue, actual, "Iteration {0:###,###,###,##0}", i);
            }
        }

        [Test]
        public void TestString()
        {
            Random random = new Random();

            for (int i = 0; i < 10000; i++)
            {
                byte[] buffers = new byte[100];
                random.NextBytes(buffers);

                string expectedValue = Convert.ToBase64String(buffers);
                MemoryStream iostr = new MemoryStream();
                BinaryEncoder encoder = new BinaryEncoder(iostr);
                encoder.WriteString(expectedValue);


                iostr.Position = 0;
                BinaryDecoder decoder = new BinaryDecoder(iostr);
                
                string actual = decoder.ReadUTF8();
                Assert.AreEqual(expectedValue, actual, "Iteration {0:###,###,###,##0}", i);
            }
        }
        [Test]
        public void TestBoolean()
        {
            Random random = new Random();

            for (int i = 0; i < 10000; i++)
            {
                bool expectedValue = random.Next() % 2 ==0;
                MemoryStream iostr = new MemoryStream();
                BinaryEncoder encoder = new BinaryEncoder(iostr);
                encoder.write_boolean(expectedValue);
                
                iostr.Position = 0;
                BinaryDecoder decoder = new BinaryDecoder(iostr);
                bool actual = decoder.ReadBool();
                Assert.AreEqual(expectedValue, actual, "Iteration {0:###,###,###,##0}", i);
            }
        }
        [Test]
        public void TestDouble()
        {
            Random random = new Random();

            for (int i = 0; i < 10000; i++)
            {
                double expectedValue = random.NextDouble();
                MemoryStream iostr = new MemoryStream();
                BinaryEncoder encoder = new BinaryEncoder(iostr);
                encoder.write_double(expectedValue);
                iostr.Position = 0;
                BinaryDecoder decoder = new BinaryDecoder(iostr);
                double actual = decoder.ReadDouble();
                Assert.AreEqual(expectedValue, actual, "Iteration {0:###,###,###,##0}", i);

               
            }
        }
        [Test]
        public void TestFloat()
        {
            Random random = new Random();

            for (int i = 0; i < 10000; i++)
            {
                float expectedValue = (float)random.NextDouble();
                MemoryStream iostr = new MemoryStream();
                BinaryEncoder encoder = new BinaryEncoder(iostr);
                encoder.write_float(expectedValue);
                iostr.Position = 0;
                BinaryDecoder decoder = new BinaryDecoder(iostr);
                float actual = decoder.ReadFloat();
                Assert.AreEqual(expectedValue, actual, "Iteration {0:###,###,###,##0}", i);
            }
        }

        [Test]
        public void TestBytes()
        {
            Random random = new Random();

            for (int i = 0; i < 10000; i++)
            {
                byte[] expectedValue = new byte[100];
                random.NextBytes(expectedValue);
                MemoryStream iostr = new MemoryStream();
                BinaryEncoder encoder = new BinaryEncoder(iostr);
                encoder.WriteBytes(expectedValue);
                iostr.Position = 0;
                BinaryDecoder decoder = new BinaryDecoder(iostr);
                byte[] actual = decoder.ReadBytes();
                Assert.AreEqual(expectedValue, actual, "Iteration {0:###,###,###,##0}", i);
            }
        }
    }
}
