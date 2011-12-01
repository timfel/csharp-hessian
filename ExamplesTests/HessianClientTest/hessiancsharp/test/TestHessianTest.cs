/*
***************************************************************************************************** 
* HessianCharp - The .Net implementation of the Hessian Binary Web Service Protocol (www.caucho.com) 
* Copyright (C) 2004-2005  by D. Minich, V. Byelyenkiy, A. Voltmann
* http://www.hessiancsharp.com
*
* This program is free software; you can redistribute it and/or
* modify it under the terms of the GNU General Public License
* as published by the Free Software Foundation; either version 2
* of the License, or any later version.
*
* This program is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
* GNU General Public License for more details.
* 
* You can find the GNU General Public License here
* http://www.gnu.org/licenses/gpl.html
* or in the license.txt file in your source directory.
******************************************************************************************************   
* The GNU Public License does not permit the integration of the libraries in proprietary programs. 
* On request we'll also offer another licence for an appropriate royalty for integration in proprietary 
* (commercial) applications.
******************************************************************************************************  
* You can find all contact information on http://www.hessiancsharp.com	
******************************************************************************************************
*
*
******************************************************************************************************
* Last change: 2004-12-08
* By Dimi	
* Added Test for Stream
******************************************************************************************************
*
*/

using System;
using System.Collections;
using System.IO;
using System.Net;
using hessiancsharp.client;
using hessiancsharp.io;

namespace hessiancsharp.test
{
	/// <summary>
	/// Zusammenfassung für TestHessianTest.
	/// </summary>
	public class TestHessianTest
	{
		public static void testHessian() 
		{	
			CHessianProxyFactory factory = new CHessianProxyFactory();
			
			//String url = "http://localhost:9090/resin-doc/protocols/tutorial/hessian-add/hessian/hessianDotNetTest";
			String url = "http://localhost:9090/resin-doc/protocols/csharphessian/hessian/hessianDotNetTest";
			//String url = "http://localhost/MathService/test.hessian";
			//String url = "http://localhost/MathService/MyHandler.hessian";

            url = "http://localhost:8080/hessiantest/hessian/hessianDotNetTest";
			try 
			{
                /*
			
				#region TEST_INPUTSTREAM
				WebRequest webRequest =  WebRequest.Create(new Uri(url));
				webRequest.ContentType = "text/xml";
				webRequest.Method = "POST";
				MemoryStream memoryStream = new MemoryStream();
				CHessianOutput cHessianOutput = new CHessianOutput(memoryStream);

				cHessianOutput.StartCall("download");
				cHessianOutput.WriteString("C:/resin-3.0.8/webapps/resin-doc/protocols/csharphessian/WEB-INF/classes/HessianTest.java");
				cHessianOutput.CompleteCall();
				Stream sInStream = null;
				Stream sOutStream = null;

				try 
				{
					webRequest.ContentLength = memoryStream.ToArray().Length;
					sOutStream = webRequest.GetRequestStream();
					memoryStream.WriteTo(sOutStream);

					sOutStream.Flush();
					HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse();
					sInStream = webResponse.GetResponseStream();
					CHessianInput hessianInput = new CHessianInput(sInStream);
					hessianInput.StartReply();
					Stream is2 = hessianInput.ReadInputStream();
					FileStream fo = new FileStream("hallo_test.txt",FileMode.Create);					
					int b;
					while((b = is2.ReadByte())!= -1)
					{
						fo.WriteByte((byte)b);
					}
								
					hessianInput.CompleteReply();
					fo.Close();
					is2.Close();
					Console.WriteLine("Datei erfolgreich übertragen: hallo.txt");
					
				} 
				catch (Exception e) 
				{
					Console.WriteLine("Fehler "+ e.StackTrace);
				}
				finally 
				{
					if (sInStream != null) 
					{
						sInStream.Close();
					}
					if (sOutStream != null) 
					{
						sOutStream.Close();
					}
				}
                #endregion
                */
				

				IHessianTest test = (IHessianTest)factory.Create(typeof (IHessianTest), url);
				
				/*
				char shouldChar = new char();
				shouldChar = 'R';
				Console.WriteLine("ShouldChar"+shouldChar);            

				String recievedString = test.testCharToString(shouldChar);

				Console.WriteLine("ReceivedChar: " + recievedString);

               */ 
               
				//Console.WriteLine("ReceivedChar2: " + test.testChar('P'));
                //Tut nicht

                

                DateTime dt = DateTime.Today;
                string dtASString = test.testDateToString(dt);
                Console.WriteLine(dtASString);
                DateTime dt2 = test.testStringToDate("10.12.2004");
                Console.WriteLine(dt2.ToString());
            

                Console.WriteLine(test.testConcatString("Hallo ", "Welt"));				
                Console.WriteLine(test.testDoubleToString(1.2));
                Console.WriteLine(test.testStringToDouble("4.5"));
                Console.WriteLine(test.testStringToLong("-45675467"));
                Console.WriteLine(test.testStringToShort("5467"));
                Console.WriteLine(test.testFloatToString((float) 1.4));
                Console.WriteLine(test.testStringToFloat("1.89"));
                Console.WriteLine(test.testBoolToString(true));
                Console.WriteLine(test.testStringToBoolean("false"));
                Console.WriteLine(test.testStringToByte("7"));
                Console.WriteLine(test.testByteToString(5));

               

            //Integer Array Test:
            int[] intArr = {23,467};
            string[] stringArr = test.testIntArrToString(intArr);
            for (int i = 0; i < stringArr.Length; i++){
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
         

				

				Console.ReadLine();



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
