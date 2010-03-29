using System;
using System.Collections.Generic;
using System.Text;

namespace Avro
{
    class Name
    {
        public static string make_fullname(string Name, string Namespace)
        {
            
            if (string.IsNullOrEmpty(Name)) throw new ArgumentNullException("Name", "Name cannot be null.");

            if (Name.Contains("."))
                return Name;
            else
                return string.Concat(Name, ".", Namespace);
        }

        public static string extract_namespace(string Name, string Namespace)
        {
            throw new NotImplementedException();
        }

        public static string add_name(IDictionary<string, Name> names, Schema new_schema)
        {
            throw new NotImplementedException();
        }
    }
}
