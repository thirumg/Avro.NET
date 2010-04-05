using System;
using System.Collections.Generic;
using System.Text;

namespace Avro
{
    public class Name:IEquatable<Name>
    {
        public String name{ get;private set;}
        public String space { get; private set; }
        public String full { get; private set; }


        public Name(String name, String space)
        {
            if (name == null)
            {                         // anonymous
                this.name = this.space = this.full = null;
                return;
            }

            if(!name.Contains("."))
            {                          // unqualified name
                this.space = space;                       // use default space
                this.name = name;
            }
            else
            {
                string[] parts = name.Split('.');

                this.space = string.Join(".", parts, 0, parts.Length - 2);
                this.name = parts[0];
                
                // qualified name
                //this.space = name.Substring(0, lastDot);  // get space from name
                //this.name = name.Substring(lastDot + 1, name.Length-lastDot+1);
            }
            this.full = string.IsNullOrEmpty(this.space) ? this.name : this.space + "." + this.name;
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
            return string.IsNullOrEmpty(full)? 0 : full.GetHashCode();
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

    
        public static Name make_fullname(string name, string snamespace)
        {
            return new Name(name, snamespace);


            throw new NotImplementedException();
            //if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name", "name cannot be null.");

            //if (!name.Contains(".") && !string.IsNullOrEmpty(snamespace))
            //{
            //    return string.Concat(snamespace, ".", name);
            //}
            //else
            //    return name;
        }



        internal static string make_fullname(Name name, string snamespace)
        {
            throw new NotImplementedException();
        }

        internal void WriteJson(Newtonsoft.Json.JsonTextWriter writer)
        {
            JsonHelper.writeIfNotNullOrEmpty(writer, "name", this.name);
            JsonHelper.writeIfNotNullOrEmpty(writer, "namespace", this.space);
        }

        

        public bool Equals(Name other)
        {
            if (null == other)
                return false;

            return this.full.Equals(other.full);
        }
    }
}
