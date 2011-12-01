using System;
using System.Collections;
using System.Reflection;
using com.hessiannet.hessian.test;
using hessiancsharp.client;

namespace hessiancsharp.mobiletest
{
	/// <summary>
	/// Test - class for hessian compact - framework
	/// </summary>
	public class MobileTest
	{
		public MobileTest()
		{
		}

		public static void TestHessainCompact()
		{
			

			CHessianProxyFactory factory = new CHessianProxyFactory();
			//String url = "http://192.168.0.1:9090/resin-doc/protocols/tutorial/hessian-add/hessian/hessianDotNetTest";
			String url = "http://192.168.1.11:9090/resin-doc/protocols/csharphessian/hessian/hessianDotNetTest";
			CHessianMethodCaller methodCaller = new CHessianMethodCaller(factory, new Uri(url));
			try 
			{
				MethodInfo mInfo_1 = typeof(IHessianTest).GetMethod("testConcatString");
				object result = methodCaller.DoHessianMethodCall(new object[]{"Hallo ","Welt"},mInfo_1 );
				Console.WriteLine("Return value of method \"testConcatString\":" );
				Console.WriteLine(result);
				MethodInfo mInfo_2 = typeof(IHessianTest).GetMethod("testHashMap");
				string [] keys = new string[]{"Bauarbeiter","Jo!"};
				string [] values = new string[]{"Koennen wir das schaffen?","Wir schaffen das!"};
				Hashtable hashResult = (Hashtable)methodCaller.DoHessianMethodCall(new object[]{keys,values},mInfo_2 );
				IDictionaryEnumerator dict = hashResult.GetEnumerator();
				Console.WriteLine("Return value of method \"testHashMap\":" );
				while (dict.MoveNext())
				{
					Console.WriteLine(dict.Key.ToString() +" " + dict.Value.ToString() );
				}
				ParamObject pobject = (ParamObject)Activator.CreateInstance(typeof(ParamObject));
				pobject.setStringVar("Bauarbeiter, koennen wir das schaffen?");
				Hashtable hashTab = new Hashtable();
				hashTab.Add("Jo", " Wir schaffen das!");
				MethodInfo mInfo_3 = typeof(IHessianTest).GetMethod("testParamObject");
				ParamObject pObjResult = (ParamObject)methodCaller.DoHessianMethodCall(new object[]{pobject},mInfo_3 );
				Console.WriteLine("Return value of method \"testParamObject\":" );
				Console.WriteLine(pObjResult.getStringVar());
				Console.WriteLine(pObjResult.getHashVar()["Message"].ToString());
				
			} catch(Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			Console.ReadLine();
			
		}
	}
}
