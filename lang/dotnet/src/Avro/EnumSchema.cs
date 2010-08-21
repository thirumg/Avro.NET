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
    public class EnumSchema : NamedSchema
    {
        public IList<string> symbols { get; set; }

        public static EnumSchema NewInstance(JToken j)
        {
            Name name = NamedSchema.GetName(j);
            JArray jsymbols = j["symbols"] as JArray;
            if (null == jsymbols)
            {
                throw new SchemaParseException("Enum has no symbols: " + name);
            }
            List<string> symbols = new List<string>();
            ISet<string> uniqueSymbols = new HashSet<string>();
            foreach (JValue jsymbol in jsymbols)
            {
                string s = jsymbol.Value<string>();
                if (uniqueSymbols.Contains(s))
                {
                    throw new SchemaParseException("Duplicate symbol: " + s);
                }
                uniqueSymbols.Add(s);
                symbols.Add(s);
            }
            return new EnumSchema(name, symbols);
        }

        private EnumSchema(Name name, List<string> symbols)
            : base(Type.ENUM, name)
        {
            this.symbols = symbols;
        }

        protected override void WriteProperties(Newtonsoft.Json.JsonTextWriter writer)
        {
            base.WriteProperties(writer);
            writer.WritePropertyName("symbols");

            writer.WriteStartArray();

            foreach (string s in this.symbols)
            {
                writer.WriteValue(s);
            }

            writer.WriteEndArray();
        }
    }
}
