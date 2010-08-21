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
    public class UnionSchema : UnnamedSchema
    {
        public IList<Schema> schemas { get; private set; }

        public static UnionSchema NewInstance(JArray a, Names names)
        {
            List<Schema> schemas = new List<Schema>();
            ISet<string> uniqueSchemas = new HashSet<string>();

            foreach (JToken jvalue in a)
            {
                Schema unionTypes = Schema.ParseJson(jvalue, names);
                string name = unionTypes.GetName();
                if (uniqueSchemas.Contains(name))
                {
                    throw new SchemaParseException("Duplicate type in union: " + name);
                }
                uniqueSchemas.Add(name);
                schemas.Add(unionTypes);
            }

            return new UnionSchema(schemas);
        }

        private UnionSchema(List<Schema> schemas)
            : base(Type.UNION)
        {
            if (schemas.Count == 0)
            {
                throw new ArgumentNullException("Union members cannot be null or empty");
            }

            this.schemas = schemas;
        }

        internal override void writeJson(Newtonsoft.Json.JsonTextWriter writer)
        {
            writer.WriteStartArray();
            foreach (Schema schema in this.schemas)
            {
                schema.writeJson(writer);
            }
            writer.WriteEndArray();
        }
    }
}
