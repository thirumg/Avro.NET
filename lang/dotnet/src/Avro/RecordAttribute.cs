using System;
using System.Collections.Generic;
using System.Text;

namespace Avro
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class RecordAttribute : Attribute
    {
        public string Name { get; set; }
        public string Namespace { get; set; }
        public string Documentation { get; set; }

        public RecordAttribute()
        {

        }
    }
}
