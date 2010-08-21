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
    public class Names : Dictionary<Name, NamedSchema>
    {
        public string Space { get; private set; }

        public Names()
            : this(string.Empty)
        {

        }
        public Names(string space)
        {
            this.Space = space;
        }

        public NamedSchema this[string s]
        {
            get
            {
                Name name = new Name(s, this.Space);
                return this[name];
            }
        }

        public bool TryGetValue(string s, out NamedSchema schema)
        {
            Name name = new Name(s, this.Space);

            return TryGetValue(name, out schema);
        }

        public new NamedSchema this[Name name]
        {
            set
            {
                if (base.ContainsKey(name))
                    throw new SchemaParseException("Can't redefine: " + name);

                base.Add(name, value);
            }
            get
            {
                return base[name];
            }
        }


        public new void Add(Name name, NamedSchema schema)
        {
            if (null == name) throw new ArgumentNullException("name", "name cannot be null.");
            if (base.ContainsKey(name))
                throw new SchemaParseException("Can't redefine: " + name);

            base.Add(name, schema);
        }

        public bool Contains(Schema schema)
        {
            if (!(schema is NamedSchema))
                return false;

            return ContainsKey(((NamedSchema)schema).name);
        }

        public void Add(NamedSchema schema)
        {
            if (null == schema) throw new ArgumentNullException("schema", "schema cannot be null.");
            Add(schema.name, schema);
        }
    }
}
