using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace Avro.Test
{
    [TestFixture]
    public class TestProtocol
    {
        [Test]
        public void Test()
        {
            ExampleProtocol[] EXAMPLES = new ExampleProtocol[]{
  new ExampleProtocol(@"{
  ""namespace"": ""com.acme"",
  ""protocol"": ""HelloWorld"",

  ""types"": [
    {""name"": ""Greeting"", ""schema"": ""record"", ""fields"": [
      {""name"": ""message"", ""schema"": ""string""}]},
    {""name"": ""Curse"", ""schema"": ""error"", ""fields"": [
      {""name"": ""message"", ""schema"": ""string""}]}
  ],

  ""messages"": {
    ""hello"": {
      ""request"": [{""name"": ""greeting"", ""schema"": ""Greeting"" }],
      ""response"": ""Greeting"",
      ""errors"": [""Curse""]
    }
  }
}", true),
  new ExampleProtocol(@" 
{""namespace"": ""org.apache.avro.test"",
 ""protocol"": ""Simple"",

 ""types"": [
     {""name"": ""Kind"", ""schema"": ""enum"", ""symbols"": [""FOO"",""BAR"",""BAZ""]},

     {""name"": ""MD5"", ""schema"": ""fixed"", ""size"": 16},

     {""name"": ""TestRecord"", ""schema"": ""record"",
      ""fields"": [
          {""name"": ""name"", ""schema"": ""string"", ""order"": ""ignore""},
          {""name"": ""kind"", ""schema"": ""Kind"", ""order"": ""descending""},
          {""name"": ""hash"", ""schema"": ""MD5""}
      ]
     },

     {""name"": ""TestError"", ""schema"": ""error"", ""fields"": [
         {""name"": ""message"", ""schema"": ""string""}
      ]
     }

 ],

 ""messages"": {

     ""hello"": {
         ""request"": [{""name"": ""greeting"", ""schema"": ""string""}],
         ""response"": ""string""
     },

     ""echo"": {
         ""request"": [{""name"": ""record"", ""schema"": ""TestRecord""}],
         ""response"": ""TestRecord""
     },

     ""add"": {
         ""request"": [{""name"": ""arg1"", ""schema"": ""int""}, {""name"": ""arg2"", ""schema"": ""int""}],
         ""response"": ""int""
     },

     ""echoBytes"": {
         ""request"": [{""name"": ""data"", ""schema"": ""bytes""}],
         ""response"": ""bytes""
     },

     ""error"": {
         ""request"": [],
         ""response"": ""null"",
         ""errors"": [""TestError""]
     }
 }

}
    """, true),

       new ExampleProtocol(@"{""namespace"": ""org.apache.avro.test.namespace"",
 ""protocol"": ""TestNamespace"",

 ""types"": [
     {""name"": ""org.apache.avro.test.util.MD5"", ""schema"": ""fixed"", ""size"": 16},
     {""name"": ""TestRecord"", ""schema"": ""record"",
      ""fields"": [ {""name"": ""hash"", ""schema"": ""org.apache.avro.test.util.MD5""} ]
     },
     {""name"": ""TestError"", ""namespace"": ""org.apache.avro.test.errors"",
      ""schema"": ""error"", ""fields"": [ {""name"": ""message"", ""schema"": ""string""} ]
     }
 ],

 ""messages"": {
     ""echo"": {
         ""request"": [{""name"": ""record"", ""schema"": ""TestRecord""}],
         ""response"": ""TestRecord""
     },

     ""error"": {
         ""request"": [],
         ""response"": ""null"",
         ""errors"": [""org.apache.avro.test.errors.TestError""]
     }

 }

}", true), 

  new ExampleProtocol(@"{""namespace"": ""org.apache.avro.test"",
 ""protocol"": ""BulkData"",

 ""types"": [],

 ""messages"": {

     ""Read"": {
         ""request"": [],
         ""response"": ""bytes""
     },

     ""write"": {
         ""request"": [ {""name"": ""data"", ""schema"": ""bytes""} ],
         ""response"": ""null""
     }

 }

}", true)
            };


            testExamples(EXAMPLES);

        }

        private void testExamples(ExampleProtocol[] EXAMPLES)
        {
            foreach (ExampleProtocol example in EXAMPLES)
            {
                testExample(example);
            }
        }

        private void testExample(ExampleProtocol example)
        {
            try
            {
                Protocol protocol = Protocol.Parse(example.Protocol);
                Assert.IsTrue(example.Valid);

                string json = protocol.ToString();
                Protocol protocol2 = Protocol.Parse(json);

                Assert.AreEqual(protocol, protocol2);

            }
            catch (Exception ex)
            {
                Assert.IsFalse(example.Valid, ex.ToString());
            }
        }
    }
}
