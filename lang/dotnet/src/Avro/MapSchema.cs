using System;
using System.Collections.Generic;
using System.Text;

namespace Avro
{
    /// <summary>
    /// 
    /// </summary>
    public class MapSchema:Schema
    {
        public Schema ValueSchema { get; private set; }
        public MapSchema(Schema valueSchema)
            :base("map")
        {
            if (null == valueSchema) throw new ArgumentNullException("valueSchema", "valueSchema cannot be null.");
            this.ValueSchema = valueSchema;
        }

        protected override void WriteProperties(Newtonsoft.Json.JsonTextWriter writer)
        {
            writer.WritePropertyName("values");
            ValueSchema.writeJson(writer);
        }
    }
}
