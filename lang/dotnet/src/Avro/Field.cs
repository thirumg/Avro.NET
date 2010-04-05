using System;
using System.Collections.Generic;
using System.Text;

namespace Avro
{
    public enum SortOrder
    {
        Ascending,
        Descending,
        Ignore
    }

    public class Field
    {
        public string Name { get; private set; }
        
        public string Doc { get; set; }
        public object Default { get; private set; }
        public bool HasDefault { get; private set; }
        public SortOrder? Order { get; private set; }
        public Schema Type { get; set; }


        public Field(Schema schema, string name, bool hasDefault)
            : this(schema, name, hasDefault, null, SortOrder.Ignore, null)
        {

        }

        public Field(Schema schema, string name, bool hasDefault, object oDefault, SortOrder sortorder, Names names)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name", "name cannot be null.");
            if (null == schema) throw new ArgumentNullException("schema", "schema cannot be null.");

            this.Type = schema;
            this.Name = name;
        }

        internal void writeJson(Newtonsoft.Json.JsonTextWriter writer)
        {
            writer.WriteStartObject();
            JsonHelper.writeIfNotNullOrEmpty(writer, "name", this.Name);
            JsonHelper.writeIfNotNullOrEmpty(writer, "doc", this.Doc);

            if (null != this.Type)
            {
                writer.WritePropertyName("type");
                this.Type.writeJson(writer);
            }

            writer.WriteEndObject();
        }
    }
}
