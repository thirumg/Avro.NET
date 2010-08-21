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
using Newtonsoft.Json.Linq;

namespace Avro
{
    public class MapSchema : UnnamedSchema
    {
        internal static MapSchema NewInstance(JToken j, Names names)
        {
            JToken t = j["values"];
            if (null == t)
            {
                throw new AvroTypeException("Map does not have 'values'");
            }
            return new MapSchema(Schema.ParseJson(t, names));
        }

        public Schema valueSchema { get; private set; }
        public MapSchema(Schema valueSchema)
            :base(Type.MAP)
        {
            if (null == valueSchema) throw new ArgumentNullException("valueSchema", "valueSchema cannot be null.");
            this.valueSchema = valueSchema;
        }

        protected override void WriteProperties(Newtonsoft.Json.JsonTextWriter writer)
        {
            writer.WritePropertyName("values");
            valueSchema.writeJson(writer);
        }
    }
}
