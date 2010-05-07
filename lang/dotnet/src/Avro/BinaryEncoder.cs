using System;
using System.Collections.Generic;
using System.IO;

namespace Avro
{
    /// <summary>
    /// Write leaf values.
    /// </summary>
    public class BinaryEncoder:Encoder
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
            WriteLong(datum);
        }
        /// <summary>
        /// int and long values are written using variable-length, zig-zag coding.
        /// </summary>
        /// <param name="datum"></param>
        public void WriteLong(long datum)
        {
            ulong n = Util.Zig(datum);// move sign to low-order bit
            while ((n & ~0x7FUL) != 0)
            {
                write((byte)((n & 0x7f) | 0x80));
                n >>= 7;
            }
            write((byte)n);
        }

 
        /// <summary>
        /// A float is written as 4 bytes.
        /// The float is converted into a 32-bit integer using a method equivalent to
        /// Java's floatToIntBits and then encoded in little-endian format.
        /// </summary>
        /// <param name="datum"></param>
        public void write_float(float datum)
        {
            byte[] buffer = null;
            buffer = BitConverter.GetBytes(datum);
            if (!BitConverter.IsLittleEndian)
            {
                Array.Reverse(buffer);
            }

            write(buffer);

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
        /// <param name="value"></param>
        /// 
        public override void WriteBytes(byte[] value)
        {
            WriteLong(value.Length);
            write(value);
        }
        /// <summary>
        /// A string is encoded as a long followed by
        /// that many bytes of UTF-8 encoded character data.
        /// </summary>
        /// <param name="value"></param>
        public override void WriteString(string value)
        {
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(value);
            WriteBytes(buffer);
        }

        public void flush()
        {
            this.Stream.Flush();
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

        public override void StartItem()
        {

        }

        public override void WriteMapStart()
        {
            
        }

        public override void SetItemCount(int value)
        {
            if (value > 0)
            {
                WriteLong(value);
            }
        }

        public override void WriteMapEnd()
        {
            WriteLong(0);
        }

        public override void Flush()
        {
            this.Stream.Flush();
        }
    }
}
