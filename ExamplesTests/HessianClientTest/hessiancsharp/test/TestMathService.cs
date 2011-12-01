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
* Last change: 2004-11-20
* By LosAndros	
* Added comments.
******************************************************************************************************
*
*/

using System;
using hessiancsharp.client;

namespace hessiancsharp.test {
	/// <summary>
	/// Zusammenfassung für TestMathService.
	/// </summary>
	public class TestMathService {
		
		public static void testMath() {
			CHessianProxyFactory factory = new CHessianProxyFactory();

			//String url = "http://localhost:9090/resin-doc/protocols/tutorial/hessian-service/hessian/math";
			//String url = "http://localhost:2180/csharphessian/math";
			String url = "http://localhost/MathService/test2.hessian";
			try {
				IMathService math;
				math = (IMathService) factory.Create(typeof (IMathService), url);
				Console.WriteLine (math.add(3, 5));
				
				Console.WriteLine("3 + 5 = {0}",math.add(3, 5));
				Console.WriteLine("9 - 5 = {0}", math.sub(9, 5));
				Console.WriteLine("9 / 3 = {0}", math.div(9, 3));
				Console.WriteLine("9 * 3 = {0}", math.mul(9, 3));
				
				Console.WriteLine( math.testString("Hallo"));
				int[] ar = {2,3,5}; 
				Console.WriteLine( math.addArray(ar));
				
			
				for (int i = 0; i < 30; i++) {
					Console.WriteLine(i + " FOR" + i + "* 3 = " + math.mul(i, 3));
				}
				
				Console.ReadLine();
			} catch (Exception e) {
				Console.WriteLine(e.Message);
				Console.WriteLine(e.StackTrace);
				Console.ReadLine();
			}
		}
	}
}