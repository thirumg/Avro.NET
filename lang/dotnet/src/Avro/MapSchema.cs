using System;
using System.Collections.Generic;
using System.Text;

namespace Avro
{
    public class MapSchema:Schema
    {
        public Schema Values { get; private set; }
        public MapSchema(Schema values)
            :base("map")
        {
            if (null == values) throw new ArgumentNullException("values", "values cannot be null.");
            this.Values = values;
        }
    }
}
