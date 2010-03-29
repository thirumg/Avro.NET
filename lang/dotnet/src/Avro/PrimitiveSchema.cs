using System;
using System.Collections.Generic;
using System.Text;

namespace Avro
{
    public class PrimitiveSchema:Schema
    {
        public static readonly SchemaType[] SupportedTypes;
        public static readonly IDictionary<SchemaType, string> PrimitiveValueLookup;
        public static readonly IDictionary<string, SchemaType> PrimitiveKeyLookup;
        static PrimitiveSchema()
        {
            SupportedTypes = EnumHelper<SchemaType>.CreateArray(SchemaType.Null,
SchemaType.Boolean,
SchemaType.String,
SchemaType.Bytes,
SchemaType.Int,
SchemaType.Long,
SchemaType.Float,
SchemaType.Double);

            PrimitiveValueLookup = EnumHelper<SchemaType>.CreateValueLookup(SupportedTypes);
            PrimitiveKeyLookup = EnumHelper<SchemaType>.CreateKeyLookup(SupportedTypes);
        }

        public PrimitiveSchema(SchemaType type)
            : base(type)
        {
            if (!PrimitiveValueLookup.ContainsKey(type))
            {
                throw new NotSupportedException("Type \"" + type + "\" is not a primitive.");
            }
        }
    }
}
