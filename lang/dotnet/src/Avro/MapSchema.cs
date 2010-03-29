using System;
using System.Collections.Generic;
using System.Text;

namespace Avro
{
    class MapSchema:Schema
    {
        public Schema Values { get; private set; }
        public MapSchema(Schema values)
            :base(SchemaType.Map)
        {
            if (null == values) throw new ArgumentNullException("values", "values cannot be null.");
            this.Values = values;
        }
    }
}
