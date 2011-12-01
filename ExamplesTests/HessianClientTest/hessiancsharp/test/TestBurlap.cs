using System;
using System.Collections.Generic;
using System.Text;
using burlapcsharp.client;
using burlapcsharp.io;
using burlap.test;

namespace hessiancsharp.test
{
    class TestBurlap
    {
        public static void testBurlap()
        {
            CBurlapProxyFactory factory = new CBurlapProxyFactory();


            String url = "http://www.caucho.com/burlap/test/basic";

            try
            {

                Basic test = (Basic)factory.Create(typeof (Basic), url);
                Console.WriteLine(test.hello());

                Console.ReadLine();

				

            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
                Console.ReadLine();
            }
        }
    }
}
