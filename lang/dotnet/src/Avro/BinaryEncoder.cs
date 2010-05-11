using System;
using System.Collections.Generic;
using System.IO;
using Avro.IO;


namespace Avro
{
    /// <summary>
    /// Write leaf values.
    /// </summary>
    public class BinaryEncoder : Encoder
    {
        /// <summary>
        /// Write leaf values.
        /// </summary>

        public BinaryEncoder()
        {

        }

        private void write(Stream Stream, params byte[] bytes)
        {
            Stream.Write(bytes, 0, bytes.Length);
        }

        /// <summary>
        /// null is written as zero bytes
        /// </summary>

        public void WriteNull(Stream Stream)
        {

        }
        /// <summary>
        /// a boolean is written as a single byte whose value is either 0 (false) or 1 (true).
        /// </summary>
        /// <param name="datum"></param>
        public void WriteBoolean(Stream Stream, bool datum)
        {
            byte value = (byte)(datum ? 1 : 0);
            write(Stream, value);
        }

        /// <summary>
        /// int and long values are written using variable-length, zig-zag coding.
        /// </summary>
        /// <param name="datum"></param>
        public void WriteInt(Stream Stream, int datum)
        {
            WriteLong(Stream, datum);
        }
        /// <summary>
        /// int and long values are written using variable-length, zig-zag coding.
        /// </summary>
        /// <param name="datum"></param>
        public void WriteLong(Stream Stream, long datum)
        {
            ulong n = Util.Zig(datum);// move sign to low-order bit
            while ((n & ~0x7FUL) != 0)
            {
                write(Stream, (byte)((n & 0x7f) | 0x80));
                n >>= 7;
            }
            write(Stream, (byte)n);
        }


        /// <summary>
        /// A float is written as 4 bytes.
        /// The float is converted into a 32-bit integer using a method equivalent to
        /// Java's floatToIntBits and then encoded in little-endian format.
        /// </summary>
        /// <param name="datum"></param>
        public void WriteFloat(Stream Stream, float datum)
        {
            byte[] buffer = null;
            buffer = BitConverter.GetBytes(datum);
            if (!BitConverter.IsLittleEndian)
            {
                Array.Reverse(buffer);
            }

            write(Stream, buffer);

            //int bits = floatToRawIntBits(datum);
            //write(BitConverter.GetBytes(datum));
            //write((byte)((bits) & 0xFF));
            //write((byte)((bits >> 8) & 0xFF));
            //write((byte)((bits >> 16) & 0xFF));
            //write((byte)((bits >> 24) & 0xFF));
        }
        /// <summary>
        ///A double is written as 8 bytes.
        ///The double is converted into a 64-bit integer using a method equivalent to
        ///Java's doubleToLongBits and then encoded in little-endian format.
        /// </summary>
        /// <param name="datum"></param>
        public void WriteDouble(Stream Stream, double datum)
        {
            long bits = BitConverter.DoubleToInt64Bits(datum);

            write(Stream, (byte)((bits) & 0xFF));
            write(Stream, (byte)((bits >> 8) & 0xFF));
            write(Stream, (byte)((bits >> 16) & 0xFF));
            write(Stream, (byte)((bits >> 24) & 0xFF));
            write(Stream, (byte)((bits >> 32) & 0xFF));
            write(Stream, (byte)((bits >> 40) & 0xFF));
            write(Stream, (byte)((bits >> 48) & 0xFF));
            write(Stream, (byte)((bits >> 56) & 0xFF));
        }
        /// <summary>
        /// Bytes are encoded as a long followed by that many bytes of data.
        /// </summary>
        /// <param name="value"></param>
        /// 
        public void WriteBytes(Stream Stream, byte[] value)
        {
            WriteLong(Stream, value.Length);
            write(Stream, value);
        }
        /// <summary>
        /// A string is encoded as a long followed by
        /// that many bytes of UTF-8 encoded character data.
        /// </summary>
        /// <param name="value"></param>
        public void WriteString(Stream Stream, string value)
        {
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(value);
            WriteBytes(Stream, buffer);
        }


        public static int floatToRawIntBits(float f)
        {
            byte[] buffer = BitConverter.GetBytes(f);
            Array.Reverse(buffer);
            int value = BitConverter.ToInt32(buffer, 0);
            return value; // System.Net.IPAddress.NetworkToHostOrder(value);

            //BitMem bitMem = new BitMem();
            //bitMem.f = f;
            //return bitMem.i;
        }

        public void StartItem(Stream Stream)
        {

        }

        public void WriteMapStart(Stream Stream)
        {

        }

        public void SetItemCount(Stream Stream, int value)
        {
            if (value > 0)
            {
                WriteLong(Stream, value);
            }
        }

        public void WriteMapEnd(Stream Stream)
        {
            WriteLong(Stream, 0);
        }
    }
}
