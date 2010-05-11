using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using Avro.IO;

namespace Avro
{
    public enum Codec : int
    {
        Null, 
        Deflate
    }

    class DataFileConstants
    {
        public static readonly byte VERSION = 1;
        public static readonly byte[] MAGIC = new byte[] { (byte)'O', (byte)'b', (byte)'j', VERSION };
        public static readonly long FOOTER_BLOCK = -1;
        public static readonly int SYNC_SIZE = 16;
        public static readonly int DEFAULT_SYNC_INTERVAL = 1000 * SYNC_SIZE;

        public static readonly string SCHEMA = "avro.schema";
        public static readonly string CODEC = "avro.codec";
        public static readonly string NULL_CODEC = "null";
        public static readonly string DEFLATE_CODEC = "deflate";
    }

    public class DataFileWriter<T>:IDisposable
    {
        private DatumWriter<T> _output;
        private Dictionary<string, byte[]> _metadata = new Dictionary<string, byte[]>();
        private Stream _iostr;
        private byte[] _Sync;
        private Encoder _Encoder;
        public Schema Schema { get; private set; }
        private int _SyncInterval = DataFileConstants.DEFAULT_SYNC_INTERVAL;

        public DataFileWriter(DatumWriter<T> output)
        {
            if (null == output) throw new ArgumentNullException("output", "output cannot be null.");
            this._output = output;
            this._metadata = new Dictionary<string, byte[]>();
        }

        public byte[] this[string key]
        {
            get
            {
                if (string.IsNullOrEmpty(key)) throw new ArgumentNullException("key", "key cannot be null.");
                byte[] buffer = null;

                if (_metadata.TryGetValue(key, out buffer))
                    return buffer;
                return null;
            }
            set
            {
                if (string.IsNullOrEmpty(key)) throw new ArgumentNullException("key", "key cannot be null.");
                if(isReserved(key))
                    throw new AvroException("Cannot set reserved meta key: " + key);
                setMetaInternal(key, value);
            }
        }

        public DataFileWriter<T> create(Schema schema, Stream outs)
        {
            if (null == schema) throw new ArgumentNullException("schema", "schema cannot be null.");
            if (null == outs) throw new ArgumentNullException("outs", "outs cannot be null.");
            this.Schema = schema;
            setMetaInternal(DataFileConstants.SCHEMA, schema.ToString());
            this._Sync = generateSync();
            init(outs);
            _iostr.Write(DataFileConstants.MAGIC, 0, DataFileConstants.MAGIC.Length);

            _Encoder.WriteMapStart(_iostr);
            _Encoder.SetItemCount(_iostr,_metadata.Count);

            foreach (KeyValuePair<string, byte[]> entry in _metadata)
            {
                _Encoder.StartItem(_iostr);
                _Encoder.WriteString(_iostr, entry.Key);
                _Encoder.WriteBytes(_iostr, entry.Value);
            }

            _Encoder.WriteMapEnd(_iostr);
            //_Encoder.Flush();
            writeSync();
            

            return this;
        }

        private long _LastSync = 0;

        private void writeSync()
        {
            _iostr.Write(this._Sync, 0, this._Sync.Length);
            _iostr.Flush();
            _LastSync = _iostr.Position;
        }

        private void init(Stream outs)
        {
            this._output.Schema = this.Schema;
            int bufferSize = Math.Min((int)(_SyncInterval * 1.25), int.MaxValue / 2 - 1);

            this._iostr = new BufferedStream(outs, bufferSize);
            //this._Encoder = new BinaryEncoder(_iostr);
        }

        private void setMetaInternal(string key, string value)
        {
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(value);
            setMetaInternal(key, buffer);
        }

        private void setMetaInternal(string key, byte[] value)
        {
            if (_metadata.ContainsKey(key))
                _metadata[key] = value;
            else
                _metadata.Add(key, value);
        }

        private static byte[] generateSync()
        {
            return Guid.NewGuid().ToByteArray();
        }

        private static bool isReserved(string key)
        {
            return key.StartsWith("avro.");
        }

        public void Dispose()
        {
            Flush();
            this._iostr.Dispose();
        }

        public void Append(T test)
        {
            _output.Write(test, _Encoder);
        }

        public void Flush()
        {
            this._iostr.Flush();
        }
    }







    //public class DataFileWriter
    //{
    //    private static readonly Logger log = new Logger();

    //    private Dictionary<string, string> _Metadata;

    //    public Stream Writer { get; private set; }
    //    public DatumWriter DatumWriter { get; private set; }
    //    public BinaryEncoder Encoder { get; private set; }
    //    public byte[] SyncMarker { get; private set; }
        


    //    private byte[] generate_sync_marker()
    //    {
    //        return Guid.NewGuid().ToByteArray();
    //    }

    //    public DataFileWriter(Stream iostr, DatumWriter datum_writer, Schema writers_schema)
    //        : this(iostr, datum_writer, writers_schema, Codec.Null)
    //    {

    //    }

    //    public DataFileWriter(Stream iostr, DatumWriter datum_writer, Schema writers_schema, Codec codec)
    //    {
    //        if (null == iostr) throw new ArgumentNullException("iostr", "iostr cannot be null.");
    //        this.Writer = iostr;
    //        this._Metadata = new Dictionary<string, string>();
    //        this.DatumWriter = datum_writer;
    //        this.Encoder = new BinaryEncoder(this.Writer);
    //        if (null != writers_schema)
    //        {
    //            this.SyncMarker = generate_sync_marker();
    //            this["avro.codec"] = codec.ToString().ToLower();
    //            this["avro.schema"] = writers_schema.ToString();
    //            this.DatumWriter.WriterSchema = writers_schema;
    //            writeHeader();
    //        }
    //        else
    //        {

    //        }

    //    }

    //    private void writeHeader()
    //    {
    //        Dictionary<string, object> values = new Dictionary<string,object>();
    //        values.Add("magic", DataFileHelper.MAGIC);
    //        values.Add("meta", _Metadata);
    //        values.Add("sync", this.SyncMarker);

    //        DatumWriter.write_data(DataFileHelper.META_SCHEMA, values, this.Encoder);
    //    }

    //    public string this[string key]
    //    {
    //        get
    //        {
    //            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException("key", "key cannot be null.");

    //            string value = null;
    //            if (_Metadata.TryGetValue(key, out value))
    //                return value;
    //            return null;
    //        }
    //        set
    //        {
    //            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException("key", "key cannot be null.");

    //            if(_Metadata.ContainsKey(key))
    //                _Metadata[key]=value;
    //            else
    //                _Metadata.Add(key, value);

    //        }
    //    }
    //}
}
