using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace Avro.Test
{
    [SetUpFixture]
    public class FixtureSetup
    {
        [SetUp]
        public void SetupLogging()
        {
            Logger.ConfigureForUnitTesting();
        }
    }
}
