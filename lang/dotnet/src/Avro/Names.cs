using System;
using System.Collections.Generic;
using System.Text;

namespace Avro
{
    class Names:Dictionary<string, Name>
    {
        public Names()
            : base(StringComparer.Ordinal)
        {

        }
    }
}
