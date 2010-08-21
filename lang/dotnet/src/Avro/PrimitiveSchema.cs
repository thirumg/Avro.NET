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
    public sealed class PrimitiveSchema : UnnamedSchema, IEquatable<Schema>
    {
        public static readonly PrimitiveSchema NULL = new PrimitiveSchema(Schema.Type.NULL);
        public static readonly PrimitiveSchema BOOLEAN = new PrimitiveSchema(Schema.Type.BOOLEAN);
        public static readonly PrimitiveSchema INT = new PrimitiveSchema(Schema.Type.INT);
        public static readonly PrimitiveSchema LONG = new PrimitiveSchema(Schema.Type.LONG);
        public static readonly PrimitiveSchema FLOAT = new PrimitiveSchema(Schema.Type.FLOAT);
        public static readonly PrimitiveSchema DOUBLE = new PrimitiveSchema(Schema.Type.DOUBLE);
        public static readonly PrimitiveSchema BYTES = new PrimitiveSchema(Schema.Type.BYTES);
        public static readonly PrimitiveSchema STRING = new PrimitiveSchema(Schema.Type.STRING);

        public static readonly IDictionary<Type, PrimitiveSchema> primitiveTypes = new Dictionary<Type, PrimitiveSchema>();
        static PrimitiveSchema()
        {
            PrimitiveSchema[] ps = new PrimitiveSchema[] {
                NULL, BOOLEAN, INT, LONG, FLOAT, DOUBLE, BYTES, STRING
            };

            foreach (PrimitiveSchema p in ps)
            {
                primitiveTypes.Add(p.type, p);
            }
        }

        private PrimitiveSchema(Type type) :
            base(type)
        {
        }

        public static PrimitiveSchema GetInstance(string type)
        {
            switch (type)
            {
                case "null":
                    return NULL;
                case "boolean":
                    return BOOLEAN;
                case "int":
                    return INT;
                case "long":
                    return LONG;
                case "float":
                    return FLOAT;
                case "double":
                    return DOUBLE;
                case "bytes":
                    return BYTES;
                case "string":
                    return STRING;
                default:
                    return null;
            }
        }

        public override int GetHashCode()
        {
            return this.type.GetHashCode();
        }

        public bool Equals(Schema other)
        {
            if (null != other && other is PrimitiveSchema)
            {
                PrimitiveSchema o = other as PrimitiveSchema;
                return o.type == this.type;
            }
            return false;
        }
    }
}
