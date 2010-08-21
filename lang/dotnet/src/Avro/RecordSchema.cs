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
    public class RecordSchema : NamedSchema
    {
        public IDictionary<string, Field> Fields;

        private IDictionary<string, Field> _fieldLookup;

        public static RecordSchema NewInstance(Type type, JToken j, Names names)
        {
            JToken jfields = j["fields"];

            if (null == jfields)
            {
                throw new SchemaParseException("'fields' cannot be null for record");
            }

            if (jfields.Type != JTokenType.Array) {
                throw new SchemaParseException("'fields' not an array for record");
            }

            Name name = GetName(j);
            IDictionary<string, Field> fields = new Dictionary<string, Field>();
            foreach (JObject jfield in jfields)
            {
                string fieldName = JsonHelper.GetRequiredString(jfield, "name");
                Field field = createField(jfield, names);
                fields.Add(fieldName, field);
            }
            return new RecordSchema(type, name, fields);
        }

        private static Field createField(JToken jfield, Names names)
        {
            string name = JsonHelper.GetRequiredString(jfield, "name");
            string doc = JsonHelper.GetOptionalString(jfield, "doc");

            JToken jtype = jfield["type"];
            if (null == jtype)
            {
                throw new SchemaParseException("'type' was not found for field: " + name);
            }
            Schema type = Schema.ParseJson(jtype, names);

            return new Field(type, name, false);
        }

        private RecordSchema(Type type, Name name, IDictionary<string, Field> fields)
            : base(type, name) {
            this.Fields = fields;
        }

        public new Field this[string name]
        {
            get
            {
                if (string.IsNullOrEmpty(name))
                {
                    throw new ArgumentNullException("name", "name cannot be null.");
                }
                Field field;

                if (_fieldLookup.TryGetValue(name, out field))
                {
                    return field;
                }
                return null;
            }
        }

        protected override void WriteProperties(Newtonsoft.Json.JsonTextWriter writer)
        {
            base.WriteProperties(writer);

            if (null != this.Fields && this.Fields.Count > 0)
            {
                writer.WritePropertyName("fields");
                writer.WriteStartArray();

                foreach (Field field in this.Fields.Values)
                {
                    field.writeJson(writer);
                }

                writer.WriteEndArray();
            }
        }
    }
}
