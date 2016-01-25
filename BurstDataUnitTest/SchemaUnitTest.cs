using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Burst;
using Burst.Data;
using Burst.Data.Schema;

namespace BurstDataUnitTest
{
    [TestClass]
    public class SchemaUnitTest
    {
        [TestMethod]
        public void SchemaTestMethod()
        {
            var pms = new NameValueList();
            pms.Add("");
            var schema = new DbSchema(DbProvider.Initialize(pms));
        }
    }
}
