using System;
using System.Collections.Generic;
using System.Text;

namespace Avro
{
    public class Name
    {
        public String name{get;set;}
        public String space { get; set; }
        public String full { get; set; }
        public Name(String name, String space)
        {
            if (name == null)
            {                         // anonymous
                this.name = this.space = this.full = null;
                return;
            }
            int lastDot = name.LastIndexOf('.');
            if (lastDot < 0)
            {                          // unqualified name
                this.space = space;                       // use default space
                this.name = name;
            }
            else
            {                                    // qualified name
                this.space = name.Substring(0, lastDot);  // get space from name
                this.name = name.Substring(lastDot + 1, name.Length);
            }
            this.full = (this.space == null) ? this.name : this.space + "." + this.name;
        }
        public override bool Equals(Object o)
        {
            if (o == this) return true;
            if (!(o is Name)) return false;
            Name that = (Name)o;
            return full == null ? that.full == null : full.Equals(that.full);
        }
        public override int GetHashCode()
        {
            return full == null ? 0 : full.GetHashCode();
        }
        public override string ToString()
        {
            return full;
        }
        
        //public void writeName(Names names, JsonGenerator gen) throws IOException {
        //  if (name != null) gen.writeStringField("name", name);
        //  if (space != null) {
        //    if (!space.equals(names.space()))
        //      gen.writeStringField("namespace", space);
        //    if (names.space() == null)                // default namespace
        //      names.space(space);
        //  }
        //}

    
        public static string make_fullname(string name, string snamespace)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name", "name cannot be null.");

            if (!name.Contains(".") && !string.IsNullOrEmpty(snamespace))
            {
                return string.Concat(snamespace, ".", name);
            }
            else
                return name;
        }
    }
}
