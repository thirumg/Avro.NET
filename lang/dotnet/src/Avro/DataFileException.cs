using System;
using System.Collections.Generic;
using System.Text;

namespace Avro
{
    /// <summary>
    /// Raised when there's a problem reading or writing file object containers.
    /// </summary>
    public class DataFileException:AvroException
    {
        public DataFileException(string s)
            : base(s)
        {

        }
    }
}
