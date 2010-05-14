using System;
using System.Collections.Generic;
using System.Text;

namespace Avro.Test
{
    class RandomDataHelper
    {
        static Random random = new Random();

        public static string GetString(int min, int max)
        {
            System.Text.StringBuilder builder = new System.Text.StringBuilder();

            int length = random.Next(min, max);

            for (int j = 0; j < length; j++)
            {
                builder.Append((char)random.Next(1, 255));
            }

            return builder.ToString();
        }

        public static bool GetBool()
        {
            return random.Next() % 2 == 1;
        }

        public static int GetRandomInt32()
        {
            bool IsNegative = GetBool();
            int value = random.Next();
            int mult = IsNegative ? -1 : 1;

            return value * mult;
        }

        public static long GetRandomInt64()
        {
            return (long)GetRandomInt32();
        }

        public static float GetRandomFloat()
        {
            return (float)random.NextDouble();
        }
    }
}
