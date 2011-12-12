using hessiancsharp.client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using org.meet4xmas.wire;

namespace HessianCSharpTests
{   
    /// <summary>
    ///This is a test class for CHessianProxyFactoryTest and is intended
    ///to contain all CHessianProxyFactoryTest Unit Tests
    ///</summary>
    [TestClass()]
    public class CHessianProxyFactoryTest
    {
        private const string ApiUrl = "http://tessi.fornax.uberspace.de/xmas/1/";

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
        #endregion


        /// <summary>
        ///A test for Create
        ///</summary>
        [TestMethod()]
        public void CreateTest()
        {
            CHessianProxyFactory target = new CHessianProxyFactory();
            Type type = typeof(IServiceAPI);
            String url = ApiUrl;
            object actual = null;
            IServiceAPI test = (IServiceAPI)target.Create(type, url);

            actual = test.registerAccount("Tim" + DateTime.Now.ToString());

            Assert.IsInstanceOfType(actual, typeof(Response));
        }
    }
}
