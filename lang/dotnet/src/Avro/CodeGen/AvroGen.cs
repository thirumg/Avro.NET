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

        public CodeCompileUnit Generate()
        {
            _CompileUnit = new CodeCompileUnit();

            processTypes();
            
            processProtocols();


            return _CompileUnit;
        }

        private void processTypes()
        {
            foreach (Schema schema in this.Types)
            {
                CodeNamespace ns = null;

                if (schema is NamedSchema)
                {
                    NamedSchema named = schema as NamedSchema;

                    if (named.Name != null && !string.IsNullOrEmpty(named.Name.space))
                    {
                        ns = addNamespace(named.Name.space);
                    }
                }



                processSchema(ns, schema);
            }
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

                    processSchema(ns, schema);
                }


             
                CodeTypeReference protocolInterface = createProtocolInterface(protocol, ns);
                createProtocolClient(protocol, ns, protocolInterface);
            }
        }

        private void processSchema(CodeNamespace ns, Schema schema)
        {
            if (SchemaType.ENUM == schema.Type)
            {
                processEnum(schema, ns);
            }
            else if (SchemaType.FIXED == schema.Type)
            {
                processFixed(schema);
            }
            else if (SchemaType.RECORD == schema.Type)
            {
                processRecord(schema, ns);
            }
            else if (SchemaType.ERROR == schema.Type)
            {
                CodeTypeDeclaration errorRecord = processRecord(schema, ns);
            }
            else if (SchemaType.ARRAY == schema.Type)
            {
                processArray(schema);
            }
            else if (SchemaType.MAP == schema.Type)
            {
                procesMap(schema);
            }
            else if (SchemaType.UNION == schema.Type)
            {
                processUnion(schema);
            }
            else
            {
                throw new NotSupportedException("Schema Schema of \"" + schema.Type + "\" is not supported yet.");
            }
        }

        private void processFixed(Schema schema)
        {
            if (_SchemaToCodeTypeReferenceLookup.ContainsKey(schema))
                return;
            Schema fixedSchema = schema as FixedSchema;

            _SchemaToCodeTypeReferenceLookup.Add(schema, new CodeTypeReference(typeof(byte[])));
        }

        private void processUnion(Schema schema)
        {
            if (_SchemaToCodeTypeReferenceLookup.ContainsKey(schema))
                return;
            UnionSchema unionSchema = schema as UnionSchema;

            //TODO: This is wrong
            _SchemaToCodeTypeReferenceLookup.Add(schema, new CodeTypeReference(typeof(string)));
            //unionSchema.
        }

        private void procesMap(Schema schema)
        {
            if (_SchemaToCodeTypeReferenceLookup.ContainsKey(schema))
                return;

            MapSchema mapSchema = schema as MapSchema;

            CodeTypeReference typeRef = new CodeTypeReference("System.Collections.Dictionary");
            typeRef.TypeArguments.Add(new CodeTypeReference(typeof(string)));
            typeRef.TypeArguments.Add(new CodeTypeReference(typeof(string)));

            //CodeTypeReference valueType = getCodeTypeReference(mapSchema.Values);



//            typeRef.TypeArguments.Add(valueType);

            _SchemaToCodeTypeReferenceLookup.Add(schema, typeRef);
        }

        private void processArray(Schema schema)
        {
            if (_SchemaToCodeTypeReferenceLookup.ContainsKey(schema))
                return;

            ArraySchema arraySchema = schema as ArraySchema;




            CodeTypeReference arrayItemRef = getCodeTypeReference(arraySchema.ItemSchema);

            CodeTypeReference arrayRef = new CodeTypeReference(arrayItemRef, 1);

            _SchemaToCodeTypeReferenceLookup.Add(schema, arrayRef);
        }

        private CodeTypeDeclaration processRecord(Schema schema, CodeNamespace ns)
        {
            RecordSchema recordSchema = schema as RecordSchema;

            CodeNamespace recordNamespace = null;

            if (string.IsNullOrEmpty(recordSchema.Name.space) && null!=ns)
                recordNamespace = ns;
            else
                recordNamespace = addNamespace(recordSchema.Name.space);

            CodeTypeReference refRecord = new CodeTypeReference(recordSchema.Name.name);

            _SchemaToCodeTypeReferenceLookup.Add(schema, refRecord);

            CodeTypeDeclaration recordDeclare = new CodeTypeDeclaration(refRecord.BaseType);
            recordDeclare.Attributes = MemberAttributes.Public;
            recordDeclare.IsClass = true;
            recordDeclare.IsPartial = true;

            foreach (Field field in recordSchema.Fields)
            {
                if (SchemaType.NULL == field.Schema.Type)
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
                property.GetStatements.Add(new CodeMethodReturnStatement(fieldRef));
                property.SetStatements.Add(new CodeAssignStatement(fieldRef, new CodePropertySetValueReferenceExpression()));
                if (null != propertyComment)
                    property.Comments.Add(propertyComment);
                recordDeclare.Members.Add(property);
            }

            recordNamespace.Types.Add(recordDeclare);



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

            CodeNamespace addtoNs = null;

            if (null != enumschema.Name && !string.IsNullOrEmpty(enumschema.Name.space))
            {
                addtoNs = addNamespace(enumschema.Name.space);
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
                //processSchema(null, schema);
                //typeref = new CodeTypeReference(typeof(string));
            }
            return typeref;
        }
    }
}
