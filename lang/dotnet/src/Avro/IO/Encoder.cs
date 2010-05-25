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

namespace Avro.IO
{
    public interface Encoder
    {
        void WriteArrayStart(Stream iostr);
        void WriteArrayEnd(Stream iostr);

        void WriteMapStart(Stream iostr);
        void SetItemCount(Stream iostr, int value);
        void StartItem(Stream iostr);
        void WriteString(Stream iostr, string value);
        void WriteBytes(Stream iostr, byte[] value);
        void WriteMapEnd(Stream iostr);
        void WriteInt(Stream iostr, int value);
        void WriteLong(Stream iostr, long value);
        void WriteFloat(Stream iostr, float value);
        void WriteDouble(Stream iostr, double value);
        void WriteBoolean(Stream iostr, bool value);
        void WriteNull(Stream iostr);
    }
}
