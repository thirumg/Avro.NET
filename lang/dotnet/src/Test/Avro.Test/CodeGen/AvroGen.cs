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
        public void TestSimple_avpr()
        {
            string inputFile = "CodeGen/simple.avpr";
            string outputFile = Path.GetFullPath(inputFile);
            outputFile = Path.ChangeExtension(inputFile, ".cs");
            Protocol protocol = loadProtocolFromFile(inputFile);


            AvroGen generator = new AvroGen();
            generator.Protocols.Add(protocol);

            CodeCompileUnit cu = generator.Generate();
            Assert.IsNotNull(cu);
            Assert.IsTrue(cu.Namespaces.Count > 0, "Some namespaces should have been generated.");

            Microsoft.CSharp.CSharpCodeProvider csp = new Microsoft.CSharp.CSharpCodeProvider();
            CodeGeneratorOptions options = new CodeGeneratorOptions();
            options.BracingStyle = "C";
            options.BlankLinesBetweenMembers = false;

            using (StreamWriter writer = new StreamWriter(outputFile, false))
            {
                csp.GenerateCodeFromCompileUnit(cu, writer, options);
            }
           
        }

        private static Protocol loadProtocolFromFile(string inputFile)
        {
            string json = File.ReadAllText(inputFile, Encoding.UTF8);
            Protocol protocol = Protocol.Parse(json);
            Assert.IsNotNull(protocol);
            return protocol;
        }
    }
}
