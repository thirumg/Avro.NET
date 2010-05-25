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
