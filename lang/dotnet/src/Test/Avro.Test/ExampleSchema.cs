using System;
using System.Collections.Generic;
using System.Text;

namespace Avro.Test
{
    class ExampleSchema
    {
        public string SchemaString { get; set; }
        public bool Valid { get; set; }
        public string Name { get; set; }


        public ExampleSchema() { }
        public ExampleSchema(string schemaString, bool valid)
        {
            this.SchemaString = schemaString;
            this.Valid = valid;
        }
    }
}
