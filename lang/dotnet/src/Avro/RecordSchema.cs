using System;
using System.Collections.Generic;
using System.Text;

namespace Avro
{
    class RecordSchema:NamedSchema
    {
        public IList<Field> Fields { get; set; }

        public RecordSchema(string name, string snamespace, IEnumerable<Field> fields, Names names):base(name, snamespace, names)
        {
            if (null == fields) throw new ArgumentNullException("fields", "fields cannot be null.");
            this.Fields = new List<Field>(fields);
            
        }

        internal override void writeJson(Newtonsoft.Json.JsonTextWriter writer)
        {
            base.writeStartObject(writer);
            if (null != this.Fields && this.Fields.Count > 0)
            {
                writer.WritePropertyName("fields");
                writer.WriteStartArray();

                foreach (Field field in this.Fields)
                {
                    field.writeJson(writer);
                }

                writer.WriteEndArray();
            }


            writer.WriteEndObject();
        }
    }
}
