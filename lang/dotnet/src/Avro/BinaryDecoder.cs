using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Avro
{
    /// <summary>
    /// Read leaf values.
    /// </summary>
    public class BinaryDecoder
    {
        public Stream Stream { get; private set; }
        public BinaryDecoder(Stream s)
        {
            if (null == s) throw new ArgumentNullException("s", "s cannot be null.");
            this.Stream = s;
        }



        //def read(self, n):
        //  """
        //  Read n bytes.
        //  """
        //  return self.reader.read(n)

        private byte[] read(long p)
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
        public bool ReadBool()
        {
            return ord(read(1)) == 1;
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
        public int ReadInt()
        {
            return (int)ReadLong();
        }
        /// <summary>
        /// int and long values are written using variable-length, zig-zag coding.
        /// </summary>
        /// <param name="?"></param>
        /// <returns></returns>
        public long ReadLong()
        {
            byte b = (byte)this.Stream.ReadByte();
            int n = b & 0x7F;
            int shift = 7;
            while ((b & 0x80) != 0)
            {
                b = (byte)this.Stream.ReadByte();
                n |= (b & 0x7F) << shift;
                shift += 7;
            }
            long datum =  (n >> 1) ^ -(n & 1);
            return datum;
        }
        /// <summary>
        /// A float is written as 4 bytes.
        /// The float is converted into a 32-bit integer using a method equivalent to
        /// Java's floatToIntBits and then encoded in little-endian format.
        /// </summary>
        /// <returns></returns>
        public float ReadFloat()
        {
            long bits = (this.Stream.ReadByte() & 0xffL |
            (this.Stream.ReadByte()) & 0xffL << 8 |
            (this.Stream.ReadByte()) & 0xffL << 16 |
            (this.Stream.ReadByte()) & 0xffL << 24);

            

            throw new NotImplementedException();

        }

        /// <summary>
        /// A double is written as 8 bytes.
        /// The double is converted into a 64-bit integer using a method equivalent to
        /// Java's doubleToLongBits and then encoded in little-endian format.
        /// </summary>
        /// <param name="?"></param>
        /// <returns></returns>
        public double ReadDouble()
        {
            long bits = (this.Stream.ReadByte() & 0xffL) |
              (this.Stream.ReadByte() & 0xffL) << 8 |
              (this.Stream.ReadByte() & 0xffL) << 16 |
              (this.Stream.ReadByte() & 0xffL) << 24 |
              (this.Stream.ReadByte() & 0xffL) << 32 |
              (this.Stream.ReadByte() & 0xffL) << 40 |
              (this.Stream.ReadByte() & 0xffL) << 48 |
              (this.Stream.ReadByte() & 0xffL) << 56;

            return BitConverter.Int64BitsToDouble(bits);
        }

        /// <summary>
        /// Bytes are encoded as a long followed by that many bytes of data. 
        /// </summary>
        /// <returns></returns>
        public byte[] ReadBytes()
        {
            return read(ReadLong());
        }

        /// <summary>
        ///     A string is encoded as a long followed by
        ///  that many bytes of UTF-8 encoded character data.
        /// </summary>
        /// <param name="?"></param>
        /// <returns></returns>
        public string ReadUTF8()
        {
            return Encoding.UTF8.GetString(ReadBytes());
        }
        public void SkipNull()
        {

        }
        public void SkipBoolean()
        {
            Skip(1);
        }

        private void Skip(int p)
        {
            this.Stream.Seek(p, SeekOrigin.Current);
        }

        private void Skip(long p)
        {
            this.Stream.Seek(p, SeekOrigin.Current);
        }

        public void SkipInt()
        {
            SkipLong();
        }

        public void SkipLong()
        {
            int b = ord(read(1));
            while ((b & 0x80) != 0)
            {
                b = ord(read(1));
            }
        }

        public void SkipFloat()
        {
            Skip(4);
        }

        public void SkipDouble()
        {
            Skip(8);
        }
        public void SkipBytes()
        {
            Skip(ReadLong());
        }

 

        public void SkipUTF8()
        {
            SkipBytes();
        }


        internal void skip(long block_size)
        {
            throw new NotImplementedException();
        }
    }
}
