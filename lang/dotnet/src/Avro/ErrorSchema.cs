using System;
using System.Collections.Generic;
using System.Text;

namespace Avro
{
    public class ErrorSchema:RecordSchema
    {
        public ErrorSchema(Name name, IEnumerable<Field> fields, Names names)
            : base(Schema.ERROR, name, fields, names)
        {

        }
    }
}
