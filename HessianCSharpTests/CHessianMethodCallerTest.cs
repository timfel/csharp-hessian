using hessiancsharp.client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Reflection;

namespace HessianCSharpTests
{
    
    
    /// <summary>
    ///This is a test class for CHessianMethodCallerTest and is intended
    ///to contain all CHessianMethodCallerTest Unit Tests
    ///</summary>
    [TestClass()]
    public class CHessianMethodCallerTest
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
        #endregion


        /// <summary>
        ///A test for DoHessianMethodCall
        ///</summary>
        [TestMethod()]
        public void DoHessianMethodCallTest()
        {
            CHessianProxyFactory hessianProxyFactory = null; // TODO: Initialize to an appropriate value
            Uri uri = null; // TODO: Initialize to an appropriate value
            CHessianMethodCaller target = new CHessianMethodCaller(hessianProxyFactory, uri); // TODO: Initialize to an appropriate value
            object[] arrMethodArgs = null; // TODO: Initialize to an appropriate value
            MethodInfo methodInfo = null; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = target.DoHessianMethodCall(arrMethodArgs, methodInfo);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }
    }
}
