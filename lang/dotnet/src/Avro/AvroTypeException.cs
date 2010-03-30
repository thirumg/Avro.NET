using System;
using System.Collections.Generic;
using System.Text;

namespace Avro
{
    class AvroTypeException:AvroException
    {
        public AvroTypeException(string s)
            : base(s)
        {

        }
    }
}
