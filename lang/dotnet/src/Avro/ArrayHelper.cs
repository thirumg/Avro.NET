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
    public static class ArrayHelper<T>
    {
        public static bool Equals(T[] a, T[] b)
        {
            if (null == a && null == b)
                return true;
            if ((null == a && null != b) || (null == b && null != a))
                return false;
            if (a.Length != b.Length)
                return false;

            for (long i = 0; i < a.LongLength; i++)
                if (!object.Equals(a[i], b[i]))
                    return false;

            return true;
        }


    }
}
