using System;
using System.Collections.Generic;
using System.Text;

namespace Avro
{
    class SchemaResolutionException:AvroException
    {
        public SchemaResolutionException(string s, Schema readerSchema, Schema writerSchema)
            : base(s)
        {

        }
    }
}
