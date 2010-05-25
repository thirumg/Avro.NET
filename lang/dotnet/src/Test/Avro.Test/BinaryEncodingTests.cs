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
                
                BinaryEncoder.Instance.WriteInt(iostr, expectedValue);
                iostr.Flush();
                iostr.Position = 0;

                int actual = BinaryDecoder.Instance.ReadInt(iostr);
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
                
                BinaryEncoder.Instance.WriteLong(iostr, expectedValue);
                iostr.Flush();
                iostr.Position = 0;

                long actual = BinaryDecoder.Instance.ReadLong(iostr);
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

                BinaryEncoder.Instance.WriteString(iostr, expectedValue);


                iostr.Position = 0;
                

                string actual = BinaryDecoder.Instance.ReadString(iostr);
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
                
                BinaryEncoder.Instance.WriteBoolean(iostr, expectedValue);
                
                iostr.Position = 0;
                
                bool actual = BinaryDecoder.Instance.ReadBool(iostr);
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
                
                BinaryEncoder.Instance.WriteDouble(iostr, expectedValue);
                iostr.Position = 0;
                
                double actual = BinaryDecoder.Instance.ReadDouble(iostr);
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
                
                BinaryEncoder.Instance.WriteFloat(iostr, expectedValue);
                iostr.Position = 0;
                
                float actual = BinaryDecoder.Instance.ReadFloat(iostr);
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
                
                BinaryEncoder.Instance.WriteBytes(iostr, expectedValue);
                iostr.Position = 0;
                
                byte[] actual = BinaryDecoder.Instance.ReadBytes(iostr);
                Assert.IsTrue(
                    ArrayHelper<byte>.Equals(expectedValue, actual)
                    , "Iteration {0:###,###,###,##0}", i);
            }
        }
    }
}
