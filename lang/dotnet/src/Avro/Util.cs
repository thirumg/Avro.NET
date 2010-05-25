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

namespace Avro
{
    class Util
    {
        private const long Int64Msb = ((long)1) << 63;
        private const int Int32Msb = ((int)1) << 31;

        public static bool checkIsValue(string type, params string[] types)
        {
            foreach (string t in types)
                if (t == type)
                    return true;

            return false;
        }

        public static uint Zig(int value)
        {
            return (uint)((value << 1) ^ (value >> 31));
        }
        public static ulong Zig(long value)
        {
            return (ulong)((value << 1) ^ (value >> 63));
        }

        public static int Zag(uint ziggedValue)
        {
            int value = (int)ziggedValue;
            return (-(value & 0x01)) ^ ((value >> 1) & ~Util.Int32Msb);
        }

        public static long Zag(ulong ziggedValue)
        {
            long value = (long)ziggedValue;
            return (-(value & 0x01L)) ^ ((value >> 1) & ~Util.Int64Msb);
        }
    }
}
