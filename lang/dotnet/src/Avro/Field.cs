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
        /// <summary>
        /// Field Name
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// Documentation
        /// </summary>
        public string Documentation { get; set; }
        public object Default { get; private set; }
        public bool HasDefault { get; private set; }
        public SortOrder? Order { get; private set; }
        public Schema Schema { get; set; }


        public Field(Schema schema, string name, bool hasDefault)
            : this(schema, name, hasDefault, null, SortOrder.Ignore, null)
        {

        }

        public Field(Schema type, string name, bool hasDefault, object oDefault, SortOrder sortorder, Names names)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name", "name cannot be null.");
            if (null == type) throw new ArgumentNullException("type", "type cannot be null.");

            this.Schema = type;
            this.Name = name;
        }

        internal void writeJson(Newtonsoft.Json.JsonTextWriter writer)
        {
            writer.WriteStartObject();
            JsonHelper.writeIfNotNullOrEmpty(writer, "name", this.Name);
            JsonHelper.writeIfNotNullOrEmpty(writer, "doc", this.Documentation);

            if (null != this.Schema)
            {
                writer.WritePropertyName("type");
                this.Schema.writeJson(writer);
            }

            writer.WriteEndObject();
        }
    }
}
