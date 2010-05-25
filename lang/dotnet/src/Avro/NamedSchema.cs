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
    public abstract class NamedSchema:Schema
    {
        public Name Name { get; private set; }

        //public string Namespace
        //{
        //    get { return this.Name.space; }
        //}

        //public string FullName
        //{
        //    get
        //    {
        //        return this.Name.full;
        //    }
        //}
        public NamedSchema(string type, Name name, Names names)
            : base(type)
        {
            if(null==name)throw new ArgumentNullException("name", "name cannot be null.");
            

            if (PrimitiveSchema.PrimitiveKeyLookup.ContainsKey(name.full))
            {
                throw new SchemaParseException("Schemas may not be named after primitives: " + name.full);
            }

            this.Name = name;
            //Avro.Name a = Avro.Name.extract_namespace(name, snamespace);
        }


        public static readonly string[] SupportedTypes;
        public static readonly IDictionary<string, string> PrimitiveKeyLookup;
        

        static NamedSchema()
        {
            SupportedTypes = new string[]{
               "fixed",
              "enum",
              "record",
              "error"
            };

            Dictionary<string, string> keylookup = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (string key in SupportedTypes)
            {
                keylookup.Add(key, key);
            }
            PrimitiveKeyLookup = keylookup;
        }




        protected override void WriteProperties(Newtonsoft.Json.JsonTextWriter writer)
        {
            base.WriteProperties(writer);
            this.Name.WriteJson(writer);

            //JsonHelper.writeIfNotNullOrEmpty(writer, "name", this.Name);
            JsonHelper.writeIfNotNullOrEmpty(writer, "namespace", this.Name.space);
        }
    }
}
