using System;
using System.Collections.Generic;
using System.Text;

namespace Avro
{
    public class BooleanSchema:PrimitiveSchema
    {
        public BooleanSchema()
            : base(Schema.BOOLEAN)
        {

        }
    }
}
