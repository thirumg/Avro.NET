using System;
using System.Collections.Generic;
using System.Text;

namespace Avro
{
    public class EnumSchema:NamedSchema
    {
        public IList<string> Symbols { get; set; }

        public EnumSchema(Name name, IEnumerable<string> symbols, Names names)
            : base("enum", name, names)
        {
            if (null == symbols) throw new ArgumentNullException("symbols", "symbols cannot be null or empty.");

            this.Symbols = new List<string>(symbols);

            if (this.Symbols.Count == 0) throw new ArgumentNullException("symbols", "symbols cannot be null or empty.");

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
