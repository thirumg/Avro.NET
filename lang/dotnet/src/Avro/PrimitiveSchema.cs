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
    public class PrimitiveSchema:Schema
    {
        public static readonly Schema Null;
        public static readonly Schema Boolean;
        public static readonly Schema Int;
        public static readonly Schema Long;
        public static readonly Schema Float;
        public static readonly Schema Double;
        public static readonly Schema Bytes;
        public static readonly Schema String;
        public static readonly string[] SupportedTypes;
        //public static readonly IDictionary<SchemaType, string> PrimitiveValueLookup;
        public static readonly IDictionary<string, string> PrimitiveKeyLookup;
        static PrimitiveSchema()
        {
            SupportedTypes = new string[]{
                  "null",
  "boolean",
  "string",
  "bytes",
  "int",
  "long",
  "float",
  "double"
            };

            Dictionary<string, string> keylookup = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (string key in SupportedTypes)
            {
                keylookup.Add(key, key);
            }
            PrimitiveKeyLookup = keylookup;

            Null = new PrimitiveSchema(SchemaType.NULL);
            Boolean = new PrimitiveSchema(SchemaType.BOOLEAN);
            Int = new PrimitiveSchema(SchemaType.INT);
            Long = new PrimitiveSchema(SchemaType.LONG);
            Float = new PrimitiveSchema(SchemaType.FLOAT);
            Double = new PrimitiveSchema(SchemaType.DOUBLE);
            Bytes = new PrimitiveSchema(SchemaType.BYTES);
            String = new PrimitiveSchema(SchemaType.STRING);
        }

        public PrimitiveSchema(string type)
            : base(type)
        {
            if (!PrimitiveKeyLookup.ContainsKey(type))
            {
                throw new NotSupportedException("Type \"" + type + "\" is not a primitive.");
            }
        }



        public static Schema Create(string json)
        {
            if (!IsPrimitive(json))
                throw new NotSupportedException(json + " is not supported as a primitive type.");
            return new PrimitiveSchema(json.Trim('\"'));
        }

        public static bool IsPrimitive(string json)
        {
            return PrimitiveKeyLookup.ContainsKey(json.Trim('\"'));
        }

        public override int GetHashCode()
        {
            return this.Type.GetHashCode();
        }


    }
}
