using System;
using System.Collections.Generic;
using System.Text;

namespace Avro
{
    public class ProtocolParseException:AvroException
    {
        public ProtocolParseException(string s)
            : base(s)
        {

        }

        public ProtocolParseException(string s, Exception inner)
            : base(s, inner)
        {

        }

    }
}
