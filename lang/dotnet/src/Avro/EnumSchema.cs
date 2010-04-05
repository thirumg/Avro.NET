using System;
using System.Collections.Generic;
using System.Text;

namespace Avro
{
    public class EnumSchema:NamedSchema
    {
        public string[] Symbols { get; set; }

        public EnumSchema(string name, string snamespace, string[] symbols, Names names)
            : base("enum", name, snamespace, names)
        {
            if (null == symbols || symbols.Length == 0) throw new ArgumentNullException("symbols", "symbols cannot be null or empty.");

            this.Symbols = symbols;
        }

        protected override void WriteProperties(Newtonsoft.Json.JsonTextWriter writer)
        {
            base.WriteProperties(writer);
            writer.WritePropertyName("symbols");

            writer.WriteStartArray();

            foreach (string s in this.Symbols)
            {
                writer.WriteValue(s);
            }


            writer.WriteEndArray();
        }
    }
}
