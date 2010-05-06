using System;
using System.Collections.Generic;
using System.Text;

namespace Avro
{
    public class NullSchema:Schema
    {
        public NullSchema()
            : base("null")
        {

        }
    }
}
