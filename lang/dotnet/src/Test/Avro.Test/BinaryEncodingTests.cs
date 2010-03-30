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
                iostr.Position = 0;
                BinaryDecoder decoder = new BinaryDecoder(iostr);
                int actual = decoder.ReadInt();
                Assert.AreEqual(expectedValue, actual, "Iteration {0:###,###,###,##0}", i);
            }
        }
    }
}
