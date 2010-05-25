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
using System.IO;

namespace Avro.IO
{
    public interface Decoder
    {
        bool ReadBool(Stream Stream);
        byte[] ReadBytes(Stream Stream);
        double ReadDouble(Stream Stream);
        void ReadFixed(Stream Stream, byte[] buffer);
        float ReadFloat(Stream Stream);
        int ReadInt(Stream Stream);
        long ReadLong(Stream Stream);
        long ReadMapStart(Stream Stream);
        object ReadNull();
        string ReadString(Stream Stream);
        void SkipBoolean(Stream Stream);
        void SkipBytes(Stream Stream);
        void SkipDouble(Stream Stream);
        void SkipFloat(Stream Stream);
        void SkipInt(Stream Stream);
        void SkipLong(Stream Stream);
        void SkipNull(Stream Stream);
        void SkipString(Stream Stream);
    }


}
