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

namespace Avro.Test
{
    class RandomDataHelper
    {
        static Random random = new Random();

        public static byte[] GetBytes(int min, int max)
        {
            int length = random.Next(min, max);
            return GetBytes(length);
        }
        public static byte[] GetBytes(int length)
        {
            byte[] buffer = new byte[length];
            random.NextBytes(buffer);
            return buffer;
        }

        public static string GetString(int min, int max)
        {
            System.Text.StringBuilder builder = new System.Text.StringBuilder();

            int length = random.Next(min, max);

            for (int j = 0; j < length; j++)
            {
                builder.Append((char)random.Next(1, 255));
            }

            return builder.ToString();
        }

        public static bool GetRandomBool()
        {
            return random.Next() % 2 == 1;
        }

        public static int GetRandomInt32()
        {
            bool IsNegative = GetRandomBool();
            int value = random.Next();
            int mult = IsNegative ? -1 : 1;

            return value * mult;
        }

        public static long GetRandomInt64()
        {
            return (long)GetRandomInt32();
        }

        public static float GetRandomFloat()
        {
            return (float)random.NextDouble();
        }
    }
}
