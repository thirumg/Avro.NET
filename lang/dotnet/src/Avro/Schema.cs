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
        private static readonly Logger log = new Logger();
        public string Type { get; private set; }

        public Schema(string type)
        {
            this.Type = type;
        }

        internal static Schema Parse(JToken j, Names names)
        {
            if (log.IsDebugEnabled) log.DebugFormat("Parse(JToken, Names) - j = {0}, names = {1}", j, names);
            if (null == j) throw new ArgumentNullException("j", "j cannot be null.");
            if (log.IsDebugEnabled) log.DebugFormat("Parse(JToken, Names) - j.GetType() == {0}", j.GetType());
            if (j is JValue)
            {
                string value = j.Value<string>();

                
                if (PrimitiveSchema.PrimitiveKeyLookup.ContainsKey(value))
                {
                    return new PrimitiveSchema(value);
                }

                //return new NamedSchema(

                //return null;
                return new Schema(value);
                //throw new SchemaParseException("\"" + value + "\" is not a primitive type");
            }
            else if (j is JArray)
            {
                JArray array = j as JArray;

                List<Schema> schemas = new List<Schema>();

                foreach(JToken jvalue in array)
                {
                    Schema unionTypes = Schema.Parse(jvalue, names);
                    schemas.Add(unionTypes);
                }

                return new UnionSchema(schemas);

            }
            else if (j is JObject)
            {

                string type = JsonHelper.getRequiredString(j, "type");
                if (log.IsDebugEnabled) log.DebugFormat("Parse(JObject) - type = \"{0}\"", type);
                //string type = SchemaType.Null;

                //if (!EnumHelper<SchemaType>.TryParse(stype, out type))
                //{
                //    throw new SchemaParseException(string.Format("Undefined type: \"{0}\"", stype));
                //}

                if (Util.checkIsValue(type, PrimitiveSchema.SupportedTypes))
                {
                    if (log.IsDebugEnabled) log.DebugFormat("\"{0}\" is primitive to returning PrimitiveSchema", type);
                    return new PrimitiveSchema(type);
                }
                if (Util.checkIsValue(type, NamedSchema.SupportedTypes))
                {
                    if (log.IsDebugEnabled) log.DebugFormat("\"{0}\" is named.", type);
                    string sname = JsonHelper.getRequiredString(j, "name");
                    string snamespace = JsonHelper.getOptionalString(j, "namespace");
                    string name = Name.make_fullname(sname, snamespace);
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
                            return new FixedSchema(name, snamespace, size);
                        case "enum":
                            JArray jsymbols = j["symbols"] as JArray;
                            List<string> symbols = new List<string>();
                            foreach (JValue jsymbol in jsymbols)
                            {
                                symbols.Add(jsymbol.Value<string>());
                            }
                            return new EnumSchema(name, snamespace, symbols.ToArray(), names);
                        case "record":
                        case "error":

                            JToken jfields = j["fields"];
                            List<Field> fields = new List<Field>();

                            if (null != jfields)
                            {
                                foreach (JObject jfield in jfields)
                                {
                                    if (log.IsDebugEnabled) log.DebugFormat("{0}", jfield);
                                    string fieldName = JsonHelper.getRequiredString(jfield, "name");
                                    if (log.IsDebugEnabled) log.DebugFormat("fieldname = \"{0}\"", fieldName);

                                    Field field = createField(jfield);
                                    fields.Add(field);
                                }
                            }

                            return new RecordSchema(name, snamespace, fields, null);
                    }
                }
                else if ("array" == type)
                {
                    JToken items = j["items"];

                    Schema arraySchema = Schema.Parse(items, names);

                    if (log.IsDebugEnabled) log.DebugFormat("items = {0}", items.GetType());

                    return new ArraySchema(arraySchema); ;
                }
                else if ("map" == type)
                {
                    JToken values = j["values"];
                    Schema valuesSchema = Schema.Parse(values, names);
                    return new MapSchema(valuesSchema);
                }

                return new Schema(type);
            }


            throw new NotSupportedException();
        }



        static Field createField(JToken jfield)
        {
            string name = JsonHelper.getRequiredString(jfield, "name");
            string doc = JsonHelper.getOptionalString(jfield, "doc");

            JToken jtype = jfield["type"];

            Schema type = Schema.Parse(jtype, new Names());

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
            if (log.IsDebugEnabled) log.DebugFormat("Parse(string) - json = \"{0}\"", json);
            if (string.IsNullOrEmpty(json)) throw new ArgumentNullException("json", "json cannot be null.");
            
            
            Names names = new Names();

            try
            {
                bool IsArray = json.StartsWith("[") && json.EndsWith("]");

                JContainer j = IsArray ? (JContainer)JArray.Parse(json) : (JContainer)JObject.Parse(json);
                
                return Parse(j, names);

            }

            catch (Newtonsoft.Json.JsonSerializationException ex)
            {
                if (log.IsWarnEnabled) log.Warn("Parse(string) - Exception thrown", ex);
                throw new SchemaParseException("Could not parse " + Environment.NewLine + json);
            }


        }

        protected virtual void writeStartObject(Newtonsoft.Json.JsonTextWriter writer)
        {
            writer.WriteStartObject();

            writer.WritePropertyName("type");
            writer.WriteValue(this.Type);
        }

        internal virtual void writeJson(Newtonsoft.Json.JsonTextWriter writer)
        {
            writeStartObject(writer);


            writer.WriteEndObject();
        }

        public object GetProp(string p)
        {
            throw new NotImplementedException();
        }

        public void AddProp(string p, string p_2)
        {
            throw new NotImplementedException();
        }
    }
}
