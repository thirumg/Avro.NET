using System;
using System.Collections.Generic;
using Avro.IO;

namespace Avro
{
    public class GenericDatumWriter<T>:DatumWriter<T>
    {
        public Schema Schema { get; set; }

        public void Write(T datum, Encoder encoder)
        {
            Write(this.Schema, datum, encoder);
        }

        protected void Write(Schema schema, object datum, Encoder encoder)
        {
            switch (schema.Type)
            {
//case Schema.RECORD: writeRecord(schema, datum, encoder); break;
//    case Schema.ENUM:   writeEnum(schema, datum, encoder);   break;
//    case Schema.ARRAY:  writeArray(schema, datum, encoder);  break;
//    case Schema.MAP:    writeMap(schema, datum, encoder);    break;
//    case Schema.UNION:
//      int index = data.resolveUnion(schema, datum);
//      out.writeIndex(index);
//      write(schema.getTypes().get(index), datum, encoder);
//      break;
//    case Schema.FIXED:   writeFixed(schema, datum, encoder);   break;
    case Schema.STRING:  writeString(schema, datum, encoder);  break;
    //case Schema.BYTES:   writeBytes(datum, encoder);           break;
    //case Schema.INT:     encoder.writeInt((int)datum);     break;
    //case Schema.LONG:    encoder.writeLong((long)datum);       break;
    //case Schema.FLOAT:   encoder.writeFloat((float)datum);     break;
    //case Schema.DOUBLE:  encoder.writeDouble((Double)datum);   break;
    //case Schema.BOOLEAN: encoder.writeBoolean((Boolean)datum); break;
    //case Schema.NULL:    encoder.writeNull();                  break;

                default:
                    error(schema, datum);
                    break;
            }
        }

        private void writeString(Schema schema, object datum, Encoder encoder)
        {
            encoder.WriteString((string)datum);
        }

        private void error(Schema schema, Object datum)
        {
            throw new AvroTypeException("Not a " + schema + ": " + datum);
        }
    }
}
