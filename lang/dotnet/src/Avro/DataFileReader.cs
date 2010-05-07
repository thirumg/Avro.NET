using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.IO.Compression;

namespace Avro
{
    public class DataFileReader
    {
        

        private Stream _Reader;

        public DataFileReader(Stream iostr)
        {
            if (null == iostr) throw new ArgumentNullException("iostr", "iostr cannot be null.");
            this._Reader = iostr;


        }
    }
}
