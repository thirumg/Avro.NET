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
    //public enum SchemaType
    //{
    //    Null,
    //    Boolean,
    //    String,
    //    Bytes,
    //    Int,
    //    Long,
    //    Float,
    //    Double,
    //    Fixed,
    //    Enum,
    //    Record,
    //    Error,
    //    Array,
    //    Map,
    //    Union,
    //    Request
    //}

    public class Schema
    {
        public const string UNION = "union";
        public const string BOOLEAN = "boolean";
        public const string INT = "int";
        public const string LONG = "long";
        public const string FLOAT = "float";
        public const string DOUBLE = "double";
        public const string ARRAY = "array";
        public const string MAP = "map";
        public const string STRING = "string";
        public const string BYTES = "bytes";
        public const string NULL = "null";
        public const string ENUM = "enum";
        public const string FIXED = "fixed";
        public const string RECORD = "record";
        public const string ERROR = "error";
        private static readonly Logger log = new Logger();
        public string Type { get; private set; }
        private IDictionary<string, string> Props;
        public Schema(string type)
        {
            if (string.IsNullOrEmpty(type)) throw new ArgumentNullException("type", "type cannot be null.");
            this.Type = type;
            this.Props = new Dictionary<string, string>();
        }

        static Schema()
        {
            Dictionary<string, string> reservedprops=new Dictionary<string,string>(StringComparer.Ordinal);
            
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
            if (j is JValue)
            {
                string value = j.Value<string>();

                if (PrimitiveSchema.PrimitiveKeyLookup.ContainsKey(value))
                {
                    return new PrimitiveSchema(value);
                }

                Schema schema = null;
                if (names.TryGetValue(value, out schema))
                    return schema;

                throw new SchemaParseException("Undefined name: " + value);
            }
            else if (j is JArray)
            {
                JArray array = j as JArray;

                List<Schema> schemas = new List<Schema>();

                foreach(JToken jvalue in array)
                {
                    Schema unionTypes = Schema.ParseJson(jvalue, names);
                    schemas.Add(unionTypes);
                }

                return new UnionSchema(schemas);

            }
            else if (j is JObject)
            {

                string type = JsonHelper.getRequiredString(j, "type");
                if (log.IsDebugEnabled) log.DebugFormat("ParseJson(JObject) - type = \"{0}\"", type);
                //string type = SchemaType.Null;

                //if (!EnumHelper<SchemaType>.TryParse(stype, out type))
                //{
                //    throw new SchemaParseException(string.Format("Undefined type: \"{0}\"", stype));
                //}

                Schema schema = null;

                if (Util.checkIsValue(type, PrimitiveSchema.SupportedTypes))
                {
                    if (log.IsDebugEnabled) log.DebugFormat("\"{0}\" is primitive to returning PrimitiveSchema", type);
                    schema = new PrimitiveSchema(type);
                }
                else if (Util.checkIsValue(type, NamedSchema.SupportedTypes))
                {
                    if (log.IsDebugEnabled) log.DebugFormat("\"{0}\" is named.", type);
                    string sname = JsonHelper.getRequiredString(j, "name");
                    string snamespace = JsonHelper.getOptionalString(j, "namespace");
                    Name name = Name.make_fullname(sname, snamespace);

                    string doc = JsonHelper.getOptionalString(j, "doc");

                    switch (type)
                    {
                        case "fixed":
                            string ssize = JsonHelper.getRequiredString(j, "size");
                            int size = 0;
                            if (!int.TryParse(ssize, out size))
                            {
                                throw new SchemaParseException("Could not parse \"" + ssize + "\" to int32.");
                            }
                            schema = new FixedSchema(name, size);
                            if (null != name && !names.Contains(schema)) names.Add(schema);
                            break;
                        case "enum":
                            JArray jsymbols = j["symbols"] as JArray;
                            List<string> symbols = new List<string>();
                            foreach (JValue jsymbol in jsymbols)
                            {
                                symbols.Add(jsymbol.Value<string>());
                            }
                            schema = new EnumSchema(name, symbols, names);
                            if (null != name && !names.Contains(schema)) names.Add(schema);
                            break;
                        case "record":
                        case "error":
                            
                            JToken jfields = j["fields"];

                            if (null == jfields) throw new SchemaParseException("'fields' cannot be null.");

                            RecordSchema record = null;

                            if (Schema.ERROR == type)
                                record = new ErrorSchema(name, null, names);
                            else if (Schema.RECORD == type)
                                record = new RecordSchema(name, null, names);

                            if (null != name && !names.Contains(schema)) names.Add(record);
                            if (null != jfields)
                                if (jfields.Type == JTokenType.Array)
                                {

                                    foreach (JObject jfield in jfields)
                                    {
                                        if (log.IsDebugEnabled) log.DebugFormat("{0}", jfield);
                                        string fieldName = JsonHelper.getRequiredString(jfield, "name");
                                        if (log.IsDebugEnabled) log.DebugFormat("fieldname = \"{0}\"", fieldName);

                                        Field field = createField(jfield, names);
                                        record.AddField(field);
                                    }
                                }
                                else if (jfields.Type == JTokenType.Null)
                                {

                                }
                                else
                                {
                                    throw new SchemaParseException("'fields' has an unknown tokentype of '" + jfields.Type + "' supported types are Null or Array");
                                }
                            schema = record;
                            
                            break;
                    }

                    
                }
                else if ("array" == type)
                {
                    JToken items = j["items"];
                    //if (null == items) throw new AvroException("'items' cannot be null.");
                    Schema arraySchema = Schema.ParseJson(items, names);

                    if (log.IsDebugEnabled) log.DebugFormat("items = {0}", items.GetType());

                    return new ArraySchema(arraySchema); ;
                }
                else if ("map" == type)
                {
                    JToken values = j["values"];
                    Schema valuesSchema = Schema.ParseJson(values, names);
                    return new MapSchema(valuesSchema);
                }
                else
                {
                    if (!names.TryGetValue(type, out schema))
                    {
                        throw new AvroTypeException("Schema '" + type + "' is not a known type");
                    }
                }
                

                foreach (JToken p   in j.Children())
                {
                    if (p is JProperty)
                    {
                        JProperty prop = p as JProperty;
                        if (RESERVED_PROPS.ContainsKey(prop.Name))
                        {
                            if (log.IsDebugEnabled) log.DebugFormat("Skipping reserved property \"{0}\"", prop);
                            continue;
                        }

                        if (prop.Value is JArray)
                        {
                            schema[prop.Name] = prop.Value.ToString();
                        }
                        else
                        {
                            try
                            {
                                schema[prop.Name] = prop.Value.ToString().Trim('"');
                            }
                            catch
                            {
                                throw;
                            }
                        }
                    }
                    else
                    {
                        throw new NotSupportedException(p.ToString());
                    }
                    
                }


                return schema;
            }


            throw new NotSupportedException();
        }



        static Field createField(JToken jfield, Names names)
        {
            if (log.IsDebugEnabled) log.DebugFormat("createField(JToken) - jfield = {0}", jfield);
            string name = JsonHelper.getRequiredString(jfield, "name");
            string doc = JsonHelper.getOptionalString(jfield, "doc");

            JToken jtype = jfield["type"];
            if (null == jtype) 
                throw new SchemaParseException("'type' was not found.");
            Schema type = Schema.ParseJson(jtype, names);

            return new Field(type, name, false);

            //SchemaType type = SchemaType.Error;
            //if (!EnumHelper<SchemaType>.TryParse(stype, out type))
            //{

            //}
            
            //bool hasDefault = false;

            //object odefault = null;


            //if (null != jfield["default"])
            //{
            //    hasDefault = true;
            //}

            //string sorder = getOptionalString(jfield, "order");
            //SortOrder order = SortOrder.Ignore;
            //if (EnumHelper<SortOrder>.TryParse(sorder, out order))
            //{
            //    order = SortOrder.Ignore;
            //}

            //return new Field(type, name, hasDefault, odefault, order, null);
        }



        //static bool checkIsValue(string type, params string[] types)
        //{
        //    foreach (string t in types)
        //        if (t == type)
        //            return true;

        //    return false;
        //}
        public static Schema Parse(string json)
        {
            return Parse(json, null);
        }

        public static Schema Parse(string json, Names names)
        {
            if (log.IsDebugEnabled) log.DebugFormat("ParseJson(string) - json = \"{0}\"", json);
            if (string.IsNullOrEmpty(json)) throw new ArgumentNullException("json", "json cannot be null.");

            if (null == names)
                names = new Names();
            

            if (PrimitiveSchema.IsPrimitive(json))
            {
                return PrimitiveSchema.Create(json);
            }

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
            System.IO.StringWriter sw=new System.IO.StringWriter();
            Newtonsoft.Json.JsonTextWriter writer = new Newtonsoft.Json.JsonTextWriter(sw);
            writeJson(writer);
            return sw.ToString();
        }

        private void writeStartObject(Newtonsoft.Json.JsonTextWriter writer)
        {
            writer.WriteStartObject();

            writer.WritePropertyName("type");
            writer.WriteValue(this.Type);
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
                string v = null;
                if (this.Props.TryGetValue(key, out v))
                    return v;
                return null;
            }
            set
            {
                if (this.Props.ContainsKey(key))
                    this.Props[key] = value;
                else
                    this.Props.Add(key, value);
            }


        }

        public override bool Equals(object obj)
        {
            return string.Equals(this.ToString(), obj.ToString());

        }
    }
}
