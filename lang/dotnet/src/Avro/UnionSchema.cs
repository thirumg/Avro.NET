using System;
using System.Collections.Generic;
using System.Text;

namespace Avro
{
    public class UnionSchema:Schema
    {
        public IList<Schema> Schemas { get; private set; }

        public UnionSchema(params Schema[] schemas)
            : base("union")
        {
            if (null == schemas || schemas.Length == 0) throw new ArgumentNullException("schemas", "schemas cannot be null or empty.");

            this.Schemas = new List<Schema>(schemas);

        }

        public UnionSchema(IEnumerable<Schema> schemas)
            : base("union")
        {
            if (null == schemas) throw new ArgumentNullException("schemas", "schemas cannot be null.");
            this.Schemas = new List<Schema>(schemas);
        }

        internal override void writeJson(Newtonsoft.Json.JsonTextWriter writer)
        {
            writer.WriteStartArray();

            foreach (Schema schema in this.Schemas)
            {
                schema.writeJson(writer);
            }

            writer.WriteEndArray();
        }
    }
}
