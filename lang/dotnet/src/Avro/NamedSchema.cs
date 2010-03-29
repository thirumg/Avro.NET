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
                throw new NotImplementedException();
                //return Avro.Name.make_fullname(this.Namespace, this.Namespace);
            }
        }
        public NamedSchema(string name, string snamespace, Names names)
            : base("record")
        {
            if(null==name)throw new ArgumentNullException("name", "name cannot be null.");
            string full = Avro.Name.make_fullname(name, snamespace);

            if (PrimitiveSchema.PrimitiveKeyLookup.ContainsKey(full))
            {
                throw new SchemaParseException("Schemas may not be named after primitives: " + full);
            }

            this.Name = name;
            //Avro.Name a = Avro.Name.extract_namespace(name, snamespace);
        }


        public static readonly string[] SupportedTypes;
        public static readonly IDictionary<string, string> PrimitiveKeyLookup;
        

        static NamedSchema()
        {
            SupportedTypes = new string[]{
               "fixed",
              "enum",
              "record",
              "error"
            };

            Dictionary<string, string> keylookup = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (string key in SupportedTypes)
            {
                keylookup.Add(key, key);
            }
            PrimitiveKeyLookup = keylookup;
        }

        protected override void writeStartObject(Newtonsoft.Json.JsonTextWriter writer)
        {
            base.writeStartObject(writer);
            JsonHelper.writeIfNotNullOrEmpty(writer, "name", this.Name);
            JsonHelper.writeIfNotNullOrEmpty(writer, "namespace", this.Namespace);
            
        }
    }
}
