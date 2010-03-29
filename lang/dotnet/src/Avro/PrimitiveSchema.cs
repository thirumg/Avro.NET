using System;
using System.Collections.Generic;
using System.Text;

namespace Avro
{
    public class PrimitiveSchema:Schema
    {
        public static readonly string[] SupportedTypes;
        //public static readonly IDictionary<SchemaType, string> PrimitiveValueLookup;
        public static readonly IDictionary<string, string> PrimitiveKeyLookup;
        static PrimitiveSchema()
        {
            SupportedTypes = new string[]{
                  "null",
  "boolean",
  "string",
  "bytes",
  "int",
  "long",
  "float",
  "double"
            };

            Dictionary<string, string> keylookup = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (string key in SupportedTypes)
            {
                keylookup.Add(key, key);
            }
            PrimitiveKeyLookup = keylookup;



            ////PrimitiveValueLookup = EnumHelper<SchemaType>.CreateValueLookup(SupportedTypes);
            //PrimitiveKeyLookup = EnumHelper<SchemaType>.CreateKeyLookup(SupportedTypes);
        }

        public PrimitiveSchema(string type)
            : base(type)
        {
            if (!PrimitiveKeyLookup.ContainsKey(type))
            {
                throw new NotSupportedException("Type \"" + type + "\" is not a primitive.");
            }
        }
    }
}
