using System;
using System.Collections.Generic;
using System.Text;

namespace Avro
{
    //TODO: This is not properly generating a schema
    public class MapSchema:Schema
    {
        public Schema ValueSchema { get; private set; }
        public MapSchema(Schema valueSchema)
            :base("map")
        {
            if (null == valueSchema) throw new ArgumentNullException("valueSchema", "valueSchema cannot be null.");
            this.ValueSchema = valueSchema;
        }
    }
}
