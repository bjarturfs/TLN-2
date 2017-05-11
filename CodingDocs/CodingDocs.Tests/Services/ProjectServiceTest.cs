using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CodingDocs.Services;

namespace CodingDocs.Tests.Services
{
    [TestClass]
    public class ProjectServiceTest
    {
        private ProjectService pservice;

        [TestInitialize]
        public void Initialize()
        {
            var mockDb = new MockDatabase();
            pservice = new ProjectService(mockDb);
        }

        [TestMethod]
        public void TestMethod1()
        {
            
        }
    }
}
