using System;
using System.Collections.Generic;
using System.Text;

namespace Avro
{
    public class ArraySchema:Schema
    {
        public Schema Items { get; set; }

        public ArraySchema(Schema items)
            : base("array")
        {
            if (null == items) throw new ArgumentNullException("items", "items cannot be null.");
            this.Items = items;
        }

        protected override void WriteProperties(Newtonsoft.Json.JsonTextWriter writer)
        {
            if (null != this.Items)
            {
                writer.WritePropertyName("items");
                this.Items.writeJson(writer);
            }
        }
    }
}
