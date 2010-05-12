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
    }
}
