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
    public class Name : IEquatable<Name>
    {
        private static readonly Logger log = new Logger();

        public String name { get; private set; }
        public String space { get; private set; }
        public String full { get; private set; }


        public Name(String name, String space)
        {
            if (name == null)
            {                         // anonymous
                this.name = this.space = this.full = null;
                return;
            }

            if (!name.Contains("."))
            {                          // unqualified name
                this.space = space;                       // use default space
                this.name = name;
            }
            else
            {
                string[] parts = name.Split('.');

                this.space = string.Join(".", parts, 0, parts.Length - 1);
                this.name = parts[parts.Length - 1];
            }
            this.full = string.IsNullOrEmpty(this.space) ? this.name : this.space + "." + this.name;
        }

        public override int GetHashCode()
        {
            return string.IsNullOrEmpty(full) ? 0 : full.GetHashCode();
        }
        public override string ToString()
        {
            return full;
        }

        //public void writeName(Names names, JsonGenerator gen) throws IOException {
        //  if (name != null) gen.writeStringField("name", name);
        //  if (space != null) {
        //    if (!space.equals(names.space()))
        //      gen.writeStringField("namespace", space);
        //    if (names.space() == null)                // default namespace
        //      names.space(space);
        //  }
        //}


        public static Name make_fullname(string name, string snamespace)
        {
            if (log.IsDebugEnabled) log.DebugFormat("make_fullname(string, string) - name=\"{0}\", snamespace=\"{1}\"", name, snamespace);
            Name n = new Name(name, snamespace);
            if (log.IsDebugEnabled) log.DebugFormat("make_fullname(string, string) - name= \"{0}\"", n);

            return n;
        }

        internal void WriteJson(Newtonsoft.Json.JsonTextWriter writer)
        {
            JsonHelper.writeIfNotNullOrEmpty(writer, "name", this.name);
            JsonHelper.writeIfNotNullOrEmpty(writer, "namespace", this.space);
        }

        public override bool Equals(Object o)
        {
            if (o == this) return true;
            if (!(o is Name)) return false;
            Name that = (Name)o;
            return Equals(that);

        }

        public bool Equals(Name that)
        {
            if (null == that)
                return false;

            return full == null ? that.full == null : full.Equals(that.full);
        }
    }
}
