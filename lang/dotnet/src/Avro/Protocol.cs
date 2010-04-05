﻿using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;
using System.IO;
namespace Avro
{
    public class Protocol
    {
        public static Protocol Parse(string json)
        {
            if (string.IsNullOrEmpty(json)) throw new ArgumentNullException("json", "json cannot be null.");

            JToken jsonData = null;

            try
            {
                jsonData = JObject.Parse(json);

            }
            catch (Exception ex)
            {
                throw new ProtocolParseException("Error parsing JSON: " + json);
            }

            return Parse(jsonData);

        }

        public string Name{ get; set; }
        public string Namespace { get; set; }
        public IList<Schema> Types { get; set; }
        public IList<Message> Messages { get; set; }
        public string Doc { get; set; }

        private static Protocol Parse(JToken j)
        {
            string protocol = JsonHelper.getRequiredString(j, "protocol");
            string snamespace = JsonHelper.getRequiredString(j, "namespace");
            string doc = JsonHelper.getOptionalString(j, "doc");

            Names names = new Names();

            JToken jtypes = j["types"];
            List<Schema> types = new List<Schema>();
            if (jtypes is JArray)
            {
                foreach (JToken jtype in jtypes)
                {
                    Schema schema = Schema.ParseJson(jtype, names);
                    types.Add(schema);
                }
            }


            JToken jmessages = j["messages"];
            List<Message> messages = new List<Message>();


            foreach (JProperty jmessage in jmessages)
            {
                Message message = Message.Parse(jmessage, names);
                messages.Add(message);
            }

            


            return new Protocol(protocol, snamespace, doc, types, messages);
        }

        public Protocol(string name, string snamespace, string doc, IList<Schema> types, IList<Message> messages)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name", "name cannot be null.");
            if (null == types) throw new ArgumentNullException("types", "types cannot be null.");
            if (null == messages) throw new ArgumentNullException("messages", "messages cannot be null.");

            this.Name = name;
            this.Namespace = snamespace;
            this.Doc = doc;
            this.Types = types;
            this.Messages = messages;
        }

        

        public override string ToString()
        {
            using (StringWriter sw = new StringWriter())
            {

                

                using (Newtonsoft.Json.JsonTextWriter writer = new Newtonsoft.Json.JsonTextWriter(sw))
                {
                    #if(DEBUG)
                    writer.Formatting = Newtonsoft.Json.Formatting.Indented;
                    #endif

                    writeJson(writer);
                    writer.Flush();
                    return sw.ToString();
                }
            }
        }

        internal void writeJson(Newtonsoft.Json.JsonTextWriter writer)
        {
            writer.WriteStartObject();

            JsonHelper.writeIfNotNullOrEmpty(writer, "namespace", this.Namespace);
            JsonHelper.writeIfNotNullOrEmpty(writer, "doc", this.Doc);
            JsonHelper.writeIfNotNullOrEmpty(writer, "protocol", this.Name);

            writer.WritePropertyName("types");
            writer.WriteStartArray();

            foreach (Schema type in this.Types)
            {
                type.writeJson(writer);
            }

            writer.WriteEndArray();


            writer.WritePropertyName("messages");
            writer.WriteStartObject();

            foreach (Message message in this.Messages)
            {
                writer.WritePropertyName(message.Name);
                message.writeJson(writer);
            }

            writer.WriteEndObject();

            writer.WriteEndObject();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Protocol)) return false;

            string a = this.ToString();
            string b = obj.ToString();
            //TODO:This will perform like shit. FIx it.

            return string.Equals(a, b);
        }
    }
}
