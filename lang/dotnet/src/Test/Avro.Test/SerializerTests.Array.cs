using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using Avro.IO;

namespace Avro.Test
{
    public partial class SerializerTests
    {
        [TestCase]
        public void Array_String()
        {
            Schema schema = new ArraySchema(new PrimitiveSchema(Schema.STRING));
            string[] expected = new string[ITERATIONS];
            for (int i = 0; i < expected.Length; i++)
                expected[i] = RandomDataHelper.GetString(50, 1000);

            using (MemoryStream iostr = new MemoryStream())
            {
                Serializer.Serialize(PrefixStyle.None, schema, iostr, BinaryEncoder.Instance, expected);
                Assert.Greater(iostr.Length, 0);
                iostr.Position = 0L;
                string[] actual = Serializer.Deserialize<string[]>(PrefixStyle.None, schema, iostr, BinaryDecoder.Instance);
                Assert.IsNotNull(actual);
                Assert.AreEqual(expected.LongLength, actual.LongLength);

            }


        }

        public static string ReadString(Stream stream, Decoder decoder)
        {
            return decoder.ReadString(stream);
        }

        public static string[] DeserializeArray(Stream stream, Decoder decoder)
        {
            long length = decoder.ReadLong(stream);
            string[] values = new string[length];

            for (long i = 0; i < length; i++)
            {
                values[i] = ReadString(stream, decoder);
            }

            return values;
        }


        public static void SerializeString(Stream iostr, Encoder encoder, string value)
        {
            encoder.WriteString(iostr, value);
        }

        public static void SerializeArray(Stream iostr, Encoder encoder, string[] value)
        {
            encoder.WriteArrayStart(iostr);
            encoder.WriteLong(iostr, value.LongLength);

            for (long i = 0; i < value.LongLength; i++)
                SerializeString(iostr, encoder, value[i]);

            encoder.WriteArrayEnd(iostr);
        }
        public static void SerializeArray(Stream iostr, Encoder encoder, IList<string> value)
        {
            encoder.WriteArrayStart(iostr);
            encoder.WriteLong(iostr, value.Count);

            for (int i = 0; i < value.Count; i++)
                encoder.WriteString(iostr, value[i]);

            encoder.WriteArrayEnd(iostr);
        }
    }
}
