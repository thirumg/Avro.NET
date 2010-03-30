using System;
using System.Collections.Generic;
using System.Text;

namespace Avro
{
    class Util
    {
        public static bool checkIsValue(string type, params string[] types)
        {
            foreach (string t in types)
                if (t == type)
                    return true;

            return false;
        }
    }
}
