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
    class JsonHelper
    {
        static readonly Logger log = new Logger();
        public static string getOptionalString(JToken j, string property)
        {
            if (log.IsDebugEnabled) log.DebugFormat("getOptionalString(JToken, string) - property = {1}, j = {0}", j, property);
            if (null == j) throw new ArgumentNullException("j", "j cannot be null.");
            if (string.IsNullOrEmpty(property)) throw new ArgumentNullException("property", "property cannot be null.");

            JToken child = j[property];
            if (log.IsDebugEnabled) log.DebugFormat("getOptionalString(JToken, string) - child = {0}", child);
            if (null == child) return null;

            string value = child.ToString();
            return value.Trim('\"');
        }

        public static string getRequiredString(JToken j, string property)
        {
            string value = getOptionalString(j, property);
            if (string.IsNullOrEmpty(value))
                throw new SchemaParseException(string.Format("No \"{0}\" property: {1}", property, j));
            return value;
        }

        internal static void writeIfNotNullOrEmpty(Newtonsoft.Json.JsonTextWriter writer, string key, string value)
        {
            if (string.IsNullOrEmpty(value)) return;

            writer.WritePropertyName(key);
            writer.WriteValue(value);
        }
    }
}
