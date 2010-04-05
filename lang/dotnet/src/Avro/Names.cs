using System;
using System.Collections.Generic;
using System.Text;

namespace Avro
{
    public class Names:Dictionary<string, Name>
    {
        public Names()
            : base(StringComparer.Ordinal)
        {

        }
    }
}
