﻿using System;
using System.Collections.Generic;


namespace Avro
{
    /// <summary>
    /// Deserialize Avro-encoded data into a .net data structure.
    /// </summary>
    class DatumReader
    {
        public Schema WriterSchema { get; private set; }
        public Schema ReaderSchema { get; private set; }

        /// <summary>
        /// As defined in the Avro specification, we call the schema encoded
        /// in the data the "writer's schema", and the schema expected by the
        /// reader the "reader's schema".
        /// </summary>
        /// <param name="writerSchema"></param>
        /// <param name="readerSchema"></param>
        public DatumReader(Schema writerSchema, Schema readerSchema)
        {
            if (null == writerSchema) throw new ArgumentNullException("writerSchema", "writerSchema cannot be null.");
            if (null == readerSchema) throw new ArgumentNullException("readerSchema", "readerSchema cannot be null.");

            this.WriterSchema = writerSchema;
            this.ReaderSchema = readerSchema;
        }


        static bool check_props(Schema schema_one, Schema schema_two, params string[] prop_list)
        {
            return check_props(schema_one, schema_two, prop_list);
        }
        static bool check_props(Schema schema_one, Schema schema_two, IEnumerable<string> prop_list)
        {
            throw new NotImplementedException();
            //foreach (string prop in prop_list)
            //{
            //    if (getattr(schema_one, prop) != getattr(schema_two, prop))
            //        return false;
            //}
            //return true;
        }
        static bool match_schemas(Schema writers_schema, Schema readers_schema)
        {
            string w_type = writers_schema.Type, r_type = readers_schema.Type;

            if (string.Equals("union", w_type) || string.Equals("union", r_type))
                return true;
            else if (PrimitiveSchema.PrimitiveKeyLookup.ContainsKey(w_type) && PrimitiveSchema.PrimitiveKeyLookup.ContainsKey(r_type) && w_type == r_type)
                return true;
            else if (w_type == "record" && r_type == "record" && DatumReader.check_props(writers_schema, readers_schema, "fullname"))
                return true;
            else if (w_type == "error" && r_type == "error" && DatumReader.check_props(writers_schema, readers_schema, "fullname"))
                return true;
            else if (w_type == "request" && r_type == "request")
                return true;
            else if (w_type == "fixed" && r_type == "fixed" && DatumReader.check_props(writers_schema, readers_schema, "fullname", "size"))
                return true;
            else if (w_type == "enum" && r_type == "enum" && DatumReader.check_props(writers_schema, readers_schema, "fullname"))
                return true;
            //else if (w_type == "map" && r_type == "map" && DatumReader.check_props(writers_schema.values, readers_schema.values, "type"))
            //    return true;
            //else if (w_type == "array" && r_type == "array" && DatumReader.check_props(writers_schema.items, readers_schema.items, "type"))
            //    return true;
            if (w_type == "int" && Util.checkIsValue(r_type, "long", "float", "double"))
                return true;
            else if (w_type == "long" && Util.checkIsValue(r_type, "float", "double"))
                return true;
            else if (w_type == "float" && r_type == "double")
                return true;

            if (Util.checkIsValue(w_type, "map", "array"))
                throw new NotImplementedException(w_type);

            return false;
        }

        public object read(BinaryDecoder decoder)
        {
            if (null == this.ReaderSchema)
                this.ReaderSchema = this.WriterSchema;

            return read_data(this.WriterSchema, this.ReaderSchema, decoder);

        }

        private object read_data(Schema writers_schema, Schema readers_schema, BinaryDecoder decoder)
        {
            if (!match_schemas(writers_schema, readers_schema))
                throw new SchemaResolutionException("Schemas do not match.", writers_schema, readers_schema);

            if (writers_schema.Type != "union" && readers_schema.Type == "union")
            {
                foreach (Schema s in ((UnionSchema)readers_schema).Schemas)
                {
                    if (DatumReader.match_schemas(writers_schema, s))
                    {
                        return read_data(writers_schema, s, decoder);
                    }
                }

                throw new SchemaResolutionException("Schemas do not match.", writers_schema, readers_schema);
            }

            if (writers_schema.Type == "null")
                return decoder.ReadNull();
            else if (writers_schema.Type == "boolean")
                return decoder.ReadBool();
            else if (writers_schema.Type == "string")
                return decoder.ReadUTF8();
            else if (writers_schema.Type == "int")
                return decoder.ReadInt();
            else if (writers_schema.Type == "long")
                return decoder.ReadLong();
            else if (writers_schema.Type == "float")
                return decoder.ReadFloat();
            else if (writers_schema.Type == "double")
                return decoder.ReadDouble();
            else if (writers_schema.Type == "bytes")
                return decoder.ReadBytes();
            else if (writers_schema.Type == "fixed")
                return read_fixed(writers_schema, readers_schema, decoder);
            else if (writers_schema.Type == "enum")
                return read_enum(writers_schema, readers_schema, decoder);
            else if (writers_schema.Type == "array")
                return read_array(writers_schema, readers_schema, decoder);
            else if (writers_schema.Type == "map")
                return read_map(writers_schema, readers_schema, decoder);
            else if (writers_schema.Type == "union")
                return read_union(writers_schema, readers_schema, decoder);
            else if (Util.checkIsValue(writers_schema.Type, "record", "error", "request"))
                return read_record(writers_schema, readers_schema, decoder);
            else
                throw new AvroException("Cannot read unknown schema type) " + writers_schema.Type);
        }

        public void skip_data(Schema writers_schema, BinaryDecoder decoder)
        {
            if (writers_schema.Type == "null")
                decoder.SkipNull();
            else if (writers_schema.Type == "boolean")
                decoder.SkipBoolean();
            else if (writers_schema.Type == "string")
                decoder.SkipUTF8();
            else if (writers_schema.Type == "int")
                decoder.SkipInt();
            else if (writers_schema.Type == "long")
                decoder.SkipLong();
            else if (writers_schema.Type == "float")
                decoder.SkipFloat();
            else if (writers_schema.Type == "double")
                decoder.SkipDouble();
            else if (writers_schema.Type == "bytes")
                decoder.ReadBytes();
            else if (writers_schema.Type == "fixed")
                skip_fixed(writers_schema as FixedSchema, decoder);
            else if (writers_schema.Type == "enum")
                skip_enum(writers_schema, decoder);
            else if (writers_schema.Type == "array")
                skip_array(writers_schema as ArraySchema, decoder);
            else if (writers_schema.Type == "map")
                skip_map(writers_schema as MapSchema, decoder);
            else if (writers_schema.Type == "union")
                skip_union(writers_schema as UnionSchema, decoder);
            else if (Util.checkIsValue(writers_schema.Type, "record", "error", "request"))
                skip_record(writers_schema as RecordSchema, decoder);
            else
                throw new AvroException("Unknown schema type: %s" + writers_schema.Type);

        }

        private void skip_record(RecordSchema writers_schema, BinaryDecoder decoder)
        {
            foreach (Field field in writers_schema.Fields)
                skip_data(field.Type, decoder);
        }

        private void skip_union(UnionSchema writers_schema, BinaryDecoder decoder)
        {
            int index_of_schema = (int)decoder.ReadLong();
            skip_data(writers_schema.Schemas[index_of_schema], decoder);
        }

        private void skip_map(MapSchema writers_schema, BinaryDecoder decoder)
        {
            long block_count = decoder.ReadLong();
            while (block_count != 0)
            {
                if (block_count < 0)
                {
                    long block_size = decoder.ReadLong();
                    decoder.skip(block_size);
                }
                else
                {
                    for (int i = 0; i < block_count; i++)
                    {
                        decoder.SkipUTF8();
                        skip_data(writers_schema.Values, decoder);
                        block_count = decoder.ReadLong();
                    }
                }
            }
        }


        private void skip_array(ArraySchema writers_schema, BinaryDecoder decoder)
        {
            long block_count = decoder.ReadLong();
            while (block_count != 0)
            {
                if (block_count < 0)
                {
                    long block_size = decoder.ReadLong();
                    decoder.skip(block_size);
                }
                else
                {
                    for (int i = 0; i < block_count; i++)
                    {
                        decoder.SkipUTF8();
                        skip_data(writers_schema.Items, decoder);
                        block_count = decoder.ReadLong();
                    }
                }
            }
        }

        private void skip_enum(Schema writers_schema, BinaryDecoder decoder)
        {
            decoder.SkipInt();
        }

        private void skip_fixed(FixedSchema writers_schema, BinaryDecoder decoder)
        {
            decoder.skip(writers_schema.Size);
        }

        /// <summary>
        /// A record is encoded by encoding the values of its fields
        /// in the order that they are declared. In other words, a record
        /// is encoded as just the concatenation of the encodings of its fields.
        /// Field values are encoded per their schema.

        /// Schema Resolution:
        ///  * the ordering of fields may be different: fields are matched by name.
        ///  * schemas for fields with the same name in both records are resolved
        ///    recursively.
        ///  * if the writer's record contains a field with a name not present in the
        ///    reader's record, the writer's value for that field is ignored.
        ///  * if the reader's record schema has a field that contains a default value,
        ///    and writer's schema does not have a field with the same name, then the
        ///    reader should use the default value from its field.
        ///  * if the reader's record schema has a field with no default value, and 
        ///    writer's schema does not have a field with the same name, then the
        ///    field's value is unset.
        /// </summary>
        /// <param name="writers_schema"></param>
        /// <param name="readers_schema"></param>
        /// <param name="decoder"></param>
        /// <returns></returns>
        private object read_record(Schema writers_schema, Schema readers_schema, BinaryDecoder decoder)
        {
            throw new NotImplementedException();
        }

        private object read_union(Schema writers_schema, Schema readers_schema, BinaryDecoder decoder)
        {
            throw new NotImplementedException();
        }

        private object read_map(Schema writers_schema, Schema readers_schema, BinaryDecoder decoder)
        {
            throw new NotImplementedException();
        }

        private object read_array(Schema writers_schema, Schema readers_schema, BinaryDecoder decoder)
        {
            throw new NotImplementedException();
        }

        private object read_enum(Schema writers_schema, Schema readers_schema, BinaryDecoder decoder)
        {
            throw new NotImplementedException();
        }

        private object read_fixed(Schema writers_schema, Schema readers_schema, BinaryDecoder decoder)
        {
            throw new NotImplementedException();
        }
    }
}
