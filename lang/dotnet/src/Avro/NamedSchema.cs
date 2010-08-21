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
    public abstract class NamedSchema : Schema
    {
        public Name name { get; private set; }

        public static NamedSchema NewInstance(JToken j, Names names)
        {
            string type = JsonHelper.GetRequiredString(j, "type");

            NamedSchema result;
            if (names.TryGetValue(type, out result))
            {
                // FIXME: If the JSON object has anything more than "type" throw.
                return result;
            }
            /*
            string doc = JsonHelper.getOptionalString(j, "doc");
             */

            switch (type)
            {
                case "fixed":
                    return FixedSchema.NewInstance(j);
                case "enum":
                    return EnumSchema.NewInstance(j);
                case "record":
                    return RecordSchema.NewInstance(Type.RECORD, j, names);
                case "error":
                    return RecordSchema.NewInstance(Type.ERROR, j, names);
                default:
                    return null;
            }
        }

        protected static Name GetName(JToken j)
        {
            String n = JsonHelper.GetRequiredString(j, "name");
            String ns = JsonHelper.GetOptionalString(j, "namespace");
            return new Name(n, ns);
        }

        protected NamedSchema(Type type, Name name)
            : base(type)
        {
            this.name = name;
        }

        protected override void WriteProperties(Newtonsoft.Json.JsonTextWriter writer)
        {
            base.WriteProperties(writer);
            this.name.WriteJson(writer);

            //JsonHelper.writeIfNotNullOrEmpty(writer, "name", this.Name);
            JsonHelper.writeIfNotNullOrEmpty(writer, "namespace", this.name.space);
        }

        public override string GetName()
        {
            return name.full;
        }
    }
}
