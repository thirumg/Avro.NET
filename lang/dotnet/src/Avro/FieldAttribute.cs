using System;
using System.Collections.Generic;


namespace Avro
{
    [AttributeUsage(AttributeTargets.Field|AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public sealed class FieldAttribute : Attribute
    {
        public string Name { get; set; }

        public FieldAttribute()
        {

        }


    }
}
