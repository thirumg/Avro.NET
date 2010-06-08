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
using System.Net;
using System.Net.Sockets;

namespace Avro.RPC
{
    public class RPCClient
    {


        private static readonly Logger log = new Logger();

        public Avro.IO.Encoder Encoder { get; set; }

        public RPCClient():this(BinaryEncoder.Instance)
        {

        }
        public RPCClient(Avro.IO.Encoder encoder)
        {
            if (null == encoder) throw new ArgumentNullException("encoder", "encoder cannot be null.");
            this.Encoder = encoder;
        }

        private Socket _Socket;
        private NetworkStream _Stream;

        public void Connect(string host, int port)
        {
            _Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _Socket.NoDelay = true;
            _Socket.Connect(host, port);
            _Stream = new NetworkStream(_Socket, FileAccess.ReadWrite);
        }

        private static readonly Schema MetadataSchema = new ArraySchema(PrimitiveSchema.Bytes);
        private static readonly IDictionary<string, byte[]> EmptyMetadata = new Dictionary<string, byte[]>();

        public void Invoke(string name, IDictionary<string, byte[]> metadata, params Parameter[] parms)
        {
            IDictionary<string, byte[]> requestMetadata = (metadata == null ? EmptyMetadata : metadata);

            


            throw new NotImplementedException();
        }
        public Response<T> Invoke<T>(string name, IDictionary<string, byte[]> metadata, params Parameter[] parms)
        {
            IDictionary<string, byte[]> requestMetadata = (metadata == null ? EmptyMetadata : metadata);

            

            throw new NotImplementedException();
        }
    }
    
    public interface Parameter
    {
        Schema Schema { get; set; }
        byte[] GetBuffer(Avro.IO.Encoder encoder);
    }
    
    public class Parameter<T> : Parameter
    {
        public Parameter(Schema schema, T data)
        {
            this.Schema = schema;
            this.Data = data;
        }

        public Schema Schema { get; set; }
        public T Data { get; set; }

        public byte[] GetBuffer(Avro.IO.Encoder encoder)
        {
            throw new NotImplementedException();
        }
    }

    public class Response
    {
        public IDictionary<string, byte[]> Metadata { get; set; }
    }
    
    public class Response<T>:Response
    {
        public T Value { get; set; }
    }
}
