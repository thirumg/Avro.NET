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
using System.Collections.Generic;
using Avro.IO;

namespace Avro
{
    public class GenericDatumWriter<T>:DatumWriter<T>
    {
        private static readonly Logger log = new Logger();

        public Schema Schema { get; set; }

        public void Write(T datum, Encoder encoder)
        {
            Write(this.Schema, datum, encoder);
        }

        protected void Write(Schema schema, object datum, Encoder encoder)
        {
            

            switch (schema.type)
            {
                case Schema.Type.RECORD: writeRecord(schema, datum, encoder); break;
                //    case Schema.ENUM:   writeEnum(schema, datum, encoder);   break;
                //    case Schema.ARRAY:  writeArray(schema, datum, encoder);  break;
                //    case Schema.MAP:    writeMap(schema, datum, encoder);    break;
                //    case Schema.UNION:
                //      int index = data.resolveUnion(schema, datum);
                //      out.writeIndex(index);
                //      write(schema.getTypes().get(index), datum, encoder);
                //      break;
                //    case Schema.FIXED:   writeFixed(schema, datum, encoder);   break;

                //case Schema.BYTES:   writeBytes(datum, encoder);           break;

                //case Schema.INT: encoder.WriteInt((int)datum); break;
                //case Schema.STRING: encoder.WriteString((string)datum); break;
                //case Schema.LONG: encoder.WriteLong((long)datum); break;
                //case Schema.FLOAT: encoder.WriteFloat((float)datum); break;
                //case Schema.DOUBLE: encoder.WriteDouble((Double)datum); break;
                //case Schema.BOOLEAN: encoder.WriteBoolean((Boolean)datum); break;
                //case Schema.NULL: encoder.WriteNull(); break;

                default:
                    error(schema, datum);
                    break;
            }
        }

        private void writeRecord(Schema schema, object datum, Encoder encoder)
        {
            
        }

        private void error(Schema schema, Object datum)
        {
            throw new AvroTypeException("Not a " + schema + ": " + datum);
        }
    }
}
