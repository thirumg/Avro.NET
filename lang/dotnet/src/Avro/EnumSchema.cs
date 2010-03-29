using System;
using System.Collections.Generic;
using System.Text;

namespace Avro
{
    class EnumSchema:NamedSchema
    {
        public string[] Symbols { get; set; }

        public EnumSchema(string name, string snamespace, string[] symbols, Names names)
            : base(name, snamespace, names)
        {
            if (null == symbols || symbols.Length == 0) throw new ArgumentNullException("symbols", "symbols cannot be null or empty.");

            this.Symbols = symbols;
        }
    }
}
