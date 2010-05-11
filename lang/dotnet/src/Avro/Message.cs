using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Avro
{
    public class Message
    {
        public class Parameter
        {
            /// <summary>
            /// Name of the parameter
            /// </summary>
            public string Name { get; set; }
            /// <summary>
            /// Documentation for the parameter
            /// </summary>
            public string Doc { get; set; }
            /// <summary>
            /// Schema for the parameter
            /// </summary>
            public Schema Schema { get; set; }

            internal void writeJson(Newtonsoft.Json.JsonTextWriter writer)
            {
                throw new NotImplementedException();
            }
        }

        public string Name { get; set; }
        public string Doc { get; set; }
        public IList<Parameter> Request { get; set; }
        public Schema Response { get; set; }
        public UnionSchema Error { get; set; }

        public Message(string name, string doc, IList<Parameter> request, Schema response, UnionSchema error)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name", "name cannot be null.");
            this.Request = request;
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

            List<Parameter> request = new List<Parameter>();

            foreach (JToken jtype in jrequest)
            {
                Parameter parameter = new Parameter();
                parameter.Name = JsonHelper.getRequiredString(jtype, "name");
                parameter.Schema = Schema.ParseJson(jtype, names);

                if (null == parameter.Schema)
                    throw new SchemaParseException(jtype.ToString());

                request.Add(parameter);
            }

            Schema response = Schema.ParseJson(jresponse, names);


            
            UnionSchema uerrorSchema = null;
            if (null != jerrors)
            {
                Schema errorSchema = Schema.ParseJson(jerrors, names);


                if (!(errorSchema is UnionSchema))
                {
                    throw new AvroException("");
                }

                uerrorSchema = errorSchema as UnionSchema;
            }



            return new Message(name, doc, request, response, uerrorSchema);


            //Message message = new Message(name, 



            //throw new NotImplementedException();
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

                foreach (Parameter parameter in this.Request)
                {
                    System.Diagnostics.Debug.Assert(parameter != null);
                    parameter.writeJson(writer);
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
