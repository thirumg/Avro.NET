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
using System.IO;
using NUnit.Framework;
using Avro.IO;
namespace Avro.Test
{
    [TestFixture]
    public partial class SerializerTests
    {
        [TestFixtureSetUp]
        public void Setup()
        {
            Logger.ConfigureForUnitTesting();
        }

        
        const int ITERATIONS = 1000;






        void TestData(Schema schema, Encoder encoder, Decoder decoder, object[] data)
        {
            for (int i = 0; i < data.Length; i++)
            {
                object expected = data[i];
                Type expectedType = data[i].GetType();

                using (MemoryStream iostr = new MemoryStream())
                {
                    Serializer.Serialize(PrefixStyle.None, schema, iostr, encoder, expected);
                    iostr.Position = 0;
                    object actual = Serializer.Deserialize(PrefixStyle.None, schema, iostr, decoder, expectedType);
                    Assert.AreEqual(expected, actual);


                }
            }


        }

    }
}
