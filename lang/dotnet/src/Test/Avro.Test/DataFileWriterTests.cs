﻿using System;
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

                for (int i = 0; i < 5000000; i++)
                {
                    string test = string.Format("Test {0:###,###,##0}", i);
                    fileWriter.Append(test);



                }
                test2.Flush();


            }

            


        }
    }
}