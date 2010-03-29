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
    public enum SchemaType
    {
        Null,
        Boolean,
        String,
        Bytes,
        Int,
        Long,
        Float,
        Double,
        Fixed,
        Enum,
        Record,
        Error,
        Array,
        Map,
        Union,
        Request
    }

    public abstract class Schema
    {
        private static readonly Logger log = new Logger();
        public SchemaType Type { get; private set; }

        public Schema(SchemaType type)
        {
            this.Type = type;
        }

        static Schema Parse(JToken j, Names names)
        {
            if (log.IsDebugEnabled) log.DebugFormat("Parse(JToken, Names) - j = {0}, names = {1}", j, names);
            if (j is JValue)
            {
                string value = j.Value<string>();

                SchemaType primtype = SchemaType.Null;
                if (PrimitiveSchema.PrimitiveKeyLookup.TryGetValue(value, out primtype))
                {
                    return new PrimitiveSchema(primtype);
                }

               // throw new SchemaParseException("\"" + value + "\" is not a primitive type");
            }

            if (j is JArray)
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


            string stype = getRequiredString(j, "type");
            if (log.IsDebugEnabled) log.DebugFormat("Parse(JObject) - type = \"{0}\"", stype);
            SchemaType type = SchemaType.Null;

            if (!EnumHelper<SchemaType>.TryParse(stype, out type))
            {
                throw new SchemaParseException(string.Format("Undefined type: \"{0}\"", stype));
            }

            if (checkIsValue(type, PrimitiveSchema.SupportedTypes))
            {
                if (log.IsDebugEnabled) log.DebugFormat("\"{0}\" is primitive to returning PrimitiveSchema", EnumHelper<SchemaType>.ToStringLower(type));
                return new PrimitiveSchema(type);
            }
            if(checkIsValue(type, NamedSchema.SupportedTypes))
            {
                if (log.IsDebugEnabled) log.DebugFormat("\"{0}\" is named.", EnumHelper<SchemaType>.ToStringLower(type));
                string name = getRequiredString(j, "name");
                string snamespace = getOptionalString(j, "namespace");
                string doc = getOptionalString(j, "doc");

                switch (type)
                {
                    case SchemaType.Fixed:
                        string ssize = getRequiredString(j, "size");
                        int size = 0;
                        if (!int.TryParse(ssize, out size))
                        {
                            throw new SchemaParseException("Could not parse \"" + ssize + "\" to int32.");
                        }
                        return new FixedSchema(name, snamespace, size);
                    case SchemaType.Enum:
                        JArray jsymbols = j["symbols"] as JArray;
                        List<string> symbols = new List<string>();
                        foreach (JValue jsymbol in jsymbols)
                        {
                            symbols.Add(jsymbol.Value<string>());
                        }
                        return new EnumSchema(name, snamespace, symbols.ToArray(), names);
                    case SchemaType.Record:
                    case SchemaType.Error:

                        JToken jfields = j["fields"];
                        if (null == jfields)
                        {
                            throw new SchemaParseException("Could not find field \"fields\"");
                        }

                        List<Field> fields = new List<Field>();
                        foreach (JObject jfield in jfields)
                        {
                            if (log.IsDebugEnabled) log.DebugFormat("{0}", jfield);
                            string fieldName = getRequiredString(jfield, "name");
                            if (log.IsDebugEnabled) log.DebugFormat("fieldname = \"{0}\"", fieldName);

                            Field field = createField(jfield);
                            fields.Add(field);
                        }

                        return new RecordSchema(name, snamespace, fields, null);
                }
            }
            else if (SchemaType.Array == type)
            {
                JToken items = j["items"];

                Schema arraySchema = Schema.Parse(items, names);

                if (log.IsDebugEnabled) log.DebugFormat("items = {0}", items.GetType());

                return new ArraySchema(arraySchema); ;
            }
            else if (SchemaType.Map == type)
            {
                JToken values = j["values"];
                Schema valuesSchema = Schema.Parse(values, names);
                return new MapSchema(valuesSchema);
            }



            throw new NotSupportedException(type + " is not supported");
        }



        static Field createField(JToken jfield)
        {          
            string name = getRequiredString(jfield, "name");
            string doc = getOptionalString(jfield, "doc");

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

        static string getOptionalString(JToken j, string property)
        {
            if (log.IsDebugEnabled) log.DebugFormat("getOptionalString(JToken, string) - property = {1}, j = {0}", j, property); 
            if (null == j) throw new ArgumentNullException("j", "j cannot be null.");
            if (string.IsNullOrEmpty(property)) throw new ArgumentNullException("property", "property cannot be null.");

            JToken child = j[property];
            if (log.IsDebugEnabled) log.DebugFormat("getOptionalString(JToken, string) - child = {0}", child);
            if (null == child) return null; 

            string value = child.Value<string>();
            return value.Trim('\"');
        }

        static string getRequiredString(JToken j, string property)
        {
            string value = getOptionalString(j, property);
            if(string.IsNullOrEmpty(value))
                throw new SchemaParseException(string.Format("No \"{0}\" property: {1}", property, j));
            return value;
        }

        static bool checkIsValue(SchemaType type, params SchemaType[] types)
        {
            foreach (SchemaType t in types)
                if (t == type)
                    return true;

            return false;
        }

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
    }
}
