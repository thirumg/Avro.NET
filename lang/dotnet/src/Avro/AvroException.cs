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
    }
}
