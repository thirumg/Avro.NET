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
/* 
 Licensed to the Apache Software Foundation (ASF) under one
or more contributor license agreements.  See the NOTICE file
distributed with this work for additional information
regarding copyright ownership.  The ASF licenses this file
to you under the Apache License, Version 2.0 (the
"License"); you may not use this file except in compliance
with the License.  You may obtain a copy of the License at

http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
 limitations under the License.
*/

using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Avro
{
    public abstract class Schema
    {
        private static readonly Logger log = new Logger();

        public enum Type
        {
            NULL,
            BOOLEAN,
            INT,
            LONG,
            FLOAT,
            DOUBLE,
            BYTES,
            STRING,
            RECORD,
            ENUM,
            ARRAY,
            MAP,
            UNION,
            FIXED,
            ERROR
        }

        public readonly Type type;

        private IDictionary<string, string> Props;

        protected Schema(Type type)
        {
            this.type = type;
            this.Props = new Dictionary<string, string>();
        }

        static Schema()
        {
            Dictionary<string, string> reservedprops = new Dictionary<string, string>(StringComparer.Ordinal);

            reservedprops.Add("type", null);
            reservedprops.Add("name", null);
            reservedprops.Add("namespace", null);
            reservedprops.Add("fields", null);     // Record
            reservedprops.Add("items", null);      // Array
            reservedprops.Add("size", null);       // Fixed
            reservedprops.Add("symbols", null);    // Enum
            reservedprops.Add("values", null);     // Map

            RESERVED_PROPS = reservedprops;
        }

        private static readonly IDictionary<string, string> RESERVED_PROPS;

        internal static Schema ParseJson(JToken j, Names names)
        {
            if (log.IsDebugEnabled) log.DebugFormat("ParseJson(JToken, Names) - j = {0}, names = {1}", j, names);
            if (null == j) throw new ArgumentNullException("j", "j cannot be null.");
            if (log.IsDebugEnabled) log.DebugFormat("ParseJson(JToken, Names) - j.GetType() == {0}", j.GetType());
            
            if (j.Type == JTokenType.String)
            {
                string value = (string)j;

                PrimitiveSchema ps = PrimitiveSchema.GetInstance(value);
                if (null != ps) return ps;

                NamedSchema schema = null;
                if (names.TryGetValue(value, out schema)) return schema;

                throw new SchemaParseException("Undefined name: " + value);
            }
            if (j is JArray) return UnionSchema.NewInstance(j as JArray, names);
            if (j is JObject)
            {
                JObject jo = j as JObject;
                string type = JsonHelper.GetRequiredString(jo, "type");

                Schema schema = PrimitiveSchema.GetInstance(type);
                if (null != schema) return schema;

                if (type.Equals("array")) return ArraySchema.NewInstance(j, names);
                if (type.Equals("map")) return MapSchema.NewInstance(j, names);
                
                schema = NamedSchema.NewInstance(jo, names);
                if (null != schema) return schema;
            }
            throw new AvroTypeException("Invalid JSON for schema: " + j);
        }



        public static Schema Parse(string json)
        {
            if (string.IsNullOrEmpty(json)) throw new ArgumentNullException("json", "json cannot be null.");
            return Parse(json.Trim(), new Names());
        }

        internal static Schema Parse(string json, Names names)
        {
            Schema sc = PrimitiveSchema.GetInstance(json);
            if (null != sc) return sc;

            try
            {
                bool IsArray = json.StartsWith("[") && json.EndsWith("]");
                JContainer j = IsArray ? (JContainer)JArray.Parse(json) : (JContainer)JObject.Parse(json);
                return ParseJson(j, names);
            }
            catch (Newtonsoft.Json.JsonSerializationException ex)
            {
                if (log.IsWarnEnabled) log.Warn("ParseJson(string) - Exception thrown", ex);
                throw new SchemaParseException("Could not parse " + Environment.NewLine + json);
            }
        }

        public override string ToString()
        {
            System.IO.StringWriter sw = new System.IO.StringWriter();
            Newtonsoft.Json.JsonTextWriter writer = new Newtonsoft.Json.JsonTextWriter(sw);
            writeJson(writer);
            return sw.ToString();
        }

        private void writeStartObject(Newtonsoft.Json.JsonTextWriter writer)
        {
            writer.WriteStartObject();

            writer.WritePropertyName("type");
            writer.WriteValue(this.type);
        }

        protected virtual void WriteProperties(Newtonsoft.Json.JsonTextWriter writer)
        {
        }

        internal virtual void writeJson(Newtonsoft.Json.JsonTextWriter writer)
        {
            writeStartObject(writer);
            WriteProperties(writer);

            foreach (KeyValuePair<string, string> kp in this.Props)
            {
                if (log.IsDebugEnabled) log.DebugFormat("Processing \"{0}\"", kp.Key);
                if (RESERVED_PROPS.ContainsKey(kp.Key))
                {
                    if (log.IsWarnEnabled) log.WarnFormat("Skipping reserved property \"{0}\"", kp.Key);
                    continue;
                }

                writer.WritePropertyName(kp.Key);
                writer.WriteValue(kp.Value);
            }

            writer.WriteEndObject();
        }

        public string this[string key]
        {
            get
            {
                string v;
                return (this.Props.TryGetValue(key, out v)) ? v : null;
            }
            set
            {
                if (this.Props.ContainsKey(key))
                    this.Props[key] = value;
                else
                    this.Props.Add(key, value);
            }
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return string.Equals(this.ToString(), obj.ToString());

        }

        public abstract string GetName();
    }
}
