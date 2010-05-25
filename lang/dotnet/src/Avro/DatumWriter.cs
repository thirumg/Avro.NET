/**
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
using System;
using Avro.IO;

namespace Avro
{
    public interface DatumWriter<T>
    {
        Schema Schema { set; }
        void Write(T datum, Encoder encoder);
    }


    /// <summary>
    /// DatumWriter for generic python objects.
    /// </summary>
    //public class DatumWriter
    //{
    //    public Schema WriterSchema { get; set; }

    //    public DatumWriter()
    //        : this(null)
    //    {

    //    }

    //    public DatumWriter(Schema writerSchema)
    //    {
            
    //        this.WriterSchema = writerSchema;
    //    }

    //    public void write(object value, BinaryEncoder encoder)
    //    {

    //    }

    //    public void write_data(Schema writers_schema, object datum, BinaryEncoder encoder)
    //    {
    //        if (!validate(writers_schema, datum))
    //            throw new AvroTypeException("Schema " + writers_schema + "value = " + datum);
              
    //        //# function dispatch to write datum
    //        if (writers_schema.Type == "null")
    //            encoder.write_null();
    //        else if (writers_schema.Type == "boolean")
    //            encoder.write_boolean((bool)datum);
    //        else if (writers_schema.Type == "string")
    //            encoder.write_utf8((string)datum);
    //        else if (writers_schema.Type == "int")
    //            encoder.write_int((int)datum);
    //        else if (writers_schema.Type == "long")
    //            encoder.write_long((long)datum);
    //        else if (writers_schema.Type == "float")
    //            encoder.write_float((float)datum);
    //        else if (writers_schema.Type == "double")
    //            encoder.write_double((double)datum);
    //        else if (writers_schema.Type == "bytes")
    //            encoder.write_bytes((byte[])datum);
    //        else if (writers_schema.Type == "fixed")
    //            write_fixed(writers_schema, datum, encoder);
    //        else if (writers_schema.Type == "enum")
    //            write_enum(writers_schema, datum, encoder);
    //        else if (writers_schema.Type == "array")
    //            write_array(writers_schema, datum, encoder);
    //        else if (writers_schema.Type == "map")
    //            write_map(writers_schema, datum, encoder);
    //        else if (writers_schema.Type == "union")
    //            write_union(writers_schema, datum, encoder);
    //        else if (Util.checkIsValue(writers_schema.Type, "record", "error", "request"))
    //            write_record(writers_schema, datum, encoder);
    //        else
    //            throw new AvroException("Unknown type: " + writers_schema.Type);

    //    }

    //    private bool validate(Schema writers_schema, object datum)
    //    {
    //        throw new NotImplementedException();
    //    }
    //    /// <summary>
    //    /// Fixed instances are encoded using the number of bytes declared in the schema.
    //    /// </summary>
    //    /// <param name="writers_schema"></param>
    //    /// <param name="datum"></param>
    //    /// <param name="encoder"></param>
    //    private void write_fixed(Schema writers_schema, object datum, BinaryEncoder encoder)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    private void write_enum(Schema writers_schema, object datum, BinaryEncoder encoder)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    private void write_array(Schema writers_schema, object datum, BinaryEncoder encoder)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    private void write_map(Schema writers_schema, object datum, BinaryEncoder encoder)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    private void write_union(Schema writers_schema, object datum, BinaryEncoder encoder)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    private void write_record(Schema writers_schema, object datum, BinaryEncoder encoder)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}
}
