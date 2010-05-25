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
using Avro.CodeGen;
using NUnit.Framework;
using System.IO;
using System.CodeDom;
using System.CodeDom.Compiler;

namespace Avro.Test.CodeGen
{
    [TestFixture]
    public class AvroGenTests
    {


        /// <summary>
        /// Testcase is used to load simple.avpr
        /// </summary>
        [TestCase]
        public void Simple_avpr()
        {
            const string inputFile = "CodeGen/simple.avpr";
            string outputFile = Path.GetFullPath(inputFile);
            outputFile = Path.ChangeExtension(inputFile, ".cs");
            Protocol protocol = loadProtocolFromFile(inputFile);

            AvroGen generator = new AvroGen();
            generator.Protocols.Add(protocol);

            CodeCompileUnit cu = generate(generator);

            writeSource(outputFile, cu);
           
        }

        [TestCase]
        public void BulkData_avpr()
        {
            const string inputFile = "CodeGen/BulkData.avpr";
            string outputFile = Path.GetFullPath(inputFile);
            outputFile = Path.ChangeExtension(inputFile, ".cs");
            Protocol protocol = loadProtocolFromFile(inputFile);

            AvroGen generator = new AvroGen();
            generator.Protocols.Add(protocol);

            CodeCompileUnit cu = generate(generator);

            writeSource(outputFile, cu);
        }

        [TestCase]
        public void namespace_avpr()
        {
            const string inputFile = "CodeGen/namespace.avpr";
            string outputFile = Path.GetFullPath(inputFile);
            outputFile = Path.ChangeExtension(inputFile, ".cs");
            Protocol protocol = loadProtocolFromFile(inputFile);

            AvroGen generator = new AvroGen();
            generator.Protocols.Add(protocol);

            CodeCompileUnit cu = generate(generator);

            writeSource(outputFile, cu);
        }

        [TestCase]
        [Ignore]
        public void interop_avsc()
        {
            const string inputFile = "CodeGen/interop.avsc";
            string outputFile = Path.GetFullPath(inputFile);
            outputFile = Path.ChangeExtension(inputFile, ".cs");
            Schema schema = loadSchemaFromFile(inputFile);
            
            AvroGen generator = new AvroGen();
            generator.Types.Add(schema);

            CodeCompileUnit cu = generate(generator);

            writeSource(outputFile, cu);
        }

        [TestCase]
        public void GenerateDecoderInterface()
        {
            Type decoderType = typeof(Avro.BinaryDecoder);

            CodeTypeDeclaration typeDeclare = new CodeTypeDeclaration("Decoder");
            typeDeclare.IsInterface = true;
            typeDeclare.Attributes = MemberAttributes.Public;

            List<CodeMemberMethod> methods = new List<CodeMemberMethod>();

            foreach (System.Reflection.MethodInfo method in decoderType.GetMethods())
            {
                if (!method.Name.StartsWith("Read")&&!method.Name.StartsWith("Skip"))
                    continue;

                CodeMemberMethod genMethod = new CodeMemberMethod();
                genMethod.Name = method.Name;
                genMethod.ReturnType = new CodeTypeReference(method.ReturnType);

                foreach (System.Reflection.ParameterInfo parameter in method.GetParameters())
                {
                    CodeParameterDeclarationExpression parm = new CodeParameterDeclarationExpression(parameter.ParameterType, parameter.Name);
                    genMethod.Parameters.Add(parm);
                }

                methods.Add(genMethod);
            }
            methods.Sort(delegate(CodeMemberMethod a, CodeMemberMethod b) { return a.Name.CompareTo(b.Name); });
            typeDeclare.Members.AddRange(methods.ToArray());
            Microsoft.CSharp.CSharpCodeProvider csp = new Microsoft.CSharp.CSharpCodeProvider();
            System.CodeDom.Compiler.CodeGeneratorOptions options = new CodeGeneratorOptions();
            options.BracingStyle = "C";
            options.BlankLinesBetweenMembers = false;
            using (StringWriter writer = new StringWriter())
            {
                csp.GenerateCodeFromType(typeDeclare, writer, options);
                Console.WriteLine(writer);
            }


        }


        private static void writeSource(string outputFile, CodeCompileUnit cu)
        {
            Microsoft.CSharp.CSharpCodeProvider csp = new Microsoft.CSharp.CSharpCodeProvider();
            CodeGeneratorOptions options = new CodeGeneratorOptions();
            options.BracingStyle = "C";
            options.BlankLinesBetweenMembers = false;

            using (StreamWriter writer = new StreamWriter(outputFile, false))
            {
                csp.GenerateCodeFromCompileUnit(cu, writer, options);
            }
        }

        private static CodeCompileUnit generate(AvroGen generator)
        {
            CodeCompileUnit cu = generator.Generate();
            Assert.IsNotNull(cu);
            Assert.IsTrue(cu.Namespaces.Count > 0, "Some namespaces should have been generated.");
            return cu;
        }

        private static Schema loadSchemaFromFile(string inputFile)
        {
            string json = File.ReadAllText(inputFile, Encoding.UTF8);
            Schema schema = Schema.Parse(json);
            Assert.IsNotNull(schema, "Schema should not be null.");
            return schema;
        }

        private static Protocol loadProtocolFromFile(string inputFile)
        {
            string json = File.ReadAllText(inputFile, Encoding.UTF8);
            Protocol protocol = Protocol.Parse(json);
            Assert.IsNotNull(protocol, "Protocol should not be null.");
            return protocol;
        }
    }
}
