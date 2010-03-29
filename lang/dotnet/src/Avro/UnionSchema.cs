using System;
using System.Collections.Generic;
using System.Text;

namespace Avro
{
    class UnionSchema:Schema
    {
        public IList<Schema> Schemas { get; private set; }

        public UnionSchema(IEnumerable<Schema> schemas)
            : base(SchemaType.Union)
        {
            if (null == schemas) throw new ArgumentNullException("schemas", "schemas cannot be null.");
            this.Schemas = new List<Schema>(schemas);
        }
    }
}
