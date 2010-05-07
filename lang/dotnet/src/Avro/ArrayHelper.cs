using System;
using System.Collections.Generic;
using System.Text;

namespace Avro
{
    static class ArrayHelper<T>
    {
        public static bool Equals(T[] a, T[] b)
        {
            if (null == a && null == b)
                return true;
            if ((null == a && null != b) || (null == b && null != a))
                return false;
            if (a.Length != b.Length)
                return false;

            for (long i = 0; i < a.LongLength; i++)
                if (!object.Equals(a[i], b[i]))
                    return false;

            return true;
        }


    }
}
