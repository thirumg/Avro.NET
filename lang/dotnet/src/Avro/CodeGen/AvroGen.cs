/**
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
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
        private static readonly CodeTypeReference AvroFieldAttribute = new CodeTypeReference(typeof(Avro.FieldAttribute));
        private static readonly string TOOLNAME = typeof(AvroGen).FullName;
        private static readonly string VERSION = "1.0";//TODO: Update to actually pull the assembly version
        
        static AvroGen()
        {
            Dictionary<string, CodeTypeReference> primitiveLookup = new Dictionary<string, CodeTypeReference>();
            primitiveLookup.Add(SchemaType.BYTES, new CodeTypeReference(typeof(byte[])));
            primitiveLookup.Add(SchemaType.STRING, new CodeTypeReference(typeof(string)));
            primitiveLookup.Add(SchemaType.INT, new CodeTypeReference(typeof(int)));
            primitiveLookup.Add(SchemaType.LONG, new CodeTypeReference(typeof(long)));
            primitiveLookup.Add(SchemaType.BOOLEAN, new CodeTypeReference(typeof(bool)));
            primitiveLookup.Add(SchemaType.DOUBLE, new CodeTypeReference(typeof(double)));
            primitiveLookup.Add(SchemaType.FLOAT, new CodeTypeReference(typeof(float)));
            primitiveLookup.Add(SchemaType.NULL, null);

            _PrimitiveLookup = primitiveLookup;
        }

        public AvroGen()
        {
            this.Types = new List<Schema>();
            this.Protocols = new List<Protocol>();
        }

        public IList<Schema> Types { get; private set; }
        public IList<Protocol> Protocols { get; private set; }
        private Dictionary<string, CodeNamespace> _NamespaceLookup = new Dictionary<string, CodeNamespace>(StringComparer.Ordinal);
        private CodeCompileUnit _CompileUnit;
        private Dictionary<Schema, CodeTypeReference> _SchemaToCodeTypeReferenceLookup = new Dictionary<Schema, CodeTypeReference>();

        private CodeNamespace addNamespace(string name)
        {
            const string PREFIX = "addNamespace(string) - ";
            if (log.IsDebugEnabled) log.DebugFormat(PREFIX + "name = \"{0}\"", name);

            if (string.IsNullOrEmpty(name)) 
                throw new ArgumentNullException("name", "name cannot be null.");
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

        public CodeCompileUnit GenerateClient()
        {
            _CompileUnit = new CodeCompileUnit();

            processTypes();
            
            processClientProtocols();

            return _CompileUnit;
        }

        public CodeCompileUnit GenerateServer()
        {
            throw new NotImplementedException();
        }

        private void processTypes()
        {
            foreach (Schema schema in this.Types)
            {
                CodeNamespace ns = null;

                if (schema is NamedSchema)
                {
                    NamedSchema named = schema as NamedSchema;

                    if (named.name != null && !string.IsNullOrEmpty(named.name.space))
                    {
                        ns = addNamespace(named.name.space);
                    }
                }

                processSchema(ns, schema);
            }
        }

        private void processClientProtocols()
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

                    processSchema(ns, schema);
                }

                CodeTypeReference protocolInterface = createProtocolInterface(protocol, ns);
                createProtocolClient(protocol, ns, protocolInterface);
            }
        }

        private void processSchema(CodeNamespace ns, Schema schema)
        {
            if (Schema.Type.ENUM == schema.type)
            {
                processEnum(schema, ns);
            }
            else if (Schema.Type.FIXED == schema.type)
            {
                processFixed(schema);
            }
            else if (Schema.Type.RECORD == schema.type)
            {
                processRecord(schema, ns);
            }
            else if (Schema.Type.ERROR == schema.type)
            {
                CodeTypeDeclaration errorRecord = processRecord(schema, ns);
            }
            else if (Schema.Type.ARRAY == schema.type)
            {
                processArray(schema);
            }
            else if (Schema.Type.MAP == schema.type)
            {
                procesMap(schema);
            }
            else if (Schema.Type.UNION == schema.type)
            {
                processUnion(schema);
            }
            else
            {
                throw new NotSupportedException("Schema Schema of \"" + schema.type + "\" is not supported yet.");
            }
        }

        private void processFixed(Schema schema)
        {
            if (_SchemaToCodeTypeReferenceLookup.ContainsKey(schema))
                return;
            Schema fixedSchema = schema as FixedSchema;

            _SchemaToCodeTypeReferenceLookup.Add(schema, new CodeTypeReference(typeof(byte[])));
        }

        private bool findNullableType(UnionSchema schema, out CodeTypeReference type)
        {
            const string PREFIX = "findNullableType(UnionSchema, out Type) - ";

            if(schema.schemas.Count!=2)
            {

                type=null;
                return false;
            }

            bool isnullable = false;
            CodeTypeReference otherType = null;

            foreach (Schema childSchema in schema.schemas)
            {
                if (log.IsDebugEnabled) log.DebugFormat(PREFIX + "childSchema.Type = \"{0}\"", childSchema.type);

                if (PrimitiveSchema.NULL.Equals(childSchema))
                {
                    isnullable = true;
                }
                else
                {
                    otherType = getCodeTypeReference(childSchema);
                }
            }

            //TODO: Check if the type is an int, long, decimal, single

            



            type = otherType;
            return isnullable;
        }



        private void processUnion(Schema schema)
        {
            if (_SchemaToCodeTypeReferenceLookup.ContainsKey(schema))
                return;
            UnionSchema unionSchema = schema as UnionSchema;

            CodeTypeReference refNullable = null;
            if (findNullableType(unionSchema, out refNullable))
            {
                _SchemaToCodeTypeReferenceLookup.Add(schema, refNullable);
                return;
            }



            throw new NotImplementedException();
            //TODO: This is wrong
            //_SchemaToCodeTypeReferenceLookup.Add(schema, new CodeTypeReference(typeof(string)));
            //unionSchema.
        }

        private void procesMap(Schema schema)
        {
            if (_SchemaToCodeTypeReferenceLookup.ContainsKey(schema))
                return;

            MapSchema mapSchema = schema as MapSchema;

            CodeTypeReference typeRef = new CodeTypeReference(typeof(IDictionary<,>));
            typeRef.TypeArguments.Add(new CodeTypeReference(typeof(string)));
            CodeTypeReference valueRef = getCodeTypeReference(mapSchema.valueSchema);
            typeRef.TypeArguments.Add(valueRef);
            _SchemaToCodeTypeReferenceLookup.Add(schema, typeRef);
        }

        private void processArray(Schema schema)
        {
            if (_SchemaToCodeTypeReferenceLookup.ContainsKey(schema))
                return;

            ArraySchema arraySchema = schema as ArraySchema;

            CodeTypeReference arrayItemRef = getCodeTypeReference(arraySchema.itemSchema);

            CodeTypeReference arrayRef = new CodeTypeReference(arrayItemRef, 1);

            _SchemaToCodeTypeReferenceLookup.Add(schema, arrayRef);
        }

        private CodeTypeDeclaration processRecord(Schema schema, CodeNamespace ns)
        {
            RecordSchema recordSchema = schema as RecordSchema;

            CodeNamespace recordNamespace = null;

            if (string.IsNullOrEmpty(recordSchema.name.space) && null!=ns)
                recordNamespace = ns;
            else
                recordNamespace = addNamespace(recordSchema.name.space);

            CodeTypeReference refRecord = new CodeTypeReference(recordSchema.name.name);

            _SchemaToCodeTypeReferenceLookup.Add(schema, refRecord);

            CodeTypeDeclaration recordDeclare = createCodeTypeDeclaration(refRecord.BaseType);
            recordDeclare.Attributes = MemberAttributes.Public;
            recordDeclare.IsClass = true;
            recordDeclare.IsPartial = true;

            /* FIXME: Thiru
        foreach (Field field in recordSchema.Fields)
        {
            if (SchemaType.NULL == field.Schema.type)
            {
                //TODO: Look into this. It just feels wrong. I don't understand the need for a null field, but can this be stubbed out so that it will at least generate a field with type of object?
                if (log.IsDebugEnabled) log.DebugFormat("Skipping field \"{0}\" because it is null", field.Name);

                continue;
            }

            CodeTypeReference fieldType = getCodeTypeReference(field.Schema);
            if (null == fieldType)
            {
                processSchema(ns, field.Schema);

                fieldType = getCodeTypeReference(field.Schema);

                if (null == fieldType)
                {
                    throw new Exception("Field Schema \"" + field.Schema + "\" not found.");
                }
            }

            CodeCommentStatement propertyComment = string.IsNullOrEmpty(field.Documentation) ? null : createDocComment(field.Documentation);
            string privFieldName = string.Concat("_", field.Name);
            CodeFieldReferenceExpression fieldRef = new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), privFieldName);
            CodeMemberField codeField = new CodeMemberField(fieldType, fieldRef.FieldName);
            codeField.Attributes = MemberAttributes.Private;
            if (null != propertyComment)
                codeField.Comments.Add(propertyComment);

            recordDeclare.Members.Add(codeField);

            CodeMemberProperty property = new CodeMemberProperty();
            property.Attributes = MemberAttributes.Public|MemberAttributes.Final;
            property.Name = field.Name;
            property.Type = fieldType;
            addFieldAttribute(field, property);
            property.GetStatements.Add(new CodeMethodReturnStatement(fieldRef));
            property.SetStatements.Add(new CodeAssignStatement(fieldRef, new CodePropertySetValueReferenceExpression()));
            if (null != propertyComment)
                property.Comments.Add(propertyComment);
            recordDeclare.Members.Add(property);
        }
             */

            recordNamespace.Types.Add(recordDeclare);



            return recordDeclare;
        }

        private void addFieldAttribute(Field field, CodeMemberProperty property)
        {
            CodeAttributeDeclaration declare = new CodeAttributeDeclaration(AvroFieldAttribute);
            declare.Arguments.Add(new CodeAttributeArgument("Name", new CodePrimitiveExpression(field.Name)));
            property.CustomAttributes.Add(declare);
        }

        private static CodeTypeDeclaration createCodeTypeDeclaration(string name)
        {
            const string PREFIX = "createCodeTypeDeclaration(string) - ";
            if (log.IsDebugEnabled) log.DebugFormat(PREFIX + "name = \"{0}\"", name);
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name", "name cannot be null.");

            CodeTypeDeclaration typedeclare = new CodeTypeDeclaration(name);
            CodeAttributeDeclaration toolAttribute = new CodeAttributeDeclaration(new CodeTypeReference(typeof(System.CodeDom.Compiler.GeneratedCodeAttribute)));
            toolAttribute.Arguments.Add(new CodeAttributeArgument(new CodePrimitiveExpression(TOOLNAME)));
            toolAttribute.Arguments.Add(new CodeAttributeArgument(new CodePrimitiveExpression(VERSION)));
            typedeclare.CustomAttributes.Add(toolAttribute);
            return typedeclare;
        }

        private void processEnum(Schema schema, CodeNamespace ns)
        {
            EnumSchema enumschema = schema as EnumSchema;
            //TODO: This looks wrong. Double check is an enum schema a named schema? If so use the namespace if no namespace is provided. 
            if(null==enumschema) throw new NotSupportedException();
            CodeTypeReference refEnum = new CodeTypeReference(enumschema.name.full);
            CodeTypeDeclaration typeEnum = createCodeTypeDeclaration(refEnum.BaseType);
            typeEnum.BaseTypes.Add(typeof(int));
            typeEnum.IsEnum = true;
            typeEnum.Attributes = MemberAttributes.Public;

            int index = 0;
            foreach (string symbol in enumschema.symbols)
            {
                CodeMemberField field = new CodeMemberField(typeof(int), symbol);
                field.InitExpression = new CodePrimitiveExpression(index++);
                typeEnum.Members.Add(field);
            }

            _SchemaToCodeTypeReferenceLookup.Add(schema, refEnum);

            CodeNamespace addtoNs = null;

            if (null != enumschema.name && !string.IsNullOrEmpty(enumschema.name.space))
            {
                addtoNs = addNamespace(enumschema.name.space);
            }
            else
            {
                addtoNs = ns;
            }

            addtoNs.Types.Add(typeEnum);
        }

        private void createProtocolClient(Protocol protocol, CodeNamespace ns, CodeTypeReference protocolInterface)
        {
            string clientName = string.Concat(protocol.Name, "Client");
            CodeTypeDeclaration typeClient = createCodeTypeDeclaration(clientName);
            typeClient.IsClass = true;
            typeClient.IsPartial = true;
            typeClient.Attributes = MemberAttributes.Public;
            typeClient.BaseTypes.Add(new CodeTypeReference(typeof(Avro.RPC.RPCClient)));
            typeClient.BaseTypes.Add(protocolInterface);
            ns.Types.Add(typeClient);

            foreach (Message message in protocol.Messages)
            {
                CodeMemberMethod methodMessage = new CodeMemberMethod();
                typeClient.Members.Add(methodMessage);
                methodMessage.Name = message.Name;
                methodMessage.Attributes = MemberAttributes.Public | MemberAttributes.Final;
                if (!(message.Response == null||message.Response == PrimitiveSchema.NULL))
                    methodMessage.ReturnType = getCodeTypeReference(message.Response);
                if (!string.IsNullOrEmpty(message.Doc))
                    methodMessage.Comments.Add(createDocComment(message.Doc));

                List<CodeExpression> invokeParameters = new List<CodeExpression>();
                invokeParameters.Add(new CodePrimitiveExpression(message.Name));
                invokeParameters.Add(new CodePrimitiveExpression(null));
                foreach (Message.Parameter messageParm in message.Request)
                {
                    CodeVariableReferenceExpression varParm = new CodeVariableReferenceExpression(messageParm.Name);
                    CodeTypeReference messageTypeRef = getCodeTypeReference(messageParm.Schema);
                    CodeParameterDeclarationExpression decParm = new CodeParameterDeclarationExpression(messageTypeRef, messageParm.Name);
                    methodMessage.Parameters.Add(decParm);

                    CodeTypeReference refParameter = new CodeTypeReference(typeof(Avro.RPC.Parameter<>));
                    refParameter.TypeArguments.Add(messageTypeRef);

                    CodeObjectCreateExpression createparm = new CodeObjectCreateExpression(refParameter, new CodePrimitiveExpression(null), varParm);
                    invokeParameters.Add(createparm);
                }

                CodeMethodInvokeExpression callInvoke = new CodeMethodInvokeExpression(new CodeBaseReferenceExpression(), "Invoke", invokeParameters.ToArray());

                if ((message.Response == null || PrimitiveSchema.NULL.Equals(message.Response)))
                {
                    methodMessage.Statements.Add(callInvoke);
                }
                else
                {
                    callInvoke.Method.TypeArguments.Add(methodMessage.ReturnType);
                    methodMessage.Statements.Add(new CodeMethodReturnStatement(callInvoke));
                    
                }




                methodMessage.Statements.Add(new CodeThrowExceptionStatement( new CodeObjectCreateExpression( typeof(NotImplementedException))));

            }
        }

        private CodeCommentStatement createDocComment(string comment)
        {
            string text = string.Format("<summary>\r\n {0}\r\n </summary>", comment);
            return new CodeCommentStatement(text, true);
        }
  
        private CodeTypeReference createProtocolInterface(Protocol protocol, CodeNamespace ns)
        {
            CodeTypeReference typeRef = new CodeTypeReference(protocol.Name);
            CodeTypeDeclaration typeInterface = createCodeTypeDeclaration(typeRef.BaseType);
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

            /* FIXME: Thiru
            if (_PrimitiveLookup.TryGetValue(schema.type, out typeref))
                return typeref;
             */

            if (!_SchemaToCodeTypeReferenceLookup.TryGetValue(schema, out typeref))
            {
                processSchema(null, schema);

                if (!_SchemaToCodeTypeReferenceLookup.TryGetValue(schema, out typeref))
                {
                    throw new Exception("Could not determine type for " + schema);
                }
            }
            return typeref;
        }
    }
}
