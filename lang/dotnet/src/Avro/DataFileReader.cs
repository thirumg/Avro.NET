using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.IO.Compression;

namespace Avro
{
    public class DataFileReader<T>
    {
        

        private Stream _Reader;
        private BinaryDecoder _Decoder;
        private Dictionary<string, byte[]> _Metadata = new Dictionary<string, byte[]>();

        public DataFileReader(Stream input, DatumReader<T> reader)
        {
            if (null == input) throw new ArgumentNullException("input", "input cannot be null.");
            this._Reader = input;
            init(input);

        }

        private void init(Stream input)
        {
            //TODO: Should this be a buffered stream?
            this._Decoder = new BinaryDecoder(input);
            byte[] magic = new byte[DataFileConstants.MAGIC.Length];
            try
            {
                _Decoder.ReadFixed(magic);
            }
            catch (IOException)
            {
                throw new IOException("Not a data file.");
            }
            
            if(!ArrayHelper<byte>.Equals(magic, DataFileConstants.MAGIC))
                throw new IOException("Not a data file.");

            long l = _Decoder.ReadMapStart();

            if (l > 0)
            {
                for (long i = 0; i < l; i++)
                {
                    string key = _Decoder.ReadString();
                    byte[] buffer = _Decoder.ReadBytes();
                    _Metadata.Add(key, buffer);
                }
            }

        }
    }
}
