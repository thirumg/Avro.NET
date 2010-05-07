using System;
using System.Collections.Generic;
using System.Text;

namespace Avro
{
    public abstract class Encoder
    {
        public abstract void WriteMapStart();
        public abstract void SetItemCount(int value);
        public abstract void StartItem();
        public abstract void WriteString(string value);
        public abstract void WriteBytes(byte[] value);
        public abstract void WriteMapEnd();
        public abstract void Flush();
    }
}
