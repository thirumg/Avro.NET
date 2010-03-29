using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Avro
{
    public class Message
    {
        public string Name { get; set; }
        public string Doc { get; set; }
        public IList<Schema> Request { get; set; }
        public Schema Response { get; set; }
        public UnionSchema Error { get; set; }

        public Message(string name, string doc, IList<Schema> Request, Schema response, UnionSchema error)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name", "name cannot be null.");
            this.Request = Request;
            this.Response = response;
            this.Error = error;
            this.Name = name;
            this.Doc = doc;


        }

        internal static Message Parse(Newtonsoft.Json.Linq.JProperty jmessage, Names names)
        {
            string name = jmessage.Name;
            string doc = JsonHelper.getOptionalString(jmessage.Value, "doc");
            JToken jrequest = jmessage.Value["request"];
            JToken jresponse = jmessage.Value["response"];
            JToken jerrors = jmessage.Value["errors"];

            List<Schema> request = new List<Schema>();

            foreach (JToken jtype in jrequest)
            {
                Schema schema = Schema.Parse(jtype, names);
                request.Add(schema);
            }

            Schema response = Schema.Parse(jresponse, names);


            
            UnionSchema uerrorSchema = null;
            if (null != jerrors)
            {
                Schema errorSchema = Schema.Parse(jerrors, names);


                if (!(errorSchema is UnionSchema))
                {
                    throw new AvroException("");
                }

                uerrorSchema = errorSchema as UnionSchema;
            }



            return new Message(name, doc, request, response, uerrorSchema);


            //Message message = new Message(name, 



            throw new NotImplementedException();
        }

        internal void writeJson(Newtonsoft.Json.JsonTextWriter writer)
        {
            writer.WriteStartObject();
            //JsonHelper.writeIfNotNullOrEmpty(writer, "name", this.Name);
            JsonHelper.writeIfNotNullOrEmpty(writer, "doc", this.Doc);

            if (null != this.Request)
            {
                writer.WritePropertyName("request");
                writer.WriteStartArray();

                foreach (Schema schema in this.Request)
                {
                    schema.writeJson(writer);
                }

                writer.WriteEndArray();
            }

            if (null != this.Response)
            {
                writer.WritePropertyName("response");
                this.Response.writeJson(writer);
            }

            if (null != this.Error)
            {
                writer.WritePropertyName("errors");
                this.Error.writeJson(writer);
            }


            writer.WriteEndObject();
        }
    }
}
