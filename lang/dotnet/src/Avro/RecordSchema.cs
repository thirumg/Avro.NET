using System;
using System.Collections.Generic;
using System.Text;

namespace Avro
{
    class RecordSchema:NamedSchema
    {
        public RecordSchema(string name, string snamespace, IEnumerable<Field> fields, Names names):base(name, snamespace, names)
        {

        }

        
    }
}
