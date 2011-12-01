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
using System.Collections;

namespace hessiancsharp.test
{
	/// <summary>
	/// Interface for specification of
	/// the Hessian-Test-Methods
	/// </summary>
	public interface IHessianTest
	{
		string testConcatString(string param1, string param2);
		bool testStringToBoolean(string param);
		long testStringToLong(string param);
		double testStringToDouble(string param);
		short testStringToShort(string param);
		int testStringToInt(string param);
		float testStringToFloat(string param);
		byte testStringToByte(String param);
		
		string testIntToString(int param);
		string testDoubleToString(double param);
		string testShortToString(short param);
		string testLongToString(long param);
		string testFloatToString(float param);
		string testBoolToString(bool param);
		string testByteToString(byte param);
		string testCharToString(char param);

		string[] testIntArrToString(int[] param);
		int[] testStringArrToInt(string[] param);


		string[] testDoubleArrToString(double[] param);
		double[] testStringArrToDouble(string[] param);
		string[] testLongArrToString(long[] param);
		long[] testStringArrToLong(string[] param);
		string[] testShortArrToString(short[] param);
		short[] testStringArrToShort(string[] param);
		string[] testFloatArrToString(float[] param);
		float[] testStringArrToFloat(string[] param);
		string[] testByteArrToString(byte[] param);
		byte[] testStringArrToByte(string[] param);
		string[] testBoolArrToString(bool[] param);
		bool[] testStringArrToBool(string[] param);
		string[] testCharArrToString(char[] param);
		char[] testStringArrToChar(string[] param);

		
		System.Collections.Hashtable testHashMap(string [] keys, string [] values);
		string testHashMapParam(System.Collections.Hashtable param);
		ParamObject testParamObject(ParamObject param);

		string testSendParamObject(ParamObject param);
		ParamObject testReceiveParamObject(String param);

		string testArrayListParam(ArrayList param);
		ArrayList testArrayList(String [] values);
		bool testList(IList values);
		DateTime testStringToDate(string param);
		string testDateToString(DateTime param);
		char testChar(char param);


	}
}
