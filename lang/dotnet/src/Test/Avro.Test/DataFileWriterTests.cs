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
using System.Text;
using NUnit.Framework;
using System.IO;
namespace Avro.Test
{
    public class DataFileWriterTests
    {
        [TestCase]
        public void Test()
        {
            Stream iostr = new FileStream("Test.bin", FileMode.Create);

            using (iostr)
            {
                GenericDatumWriter<string> datumWriter = new GenericDatumWriter<string>();
                DataFileWriter<string> fileWriter = new DataFileWriter<string>(datumWriter);
                Schema schema = new PrimitiveSchema("string");
                DataFileWriter<string> test2 = fileWriter.create(schema, iostr);

                for (int i = 0; i < 5000; i++)
                {
                    string test = string.Format("Test {0:###,###,##0}", i * 100);
                    fileWriter.Append(test);



                }
                test2.Flush();
            }

            iostr = new FileStream("Test.bin", FileMode.Open);

            using (iostr)
            {
                GenericDatumReader<string> datumReader = new GenericDatumReader<string>();
                DataFileReader<string> filereader = new DataFileReader<string>(iostr, datumReader);

                foreach (string test in filereader.GetItems())
                {

                }
            }






        }
    }
}
