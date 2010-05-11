using System;
using System.Collections.Generic;
using System.IO;
using Avro.IO;

namespace Avro
{
    /// <summary>
    /// Read leaf values.
    /// </summary>
    public class BinaryDecoder:Decoder
    {
        //def read(self, n):
        //  """
        //  Read n bytes.
        //  """
        //  return self.reader.read(n)

        private byte[] read(Stream Stream, long p)
        {
            byte[] buffer = new byte[p];

            Stream.Read(buffer, 0, (int)p);

            //TODO: This sucks fix it.

            return buffer;
        }

        /// <summary>
        /// null is written as zero bytes
        /// </summary>
        public object ReadNull()
        {
            return null;
        }
        /// <summary>
        /// a boolean is written as a single byte 
        /// whose value is either 0 (false) or 1 (true).
        /// </summary>
        /// <returns></returns>
        public bool ReadBool(Stream Stream)
        {
            return read(Stream) == 1;
        }

        private byte ord(byte[] p)
        {
            return p[0];
        }
        /// <summary>
        /// int and long values are written using variable-length, zig-zag coding.
        /// </summary>
        /// <param name="?"></param>
        /// <returns></returns>
        public int ReadInt(Stream Stream)
        {
            return (int)ReadLong(Stream);
        }
        /// <summary>
        /// int and long values are written using variable-length, zig-zag coding.
        /// </summary>
        /// <param name="?"></param>
        /// <returns></returns>
        public long ReadLong(Stream Stream)
        {
            byte b = read(Stream);
            ulong n = b & 0x7FUL;
            int shift = 7;
            while ((b & 0x80) != 0)
            {
                b = read(Stream);
                n |= (b & 0x7FUL) << shift;
                shift += 7;
            }
            return Util.Zag(n);

        }

        private byte read(Stream Stream)
        {
            return (byte)Stream.ReadByte();
        }
        

        /// <summary>
        /// A float is written as 4 bytes.
        /// The float is converted into a 32-bit integer using a method equivalent to
        /// Java's floatToIntBits and then encoded in little-endian format.
        /// </summary>
        /// <returns></returns>
        public float ReadFloat(Stream Stream)
        {
            byte[] buffer = read(Stream, 4);

            if (!BitConverter.IsLittleEndian)
                Array.Reverse(buffer);

            return BitConverter.ToSingle(buffer, 0);
            
            //int bits = (Stream.ReadByte() & 0xff |
            //(Stream.ReadByte()) & 0xff << 8 |
            //(Stream.ReadByte()) & 0xff << 16 |
            //(Stream.ReadByte()) & 0xff << 24);
            //return intBitsToFloat(bits);
        }

        public static float intBitsToFloat(int value)
        {
            return BitConverter.ToSingle(BitConverter.GetBytes(value), 0);
        }

        /// <summary>
        /// A double is written as 8 bytes.
        /// The double is converted into a 64-bit integer using a method equivalent to
        /// Java's doubleToLongBits and then encoded in little-endian format.
        /// </summary>
        /// <param name="?"></param>
        /// <returns></returns>
        public double ReadDouble(Stream Stream)
        {
            long bits = (Stream.ReadByte() & 0xffL) |
              (Stream.ReadByte() & 0xffL) << 8 |
              (Stream.ReadByte() & 0xffL) << 16 |
              (Stream.ReadByte() & 0xffL) << 24 |
              (Stream.ReadByte() & 0xffL) << 32 |
              (Stream.ReadByte() & 0xffL) << 40 |
              (Stream.ReadByte() & 0xffL) << 48 |
              (Stream.ReadByte() & 0xffL) << 56;

            return BitConverter.Int64BitsToDouble(bits);
        }

        /// <summary>
        /// Bytes are encoded as a long followed by that many bytes of data. 
        /// </summary>
        /// <returns></returns>
        public byte[] ReadBytes(Stream Stream)
        {
            return read(Stream, ReadLong(Stream));
        }


        public void SkipNull(Stream Stream)
        {

        }
        public void SkipBoolean(Stream Stream)
        {
            Skip(Stream, 1);
        }

        private void Skip(Stream Stream, int p)
        {
            Stream.Seek(p, SeekOrigin.Current);
        }

        private void Skip(Stream Stream, long p)
        {
            Stream.Seek(p, SeekOrigin.Current);
        }

        public void SkipInt(Stream Stream)
        {
            SkipLong(Stream);
        }

        public void SkipLong(Stream Stream)
        {
            int b = ord(read(Stream,1));
            while ((b & 0x80) != 0)
            {
                b = ord(read(Stream,1));
            }
        }

        public void SkipFloat(Stream Stream)
        {
            Skip(Stream, 4);
        }

        public void SkipDouble(Stream Stream)
        {
            Skip(Stream, 8);
        }
        public void SkipBytes(Stream Stream)
        {
            Skip(Stream, ReadLong(Stream));
        }

 

        public void SkipUTF8(Stream Stream)
        {
            SkipBytes(Stream);
        }


        internal void skip(long block_size)
        {
            throw new NotImplementedException();
        }

        public void ReadFixed(Stream Stream, byte[] buffer)
        {
            ReadFixed(Stream, buffer, 0, buffer.Length);
        }

        private void ReadFixed(Stream Stream, byte[] buffer, int start, int length)
        {
            //TODO: Look at this it's lame
            Stream.Read(buffer, start, length);
        }

        protected long doReadItemCount(Stream Stream)
        {
            long result = ReadLong(Stream);
            if (result < 0)
            {
                ReadLong(Stream); // Consume byte-count if present
                result = -result;
            }
            return result;
        }

        public long ReadMapStart(Stream Stream)
        {
            return ReadLong(Stream);
        }

        public string ReadString(Stream Stream)
        {
            int length = ReadInt(Stream);
            byte[] buffer = new byte[length];
            //TODO: Fix this because it's lame;
            ReadFixed(Stream, buffer);
            return System.Text.Encoding.UTF8.GetString(buffer);
        }
    }
}
