using System;
using System.Collections.Generic;
using System.Text;

namespace Avro
{
    class ArraySchema:Schema
    {
        public Schema Items { get; set; }

        public ArraySchema(Schema items)
            : base("array")
        {
            if (null == items) throw new ArgumentNullException("items", "items cannot be null.");
            this.Items = items;
        }
    }
}
