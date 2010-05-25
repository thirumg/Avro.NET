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
    public class ArraySchema:Schema
    {
        public Schema ItemSchema { get; set; }

        public ArraySchema(Schema items)
            : base("array")
        {
            if (null == items) throw new ArgumentNullException("items", "items cannot be null.");
            this.ItemSchema = items;
        }

        protected override void WriteProperties(Newtonsoft.Json.JsonTextWriter writer)
        {
            if (null != this.ItemSchema)
            {
                writer.WritePropertyName("items");
                this.ItemSchema.writeJson(writer);
            }
        }
    }
}
