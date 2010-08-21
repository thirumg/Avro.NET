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
    public enum SortOrder
    {
        ASCENDING,
        DESCENDING,
        IGNORE
    }

    public class Field
    {
        /// <summary>
        /// Field Name
        /// </summary>
        public readonly string name;
        /// <summary>
        /// Documentation
        /// </summary>
        public readonly string documentation;
        public readonly object defaultValue;
        public readonly bool hasDefault;
        public readonly SortOrder? sortOrder;
        public readonly Schema schema;


        internal Field(Schema schema, string name, string defaultValue)
            : this(schema, name, defaultValue != null, defaultValue, SortOrder.IGNORE, null)
        {

        }

        internal Field(Schema type, string name, bool hasDefault, object oDefault, SortOrder sortorder, Names names)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name", "name cannot be null.");
            if (null == type) throw new ArgumentNullException("type", "type cannot be null.");
            this.schema = type;
            this.name = name;
            this.hasDefault = hasDefault;
            this.defaultValue = oDefault;
        }

        internal void writeJson(Newtonsoft.Json.JsonTextWriter writer)
        {
            writer.WriteStartObject();
            JsonHelper.writeIfNotNullOrEmpty(writer, "name", this.name);
            JsonHelper.writeIfNotNullOrEmpty(writer, "doc", this.documentation);

            if (null != this.schema)
            {
                writer.WritePropertyName("type");
                this.schema.writeJson(writer);
            }

            writer.WriteEndObject();
        }
    }
}
