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
using System.IO.Compression;
using Avro.IO;


namespace Avro
{
    public class DataFileReader<T>
    {

        public Schema Schema { get; private set; }
        private Stream stream;
        private Decoder _Decoder;
        private Dictionary<string, byte[]> _metadata = new Dictionary<string, byte[]>();
        private byte[] _Sync = new byte[DataFileConstants.SYNC_SIZE];
        private DatumReader<T> _Reader;
        public DataFileReader(Stream input, DatumReader<T> reader)
        {
            if (null == input) throw new ArgumentNullException("input", "input cannot be null.");
            this.stream = input;
            this._Reader = reader;
            init(input);

        }

        private void init(Stream input)
        {
            //TODO: Should this be a buffered stream?
            this._Decoder = BinaryDecoder.Instance;
            byte[] magic = new byte[DataFileConstants.MAGIC.Length];
            try
            {
                _Decoder.ReadFixed(input, magic);
            }
            catch (IOException)
            {
                throw new IOException("Not a data file.");
            }

            if (!ArrayHelper<byte>.Equals(magic, DataFileConstants.MAGIC))
                throw new IOException("Not a data file.");

            long l = _Decoder.ReadMapStart(input);

            if (l > 0)
            {
                for (long i = 0; i < l; i++)
                {
                    string key = _Decoder.ReadString(input);
                    byte[] buffer = _Decoder.ReadBytes(input);
                    _metadata.Add(key, buffer);
                }
            }
            _Decoder.ReadFixed(input, _Sync);
            this.Schema = Schema.Parse(getMetaString(DataFileConstants.SCHEMA));
            //TODO: Resolve the codec. 
            _Reader.Schema = this.Schema;


        }

        //public byte[] this[string key]
        //{
        //    get
        //    {
        //        if (string.IsNullOrEmpty(key)) throw new ArgumentNullException("key", "key cannot be null.");
        //        byte[] buffer = null;

        //        if (_metadata.TryGetValue(key, out buffer))
        //            return buffer;
        //        return null;
        //    }
        //    set
        //    {
        //        if (string.IsNullOrEmpty(key)) throw new ArgumentNullException("key", "key cannot be null.");
        //        if (isReserved(key))
        //            throw new AvroException("Cannot set reserved meta key: " + key);
        //        setMetaInternal(key, value);
        //    }
        //}

        private void setMetaInternal(string key, string value)
        {
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(value);
            setMetaInternal(key, buffer);
        }

        private void setMetaInternal(string key, byte[] value)
        {
            if (_metadata.ContainsKey(key))
                _metadata[key] = value;
            else
                _metadata.Add(key, value);
        }

        private string getMetaString(string key)
        {
            byte[] buffer = null;
            if (_metadata.TryGetValue(key, out buffer))
                return System.Text.Encoding.UTF8.GetString(buffer);
            return null;
        }

        public IEnumerable<string> GetItems()
        {

            yield break;
        }
    }
}
