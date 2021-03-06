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
  ""schema"": ""HelloWorld"",

  ""types"": [
    {""name"": ""Greeting"", ""type"": ""record"", ""fields"": [
      {""name"": ""message"", ""type"": ""string""}]},
    {""name"": ""Curse"", ""type"": ""error"", ""fields"": [
      {""name"": ""message"", ""type"": ""string""}]}
  ],

  ""messages"": {
    ""hello"": {
      ""request"": [{""name"": ""greeting"", ""type"": ""Greeting"" }],
      ""response"": ""Greeting"",
      ""errors"": [""Curse""]
    }
  }
}", true),
  new ExampleProtocol(@" 
{""namespace"": ""org.apache.avro.test"",
 ""schema"": ""Simple"",

 ""types"": [
     {""name"": ""Kind"", ""type"": ""enum"", ""symbols"": [""FOO"",""BAR"",""BAZ""]},

     {""name"": ""MD5"", ""type"": ""fixed"", ""size"": 16},

     {""name"": ""TestRecord"", ""type"": ""record"",
      ""fields"": [
          {""name"": ""name"", ""type"": ""string"", ""order"": ""ignore""},
          {""name"": ""kind"", ""type"": ""Kind"", ""order"": ""descending""},
          {""name"": ""hash"", ""type"": ""MD5""}
      ]
     },

     {""name"": ""TestError"", ""type"": ""error"", ""fields"": [
         {""name"": ""message"", ""type"": ""string""}
      ]
     }

 ],

 ""messages"": {

     ""hello"": {
         ""request"": [{""name"": ""greeting"", ""type"": ""string""}],
         ""response"": ""string""
     },

     ""echo"": {
         ""request"": [{""name"": ""record"", ""type"": ""TestRecord""}],
         ""response"": ""TestRecord""
     },

     ""add"": {
         ""request"": [{""name"": ""arg1"", ""type"": ""int""}, {""name"": ""arg2"", ""type"": ""int""}],
         ""response"": ""int""
     },

     ""echoBytes"": {
         ""request"": [{""name"": ""data"", ""type"": ""bytes""}],
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
 ""schema"": ""TestNamespace"",

 ""types"": [
     {""name"": ""org.apache.avro.test.util.MD5"", ""type"": ""fixed"", ""size"": 16},
     {""name"": ""TestRecord"", ""type"": ""record"",
      ""fields"": [ {""name"": ""hash"", ""type"": ""org.apache.avro.test.util.MD5""} ]
     },
     {""name"": ""TestError"", ""namespace"": ""org.apache.avro.test.errors"",
      ""type"": ""error"", ""fields"": [ {""name"": ""message"", ""type"": ""string""} ]
     }
 ],

 ""messages"": {
     ""echo"": {
         ""request"": [{""name"": ""record"", ""type"": ""TestRecord""}],
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
 ""schema"": ""BulkData"",

 ""types"": [],

 ""messages"": {

     ""Read"": {
         ""request"": [],
         ""response"": ""bytes""
     },

     ""write"": {
         ""request"": [ {""name"": ""data"", ""type"": ""bytes""} ],
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
