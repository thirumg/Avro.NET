using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Avro
{
    /// <summary>
    /// Write leaf values.
    /// </summary>
    public class BinaryEncoder
    {
        public Stream Stream { get; private set; }
        /// <summary>
        /// Write leaf values.
        /// </summary>
        /// <param name="s">writer is a Python object on which we can call write.</param>
        public BinaryEncoder(Stream s)
        {
            if (null == s) throw new ArgumentNullException("s", "s cannot be null.");
            this.Stream = s;
        }

        private void write(params byte[] bytes)
        {
            Stream.Write(bytes, 0, bytes.Length);
        }
        
        /// <summary>
        /// null is written as zero bytes
        /// </summary>
        public void write_null()
        {

        }
        /// <summary>
        /// a boolean is written as a single byte whose value is either 0 (false) or 1 (true).
        /// </summary>
        /// <param name="datum"></param>
        public void write_boolean(bool datum)
        {
            if (datum)
                write(1);
            else
                write(0);
        }

        /// <summary>
        /// int and long values are written using variable-length, zig-zag coding.
        /// </summary>
        /// <param name="datum"></param>
        public void write_int(int datum)
        {
            write_long(datum);
        }
        /// <summary>
        /// int and long values are written using variable-length, zig-zag coding.
        /// </summary>
        /// <param name="datum"></param>
        public void write_long(long datum)
        {
            ulong value = Util.Zig(datum);
            byte[] buffer = new byte[10];
            int ioIndex = 0;
            int count = 0;
            do
            {
                buffer[ioIndex++] = (byte)((value & 0x7F) | 0x80);
                count++;
            } while ((value >>= 7) != 0);
            buffer[ioIndex - 1] &= 0x7F;

            Array.Resize<byte>(ref buffer, count);
            write(buffer);
        }

 
        /// <summary>
        /// A float is written as 4 bytes.
        /// The float is converted into a 32-bit integer using a method equivalent to
        /// Java's floatToIntBits and then encoded in little-endian format.
        /// </summary>
        /// <param name="datum"></param>
        public void write_float(float datum)
        {

        }
        /// <summary>
        ///A double is written as 8 bytes.
        ///The double is converted into a 64-bit integer using a method equivalent to
        ///Java's doubleToLongBits and then encoded in little-endian format.
        /// </summary>
        /// <param name="datum"></param>
        public void write_double(double datum)
        {
            long bits = BitConverter.DoubleToInt64Bits(datum);

            write((byte)((bits) & 0xFF));
            write((byte)((bits >> 8) & 0xFF));
            write((byte)((bits >> 16) & 0xFF));
            write((byte)((bits >> 24) & 0xFF));
            write((byte)((bits >> 32) & 0xFF));
            write((byte)((bits >> 40) & 0xFF));
            write((byte)((bits >> 48) & 0xFF));
            write((byte)((bits >> 56) & 0xFF));
        }
        /// <summary>
        /// Bytes are encoded as a long followed by that many bytes of data.
        /// </summary>
        /// <param name="datum"></param>
        public void write_bytes(byte[] datum)
        {
            write_long(datum.Length);
            write(datum);
        }
        /// <summary>
        /// A string is encoded as a long followed by
        /// that many bytes of UTF-8 encoded character data.
        /// </summary>
        /// <param name="s"></param>
        public void write_utf8(string s)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(s);
            write_bytes(buffer);
        }
    }
}
