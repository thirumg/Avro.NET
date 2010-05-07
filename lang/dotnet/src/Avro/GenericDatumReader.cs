using System;
using System.Collections.Generic;
using Avro.IO;
namespace Avro
{
    public class GenericDatumReader<T>:DatumReader<T>
    {
        public Schema Schema { get; set; }
        public Schema Expected { get; set; }
        


        public T Read(T reuse, Decoder decoder)
        {
            throw new NotImplementedException();
        }


    }
}
