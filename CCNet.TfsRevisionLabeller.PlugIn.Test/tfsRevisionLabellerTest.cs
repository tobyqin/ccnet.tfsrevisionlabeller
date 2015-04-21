using Microsoft.VisualStudio.TestTools.UnitTesting;
using ThoughtWorks.CruiseControl.Core;

namespace ccnet.tfsrevisionlabeller.plugin.Test
{
    /// <summary>
    ///This is a test class for tfsRevisionLabellerTest and is intended
    ///to contain all tfsRevisionLabellerTest Unit Tests
    ///</summary>
    [TestClass()]
    public class tfsRevisionLabellerTest
    {
        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes

        //
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //

        #endregion Additional test attributes

        /// <summary>
        ///A test for Generate
        ///</summary>
        [TestMethod()]
        public void GenerateVersion_With_Provided_Build_Number()
        {
            TfsRevisionLabeller target = new TfsRevisionLabeller(); // TODO: Initialize to an appropriate value
            IIntegrationResult integrationResult = new IntegrationResult(); // TODO: Initialize to an appropriate value

            target.Major = 3;
            target.Minor = 1;
            target.Build = 0;
            target.Prefix = "Prod-";
            target.ProjectPath = @"$/";
            target.Server = "http://dev:8080";
            target.Domain = "cn";
            target.Username = "test";
            target.Password = "test";
            string expected = "Prod-3.1.1.1";
            string actual;
            actual = target.Generate(integrationResult);
            Assert.AreEqual(expected, actual);
            TestContext.WriteLine("Actual Build Numberb: " + actual);
        }

        /// <summary>
        ///A test for Generate
        ///</summary>
        [TestMethod()]
        public void GenerateVersion_With_Fake_Build_Number()
        {
            TfsRevisionLabeller target = new TfsRevisionLabeller(); // TODO: Initialize to an appropriate value
            IIntegrationResult integrationResult = new IntegrationResult(); // TODO: Initialize to an appropriate value

            target.Major = 3;
            target.Minor = 1;
            target.Build = 0;
            target.Prefix = "Prod-";
            target.ProjectPath = @"$/Error";
            target.Server = "http://tfs.bad";
            target.Domain = "no";
            target.Username = "no";
            target.Password = "no";
            string expected = "Prod-3.1.0.1";
            string actual;
            actual = target.Generate(integrationResult);
            Assert.AreEqual(expected, actual);
            TestContext.WriteLine("Actual Build Numberb: " + actual);
        }
    }
}