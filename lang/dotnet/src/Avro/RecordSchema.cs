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
    public class RecordSchema:NamedSchema
    {
        public IList<Field> Fields { get; set; }
        private IDictionary<string, Field> _fieldLookup;

        public RecordSchema(Name name)
            : this(SchemaType.RECORD, name, null, null)
        {

        }

        public RecordSchema(Name name, IEnumerable<Field> fields, Names names)
            : this(SchemaType.RECORD, name, fields, names)
        {

        }

        public RecordSchema(string type, Name name, IEnumerable<Field> fields, Names names)
            : base(type, name, names)
        {
            //if (null == fields) throw new ArgumentNullException("fields", "fields cannot be null.");
            this.Fields = new List<Field>();

            Dictionary<string, Field> fieldLookup = new Dictionary<string, Field>(StringComparer.Ordinal);

            if (null != fields)
                foreach (Field field in fields)
                {
                    AddField(field);
                }

            _fieldLookup = fieldLookup;
        }

        public void AddField(Field field)
        {
            this.Fields.Add(field);
            _fieldLookup.Add(field.Name, field);
        }

        


        public new Field this[string name]
        {
            get
            {
                if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name", "name cannot be null.");
                Field field = null;

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

                foreach (Field field in this.Fields)
                {
                    field.writeJson(writer);
                }

                writer.WriteEndArray();
            }
        }
    }
}
