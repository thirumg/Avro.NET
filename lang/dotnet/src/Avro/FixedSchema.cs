using System;
using System.Collections.Generic;
using System.Text;

namespace Avro
{
    class FixedSchema:NamedSchema
    {
        public int Size { get; set; }

        public FixedSchema(string name, string snamespace, int size):base(name, snamespace, null)
        {
            if (size <= 0) throw new ArgumentOutOfRangeException("size", "size must be greater than zero.");
            
            this.Size = size;
        }

        public FixedSchema(string name, string snamespace, int size, Names names)
            : this(name, snamespace, size)
        {

        }
    }
}
