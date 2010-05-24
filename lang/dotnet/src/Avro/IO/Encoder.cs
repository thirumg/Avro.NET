using System;
using System.Collections.Generic;
using System.IO;

namespace Avro.IO
{
    public interface Encoder
    {
        void WriteArrayStart(Stream iostr);
        void WriteArrayEnd(Stream iostr);

        void WriteMapStart(Stream iostr);
        void SetItemCount(Stream iostr, int value);
        void StartItem(Stream iostr);
        void WriteString(Stream iostr, string value);
        void WriteBytes(Stream iostr, byte[] value);
        void WriteMapEnd(Stream iostr);
        void WriteInt(Stream iostr, int value);
        void WriteLong(Stream iostr, long value);
        void WriteFloat(Stream iostr, float value);
        void WriteDouble(Stream iostr, double value);
        void WriteBoolean(Stream iostr, bool value);
        void WriteNull(Stream iostr);
    }
}
