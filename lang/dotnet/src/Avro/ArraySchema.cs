using System;
using System.Collections.Generic;
using System.Text;

namespace Avro
{
    public class ArraySchema:Schema
    {
        public Schema ItemSchema { get; set; }

        public ArraySchema(Schema items)
            : base("array")
        {
            if (null == items) throw new ArgumentNullException("items", "items cannot be null.");
            this.ItemSchema = items;
        }

        protected override void WriteProperties(Newtonsoft.Json.JsonTextWriter writer)
        {
            if (null != this.ItemSchema)
            {
                writer.WritePropertyName("items");
                this.ItemSchema.writeJson(writer);
            }
        }
    }
}
