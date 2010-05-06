using System;
using System.Collections.Generic;
using System.Text;

namespace Avro
{
    public class AvroTypeException:AvroException
    {
        public AvroTypeException(string s)
            : base(s)
        {

        }
    }
}
