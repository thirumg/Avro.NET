using System;
using System.Collections.Generic;
using System.Text;

namespace Avro.Test
{
    class ExampleProtocol
    {
        public bool Valid { get; set; }
        public string Name { get; set; }
        public string Comment { get; set; }
        public string Protocol { get; set; }

        public ExampleProtocol(string protocol, bool valid)
            : this(protocol, valid, string.Empty, string.Empty)
        {

        }

        public ExampleProtocol(string protocol, bool valid, string name, string comment)
        {
            this.Protocol = protocol;
            this.Valid = valid;
            this.Name = name;
            this.Comment = comment;
        }

    }
}
