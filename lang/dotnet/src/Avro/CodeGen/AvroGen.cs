using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;
using System.CodeDom.Compiler;


namespace Avro.CodeGen
{
    public class AvroGen
    {
        private static readonly Logger log = new Logger();

        private static readonly Dictionary<string, CodeTypeReference> _PrimitiveLookup;

        static AvroGen()
        {
            Dictionary<string, CodeTypeReference> primitiveLookup = new Dictionary<string, CodeTypeReference>();
            primitiveLookup.Add(Schema.BYTES, new CodeTypeReference(typeof(byte[])));
            primitiveLookup.Add(Schema.STRING, new CodeTypeReference(typeof(string)));
            primitiveLookup.Add(Schema.INT, new CodeTypeReference(typeof(int)));
            primitiveLookup.Add(Schema.LONG, new CodeTypeReference(typeof(long)));
            primitiveLookup.Add(Schema.NULL, null);

            _PrimitiveLookup = primitiveLookup;
        }





        public IList<Schema> Types { get; private set; }
        public IList<Protocol> Protocols { get; private set; }

        public AvroGen()
        {
            this.Types = new List<Schema>();
            this.Protocols = new List<Protocol>();
        }

        private Dictionary<string, CodeNamespace> _NamespaceLookup = new Dictionary<string, CodeNamespace>(StringComparer.Ordinal);
        private CodeCompileUnit _CompileUnit;
        private Dictionary<Schema, CodeTypeReference> _SchemaToCodeTypeReferenceLookup = new Dictionary<Schema, CodeTypeReference>();

        private CodeNamespace addNamespace(string name)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name", "name cannot be null.");
            if (log.IsDebugEnabled) log.DebugFormat("addNamespace(string) - name = \"{0}\"", name);
            CodeNamespace ns = null;

            if (!_NamespaceLookup.TryGetValue(name, out ns))
            {
                ns = new CodeNamespace(name);
                _CompileUnit.Namespaces.Add(ns);
                _NamespaceLookup.Add(name, ns);
            }
            return ns;
        }

        public CodeCompileUnit Generate()
        {
            _CompileUnit = new CodeCompileUnit();

            processTypes();
            
            processProtocols();


            return _CompileUnit;
        }

        private void processTypes()
        {
            
        }

        private void processProtocols()
        {
            foreach (Protocol protocol in Protocols)
            {
                CodeNamespace ns = addNamespace(protocol.Namespace);

                foreach (Schema schema in protocol.Types)
                {
                    if (_SchemaToCodeTypeReferenceLookup.ContainsKey(schema))
                    {
                        if (log.IsDebugEnabled) log.DebugFormat("processProtocols() - Schema already exists so skipping. \"{0}\"", schema);
                        continue;
                    }

                    if (Schema.ENUM == schema.Type)
                    {
                        processEnum(schema, ns);
                    }
                    else if (Schema.FIXED == schema.Type)
                    {

                    }
                    else if (Schema.RECORD == schema.Type)
                    {
                        processRecord(schema, ns);
                    }
                    else if (Schema.ERROR == schema.Type)
                    {
                        CodeTypeDeclaration errorRecord = processRecord(schema, ns);

                    }
                    else
                    {
                        throw new NotSupportedException("Schema Type of \"" + schema.Type + "\" is not supported yet.");
                    }
                }


             
                CodeTypeReference protocolInterface = createProtocolInterface(protocol, ns);
                createProtocolClient(protocol, ns, protocolInterface);
            }
        }

        private CodeTypeDeclaration processRecord(Schema schema, CodeNamespace ns)
        {
            RecordSchema recordSchema = schema as RecordSchema;

            CodeNamespace recordNamespace = null;

            if (string.IsNullOrEmpty(recordSchema.Name.space))
                recordNamespace = ns;
            else
                recordNamespace = addNamespace(recordSchema.Name.space);

            CodeTypeReference refRecord = new CodeTypeReference(recordSchema.Name.name);

            CodeTypeDeclaration recordDeclare = new CodeTypeDeclaration(refRecord.BaseType);
            recordDeclare.Attributes = MemberAttributes.Public;
            recordDeclare.IsClass = true;
            recordDeclare.IsPartial = true;



            recordNamespace.Types.Add(recordDeclare);

            _SchemaToCodeTypeReferenceLookup.Add(schema, refRecord);

            return recordDeclare;
        }

        private void processEnum(Schema schema, CodeNamespace ns)
        {
            EnumSchema enumschema = schema as EnumSchema;
            //TODO: This looks wrong. Double check is an enum schema a named schema? If so use the namespace if no namespace is provided. 
            if(null==enumschema) throw new NotSupportedException();
            CodeTypeReference refEnum = new CodeTypeReference(enumschema.Name.full);
            CodeTypeDeclaration typeEnum = new CodeTypeDeclaration(refEnum.BaseType);
            typeEnum.IsEnum = true;
            typeEnum.Attributes = MemberAttributes.Public;

            foreach (string symbol in enumschema.Symbols)
            {
                CodeMemberField field = new CodeMemberField(typeof(int), symbol);
                typeEnum.Members.Add(field);
            }

            _SchemaToCodeTypeReferenceLookup.Add(schema, refEnum);
            ns.Types.Add(typeEnum);
        }

        private void createProtocolClient(Protocol protocol, CodeNamespace ns, CodeTypeReference protocolInterface)
        {
            string clientName = string.Concat(protocol.Name, "Client");
            CodeTypeDeclaration typeClient = new CodeTypeDeclaration(clientName);
            typeClient.IsClass = true;
            typeClient.IsPartial = true;
            typeClient.Attributes = MemberAttributes.Public;
            typeClient.BaseTypes.Add(new CodeTypeReference(typeof(Avro.RPC.RPCClient)));
            typeClient.BaseTypes.Add(protocolInterface);
            ns.Types.Add(typeClient);

        }

        private CodeCommentStatement createDocComment(string comment)
        {
            string text = string.Format("<summary>\r\n {0}\r\n </summary>", comment);
            return new CodeCommentStatement(text, true);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="protocol"></param>
        /// <param name="ns"></param>

        private CodeTypeReference createProtocolInterface(Protocol protocol, CodeNamespace ns)
        {
            CodeTypeReference typeRef = new CodeTypeReference(protocol.Name);
            CodeTypeDeclaration typeInterface = new CodeTypeDeclaration(typeRef.BaseType);
            typeInterface.IsInterface = true;
            typeInterface.Attributes = MemberAttributes.Public;
            ns.Types.Add(typeInterface);
            if (!string.IsNullOrEmpty(protocol.Doc))
                typeInterface.Comments.Add(createDocComment(protocol.Doc));

            foreach (Message message in protocol.Messages)
            {
                CodeMemberMethod method = new CodeMemberMethod();
                method.Name = message.Name;
                method.ReturnType = getCodeTypeReference(message.Response);
                typeInterface.Members.Add(method);

                if (!string.IsNullOrEmpty(message.Doc))
                    method.Comments.Add(createDocComment(message.Doc));

                foreach (Message.Parameter parameter in message.Request)
                {
                    CodeTypeReference parmType = getCodeTypeReference(parameter.Schema);
                    CodeParameterDeclarationExpression parm = new CodeParameterDeclarationExpression(parmType, parameter.Name);
                    method.Parameters.Add(parm);
                }
            }

            return typeRef;
        }

        private CodeTypeReference getCodeTypeReference(Schema schema)
        {
            CodeTypeReference typeref = null;

            if (_PrimitiveLookup.TryGetValue(schema.Type, out typeref))
                return typeref;

            if (!_SchemaToCodeTypeReferenceLookup.TryGetValue(schema, out typeref))
            {
                typeref = new CodeTypeReference(typeof(string));
            }
            return typeref;
        }
    }
}
