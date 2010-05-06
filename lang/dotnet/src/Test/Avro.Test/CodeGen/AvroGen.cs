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
