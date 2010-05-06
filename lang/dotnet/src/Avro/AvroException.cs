using System;
using System.Collections.Generic;
using System.Text;

namespace Avro
{
    public class AvroException:Exception
    {
        public AvroException(string s)
            : base(s)
        {

        }

        public AvroException(string s, Exception inner)
            : base(s, inner)
        {

        }
    }
}
