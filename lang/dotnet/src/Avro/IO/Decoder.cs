using System;
using System.IO;

namespace Avro.IO
{
    public interface Decoder
    {
        bool ReadBool(Stream Stream);
        byte[] ReadBytes(Stream Stream);
        double ReadDouble(Stream Stream);
        void ReadFixed(Stream Stream, byte[] buffer);
        float ReadFloat(Stream Stream);
        int ReadInt(Stream Stream);
        long ReadLong(Stream Stream);
        long ReadMapStart(Stream Stream);
        object ReadNull();
        string ReadString(Stream Stream);
        void SkipBoolean(Stream Stream);
        void SkipBytes(Stream Stream);
        void SkipDouble(Stream Stream);
        void SkipFloat(Stream Stream);
        void SkipInt(Stream Stream);
        void SkipLong(Stream Stream);
        void SkipNull(Stream Stream);
        void SkipUTF8(Stream Stream);
    }


}
