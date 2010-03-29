using System;
using System.Collections.Generic;
using System.Text;

namespace Avro
{
    class NamedSchema:Schema
    {
        public string Name { get; set; }
        public string Namespace { get; set; }

        public string FullName
        {
            get
            {
                return Avro.Name.make_fullname(this.Namespace, this.Namespace);
            }
        }
        public NamedSchema(string name, string snamespace, Names names):base(SchemaType.Record)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name", "name cannot be null.");

            
        }


        public static readonly SchemaType[] SupportedTypes;


        static NamedSchema()
        {
            SupportedTypes = EnumHelper<SchemaType>.CreateArray(SchemaType.Fixed,
                SchemaType.Enum,
                SchemaType.Record,
                SchemaType.Error);
        }
    }
}
