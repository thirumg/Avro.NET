using System;
using System.Collections.Generic;
using System.Text;

namespace Avro
{
    class Util
    {
        private const long Int64Msb = ((long)1) << 63;
        private const int Int32Msb = ((int)1) << 31;

        public static bool checkIsValue(string type, params string[] types)
        {
            foreach (string t in types)
                if (t == type)
                    return true;

            return false;
        }

        public static uint Zig(int value)
        {
            return (uint)((value << 1) ^ (value >> 31));
        }
        public static ulong Zig(long value)
        {
            return (ulong)((value << 1) ^ (value >> 63));
        }

        public static int Zag(uint ziggedValue)
        {
            int value = (int)ziggedValue;
            return (-(value & 0x01)) ^ ((value >> 1) & ~Util.Int32Msb);
        }

        public static long Zag(ulong ziggedValue)
        {
            long value = (long)ziggedValue;
            return (-(value & 0x01L)) ^ ((value >> 1) & ~Util.Int64Msb);
        }
    }
}
