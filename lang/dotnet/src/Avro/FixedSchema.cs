using System;
using System.Collections.Generic;
using System.Text;

namespace Avro
{
    public class FixedSchema:NamedSchema
    {
        public int Size { get; set; }

        public FixedSchema(Name name, int size)
            : base("fixed", name, null)
        {
            if (size <= 0) throw new ArgumentOutOfRangeException("size", "size must be greater than zero.");
            
            this.Size = size;
        }

        public FixedSchema(Name name, int size, Names names)
            : this(name, size)
        {

        }

        protected override void WriteProperties(Newtonsoft.Json.JsonTextWriter writer)
        {
            base.WriteProperties(writer);

            writer.WritePropertyName("size");
            writer.WriteValue(this.Size);
        }
    }
}
