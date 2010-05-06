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
