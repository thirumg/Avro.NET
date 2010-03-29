using System;
using System.Collections.Generic;
using System.Text;

namespace Avro
{
    public class SchemaParseException:AvroException
    {
        public SchemaParseException(string s)
            : base(s)
        {

        }
    }
}
