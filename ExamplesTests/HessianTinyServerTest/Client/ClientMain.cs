using System;
using System.Collections;
using hessiancsharp.client;
using hessiancsharp.test;

namespace Client
{
	/// <summary>
	/// Zusammenfassung für Class1.
	/// </summary>
	class ClientMain
	{
		/// <summary>
		/// Der Haupteinstiegspunkt für die Anwendung.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			CHessianProxyFactory factory = new   CHessianProxyFactory("dimi","dimi");						
			String url = "http://localhost:5667/test/hessiantest.hessian";
			IHessianTest test = (IHessianTest)factory.Create(typeof (IHessianTest), url);
			DateTime d1 = System.DateTime.Now;
			for(int i = 0; i < 2; i++) {
				//ClientMain.test(test);
                ClientMain.testWithConsole(test);
				Console.WriteLine(i);
			}	
			TimeSpan time = System.DateTime.Now - d1;
			
			Console.WriteLine("Time to execute: " + time);
			Console.ReadLine();

		}

		private static void testWithConsole(IHessianTest test) {			
			try 
			{
				
				char shouldChar = new char();
				shouldChar = 'R';
				Console.WriteLine("ShouldChar"+shouldChar);            

				String recievedString = test.testCharToString(shouldChar);

				Console.WriteLine("ReceivedChar: " + recievedString);
				char t = test.testChar('P');
				Console.WriteLine("ReceivedChar2: " + t);
			


				DateTime dt = DateTime.Today;
				string dtASString = test.testDateToString(dt);
				Console.WriteLine(dtASString);
				DateTime dt2 = test.testStringToDate("10.12.2004");
				Console.WriteLine(dt2.ToString());
				
				string s1 = test.testConcatString("Hallo ", "Welt");				
				Console.WriteLine(s1);				
				string s2 = test.testDoubleToString(1.2);
				Console.WriteLine(s2);
				double d1 = test.testStringToDouble("4.5");
				Console.WriteLine(d1);
                long l1 = test.testStringToLong("-45675467");				
				Console.WriteLine(l1);
				short sh1 = test.testStringToShort("5467");				
				Console.WriteLine(sh1);
				string s3 = test.testFloatToString((float) 1.4);
				Console.WriteLine(s3);				
				float f1 = test.testStringToFloat("1.89");
				Console.WriteLine(f1);
				string s4 = test.testBoolToString(true);
				Console.WriteLine(s4);
				bool b1 = test.testStringToBoolean("false");
				Console.WriteLine(b1);
				byte by1 = test.testStringToByte("7");
				Console.WriteLine(by1);
				string s5 = test.testByteToString(5);
				Console.WriteLine(s5);
				
				

				//Integer Array Test:
				int[] intArr = {23,467};
				string[] stringArr = test.testIntArrToString(intArr);
				
				for (int i = 0; i < stringArr.Length; i++)
				{
					Console.WriteLine(stringArr[i]);
				}

				string[] stringArr2 = {"788","343"};
				int[] intArr2 = test.testStringArrToInt(stringArr2);
				for (int i = 0; i < intArr2.Length; i++)
				{
					Console.WriteLine(intArr2[i]);
				}

				//Double Arrray Test:
				double[] doubleArr = {23.467, 78.3 };
				stringArr = test.testDoubleArrToString(doubleArr);
				for (int i = 0; i < stringArr.Length; i++)
				{
					Console.WriteLine(stringArr[i]);
				}

				string[] stringArrDouble = {"788.56","343.678"};
				double[] doubleArr2 = test.testStringArrToDouble(stringArrDouble);
				for (int i = 0; i < doubleArr2.Length; i++)
				{
					Console.WriteLine(doubleArr2[i]);
				}

				//Float Arrray Test:
				float[] floatArr = {(float)22.47, (float)3.3 };
				stringArr = test.testFloatArrToString(floatArr);
				for (int i = 0; i < stringArr.Length; i++)
				{
					Console.WriteLine(stringArr[i]);
				}

				string[] stringArrFloat = {"88.56","4.678"};
				float[] floatArr2 = test.testStringArrToFloat(stringArrFloat);
				for (int i = 0; i < floatArr2.Length; i++)
				{
					Console.WriteLine(floatArr2[i]);
				}

				//Short Arrray Test:
				short[] shortArr = {56, 3 };
				stringArr = test.testShortArrToString(shortArr);
				for (int i = 0; i < stringArr.Length; i++)
				{
					Console.WriteLine(stringArr[i]);
				}

				string[] stringArrShort = {"7","38"};
				short[] shortArr2 = test.testStringArrToShort(stringArrShort);
				for (int i = 0; i < shortArr2.Length; i++)
				{
					Console.WriteLine(shortArr2[i]);
				}

				//Char Arrray Test:
				char[] charArr = {'c', 'd' };
				stringArr = test.testCharArrToString(charArr);
				for (int i = 0; i < stringArr.Length; i++)
				{
					Console.WriteLine(stringArr[i]);
				}

				string[] stringArrChar = {"l","w"};
				char[] charArr2 = test.testStringArrToChar(stringArrChar);
				for (int i = 0; i < charArr2.Length; i++)
				{
					Console.WriteLine(charArr2[i]);
				}

				//Long Arrray Test:
				long[] longArr = {56323, 3232323 };
				stringArr = test.testLongArrToString(longArr);
				for (int i = 0; i < stringArr.Length; i++)
				{
					Console.WriteLine(stringArr[i]);
				}

				string[] stringArrLong = {"111117","2222238"};
				long[] longArr2 = test.testStringArrToLong(stringArrLong);
				for (int i = 0; i < longArr2.Length; i++)
				{
					Console.WriteLine(longArr2[i]);
				}
				
				//Byte Arrray Test:
				byte[] byteArr = {5, 3 };
				stringArr = test.testByteArrToString(byteArr);
				for (int i = 0; i < stringArr.Length; i++)
				{
					Console.WriteLine(stringArr[i]);
				}

				string[] stringArrByte = {"7","3"};
				byte[] byteArr2 = test.testStringArrToByte(stringArrByte);
				for (int i = 0; i < byteArr2.Length; i++)
				{
					Console.WriteLine(byteArr2[i]);
				}

				//Bool Arrray Test:
				bool[] boolArr = {true, false };
				stringArr = test.testBoolArrToString(boolArr);
				for (int i = 0; i < stringArr.Length; i++)
				{
					Console.WriteLine(stringArr[i]);
				}

				string[] stringArrBool = {"true","false"};
				bool[] boolArr2 = test.testStringArrToBool(stringArrBool);
				for (int i = 0; i < boolArr2.Length; i++)
				{
					Console.WriteLine(boolArr2[i]);
				}

				Console.WriteLine("Test the hashtable return value");
				System.Collections.Hashtable testHash = test.testHashMap(new string[]{"Hallo"},new string []{"Welt"});
				System.Collections.IDictionaryEnumerator enumer = testHash.GetEnumerator();
				while(enumer.MoveNext())
				{
					Console.WriteLine(enumer.Key.ToString() +" " + enumer.Value.ToString()); 
				}
				Console.WriteLine("Test the hashtable param");
				Console.WriteLine(test.testHashMapParam(testHash));
				

				ArrayList arrList = test.testArrayList(new string[]{"Hallo"," Dimi"});				
				Console.WriteLine(test.testArrayListParam(arrList));
				

				Console.WriteLine("Test Object");
				
				ParamObject testPObject = new ParamObject();
				testPObject.setStringVar("Test Test");
				
				Console.WriteLine(test.testSendParamObject(testPObject));
				Console.WriteLine(test.testReceiveParamObject("REUTLINGEN").getStringVar());
				
				

				ParamObject testPObject2 = test.testParamObject(testPObject);
				
				Console.WriteLine(testPObject2.getStringVar());

				Hashtable h = testPObject2.getHashVar();
				
				Console.WriteLine(testPObject2.getHashVar()["Message"].ToString());
				

			} 
			catch (Exception e) 
			{
				Console.WriteLine(e.Message);
				Console.WriteLine(e.StackTrace);
				Console.ReadLine();
			}
		}

		private static void test(IHessianTest test) 
		{			
			try 
			{
				
				char shouldChar = new char();
				shouldChar = 'R';
				//Console.WriteLine("ShouldChar"+shouldChar);            

				String recievedString = test.testCharToString(shouldChar);

				//Console.WriteLine("ReceivedChar: " + recievedString);
				char t = test.testChar('P');
				//Console.WriteLine("ReceivedChar2: " + t);
			


				DateTime dt = DateTime.Today;
				string dtASString = test.testDateToString(dt);
				//Console.WriteLine(dtASString);
				DateTime dt2 = test.testStringToDate("10.12.2004");
				//Console.WriteLine(dt2.ToString());
				
				string s1 = test.testConcatString("Hallo ", "Welt");				
				//Console.WriteLine(s1);				
				string s2 = test.testDoubleToString(1.2);
				//Console.WriteLine(s2);
				double d1 = test.testStringToDouble("4.5");
				//Console.WriteLine(d1);
				long l1 = test.testStringToLong("-45675467");				
				//Console.WriteLine(l1);
				short sh1 = test.testStringToShort("5467");				
				//Console.WriteLine(sh1);
				string s3 = test.testFloatToString((float) 1.4);
				//Console.WriteLine(s3);				
				float f1 = test.testStringToFloat("1.89");
				//Console.WriteLine(f1);
				string s4 = test.testBoolToString(true);
				//Console.WriteLine(s4);
				bool b1 = test.testStringToBoolean("false");
				//Console.WriteLine(b1);
				byte by1 = test.testStringToByte("7");
				//Console.WriteLine(by1);
				string s5 = test.testByteToString(5);
				//Console.WriteLine(s5);
				
				

				//Integer Array Test:
				int[] intArr = {23,467};
				string[] stringArr = test.testIntArrToString(intArr);
				/*
				for (int i = 0; i < stringArr.Length; i++)
				{
					Console.WriteLine(stringArr[i]);
				}
				*/
				string[] stringArr2 = {"788","343"};
				int[] intArr2 = test.testStringArrToInt(stringArr2);
				/*
				for (int i = 0; i < intArr2.Length; i++)
				{
					Console.WriteLine(intArr2[i]);
				}
				*/

				//Double Arrray Test:
				double[] doubleArr = {23.467, 78.3 };
				stringArr = test.testDoubleArrToString(doubleArr);
				/*
				for (int i = 0; i < stringArr.Length; i++)
				{
					Console.WriteLine(stringArr[i]);
				}
				*/
				string[] stringArrDouble = {"788.56","343.678"};
				double[] doubleArr2 = test.testStringArrToDouble(stringArrDouble);
				/*
				for (int i = 0; i < doubleArr2.Length; i++)
				{
					Console.WriteLine(doubleArr2[i]);
				}
				*/

				//Float Arrray Test:
				float[] floatArr = {(float)22.47, (float)3.3 };
				stringArr = test.testFloatArrToString(floatArr);
				/*
				for (int i = 0; i < stringArr.Length; i++)
				{
					Console.WriteLine(stringArr[i]);
				}
				*/

				string[] stringArrFloat = {"88.56","4.678"};
				float[] floatArr2 = test.testStringArrToFloat(stringArrFloat);
				/*
				for (int i = 0; i < floatArr2.Length; i++)
				{
					Console.WriteLine(floatArr2[i]);
				}
				*/

				//Short Arrray Test:
				short[] shortArr = {56, 3 };
				stringArr = test.testShortArrToString(shortArr);
				/*
				for (int i = 0; i < stringArr.Length; i++)
				{
					Console.WriteLine(stringArr[i]);
				}
				*/

				string[] stringArrShort = {"7","38"};
				short[] shortArr2 = test.testStringArrToShort(stringArrShort);
				/*
				for (int i = 0; i < shortArr2.Length; i++)
				{
					Console.WriteLine(shortArr2[i]);
				}
				*/

				//Char Arrray Test:
				char[] charArr = {'c', 'd' };
				stringArr = test.testCharArrToString(charArr);
				/*
				for (int i = 0; i < stringArr.Length; i++)
				{
					Console.WriteLine(stringArr[i]);
				}
				*/

				string[] stringArrChar = {"l","w"};
				char[] charArr2 = test.testStringArrToChar(stringArrChar);
				/*
				for (int i = 0; i < charArr2.Length; i++)
				{
					Console.WriteLine(charArr2[i]);
				}
				*/
				//Long Arrray Test:
				long[] longArr = {56323, 3232323 };
				stringArr = test.testLongArrToString(longArr);
				/*
				for (int i = 0; i < stringArr.Length; i++)
				{
					Console.WriteLine(stringArr[i]);
				}
				*/

				string[] stringArrLong = {"111117","2222238"};
				long[] longArr2 = test.testStringArrToLong(stringArrLong);
				/*
				for (int i = 0; i < longArr2.Length; i++)
				{
					Console.WriteLine(longArr2[i]);
				}
				*/
				
				//Byte Arrray Test:
				byte[] byteArr = {5, 3 };
				stringArr = test.testByteArrToString(byteArr);
				/*
				for (int i = 0; i < stringArr.Length; i++)
				{
					Console.WriteLine(stringArr[i]);
				}
				*/

				string[] stringArrByte = {"7","3"};
				byte[] byteArr2 = test.testStringArrToByte(stringArrByte);
				/*
				for (int i = 0; i < byteArr2.Length; i++)
				{
					Console.WriteLine(byteArr2[i]);
				}
				*/

				//Bool Arrray Test:
				bool[] boolArr = {true, false };
				stringArr = test.testBoolArrToString(boolArr);
				/*
				for (int i = 0; i < stringArr.Length; i++)
				{
					Console.WriteLine(stringArr[i]);
				}
				*/

				string[] stringArrBool = {"true","false"};
				bool[] boolArr2 = test.testStringArrToBool(stringArrBool);
				/*
				for (int i = 0; i < boolArr2.Length; i++)
				{
					Console.WriteLine(boolArr2[i]);
				}
				*/

				//Console.WriteLine("Test the hashtable return value");
				System.Collections.Hashtable testHash = test.testHashMap(new string[]{"Hallo"},new string []{"Welt"});
				System.Collections.IDictionaryEnumerator enumer = testHash.GetEnumerator();
				/*
				while(enumer.MoveNext())
				{
					Console.WriteLine(enumer.Key.ToString() +" " + enumer.Value.ToString()); 
				}
				*/
				//Console.WriteLine("Test the hashtable param");
				string s6 = test.testHashMapParam(testHash);
				//Console.WriteLine(s6);
				

				ArrayList arrList = test.testArrayList(new string[]{"Hallo"," Dimi"});	
				s6 = test.testArrayListParam(arrList);
				//Console.WriteLine(s6);
				

				//Console.WriteLine("Test Object");
				
				ParamObject testPObject = new ParamObject();
				testPObject.setStringVar("Test Test");
				s6 = test.testSendParamObject(testPObject);
				//Console.WriteLine(s6);
				s6 = test.testReceiveParamObject("REUTLINGEN").getStringVar();
				//Console.WriteLine(s6);
				
				

				ParamObject testPObject2 = test.testParamObject(testPObject);
				
				//Console.WriteLine(testPObject2.getStringVar());

				Hashtable h = testPObject2.getHashVar();
				
				//Console.WriteLine(testPObject2.getHashVar()["Message"].ToString());
				

			} 
			catch (Exception e) 
			{
				Console.WriteLine(e.Message);
				Console.WriteLine(e.StackTrace);
				Console.ReadLine();
			}
		}

	}
}
