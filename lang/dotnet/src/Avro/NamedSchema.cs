using System;
using System.Collections.Generic;
using System.Text;

namespace Avro
{
    public abstract class NamedSchema:Schema
    {
        public Name Name { get; private set; }

        //public string Namespace
        //{
        //    get { return this.Name.space; }
        //}

        //public string FullName
        //{
        //    get
        //    {
        //        return this.Name.full;
        //    }
        //}
        public NamedSchema(string type, Name name, Names names)
            : base(type)
        {
            if(null==name)throw new ArgumentNullException("name", "name cannot be null.");
            

            if (PrimitiveSchema.PrimitiveKeyLookup.ContainsKey(name.full))
            {
                throw new SchemaParseException("Schemas may not be named after primitives: " + name.full);
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




        protected override void WriteProperties(Newtonsoft.Json.JsonTextWriter writer)
        {
            base.WriteProperties(writer);
            this.Name.WriteJson(writer);

            //JsonHelper.writeIfNotNullOrEmpty(writer, "name", this.Name);
            JsonHelper.writeIfNotNullOrEmpty(writer, "namespace", this.Name.space);
        }
    }
}
