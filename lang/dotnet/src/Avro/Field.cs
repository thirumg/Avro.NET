using System;
using System.Collections.Generic;
using System.Text;

namespace Avro
{
    enum SortOrder
    {
        Ascending,
        Descending,
        Ignore
    }

    class Field
    {
        public string Name { get; private set; }
        public string Namespace { get; set; }
        public string Doc { get; set; }
        public object Default { get; private set; }
        public bool HasDefault { get; private set; }
        public SortOrder? Order { get; private set; }
        public Schema Type { get; set; }


        public Field(Schema type, string name, bool hasDefault)
            : this(type, name, hasDefault, null, SortOrder.Ignore, null)
        {

        }

        public Field(Schema type, string name, bool hasDefault, object oDefault, SortOrder sortorder, Names names)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name", "name cannot be null.");

            if (null == type) throw new ArgumentNullException("type", "type cannot be null.");

            this.Type = type;
        }
    }
}
