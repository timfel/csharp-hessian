using System;
using System.Collections;
using hessiancsharp.server;
using hessiancsharp.test;

namespace HessianNetTest
{
	/// <summary>
	/// Zusammenfassung für CHessianTest.
	/// </summary>
	public class CHessianTest:CHessianHandler, IHessianTest
	{
		public string testConcatString(string param1, string param2) {
			return param1 + param2;
		}

		public bool testStringToBoolean(string param) {
			
			return Convert.ToBoolean(param);
		}

		public long testStringToLong(string param) {
			return  Convert.ToInt64(param);
		}

		public double testStringToDouble(string param) {
			return  Convert.ToDouble(param);
		}

		public short testStringToShort(string param) {
			return  Convert.ToInt16(param);
		}

		public int testStringToInt(string param) {
			return  Convert.ToInt32(param);
		}

		public float testStringToFloat(string param) {
			return  Convert.ToSingle(param);
		}

		public byte testStringToByte(String param) {
			return  Convert.ToByte(param);
		}

		public string testIntToString(int param) {
			return  Convert.ToString(param);
		}

		public string testDoubleToString(double param) {
			return  Convert.ToString(param);
		}

		public string testShortToString(short param) {
			return  Convert.ToString(param);
		}

		public string testLongToString(long param) {
			return  Convert.ToString(param);
		}

		public string testFloatToString(float param) {
			return  Convert.ToString(param);
		}

		public string testBoolToString(bool param) {
			return  Convert.ToString(param);
		}

		public string testByteToString(byte param) {
			return  Convert.ToString(param);
		}

		public string testCharToString(char param) {
			return  Convert.ToString(param);
		}

		public string[] testIntArrToString(int[] param) {
			String[] result = new String[param.Length];
			for (int i = 0; i < param.Length; i++) 
			{
				result[i] = Convert.ToString(param[i]);
			}
		
			return result;
		}

		public int[] testStringArrToInt(string[] param) {
			int[] result = new int[param.Length];
			for (int i = 0; i < param.Length; i++) 
			{
				result[i] = Convert.ToInt32(param[i]);
			}
			return result;
		}

		public string[] testDoubleArrToString(double[] param) {
			String[] result = new String[param.Length];
			for (int i = 0; i < param.Length; i++) 
			{
				result[i] = Convert.ToString(param[i]);
			}
		
			return result;
		}

		public double[] testStringArrToDouble(string[] param) {
			double[] result = new double[param.Length];
			for (int i = 0; i < param.Length; i++) 
			{
				result[i] = Convert.ToDouble(param[i]);
			}
			return result;
		}

		public string[] testLongArrToString(long[] param) {
			String[] result = new String[param.Length];
			for (int i = 0; i < param.Length; i++) 
			{
				result[i] = Convert.ToString(param[i]);
			}
		
			return result;
		}

		public long[] testStringArrToLong(string[] param) {
			long[] result = new long[param.Length];
			for (int i = 0; i < param.Length; i++) 
			{
				result[i] = Convert.ToInt64(param[i]);
			}
			return result;
		}

		public string[] testShortArrToString(short[] param) {
			String[] result = new String[param.Length];
			for (int i = 0; i < param.Length; i++) 
			{
				result[i] = Convert.ToString(param[i]);
			}
		
			return result;
		}

		public short[] testStringArrToShort(string[] param) {
			short[] result = new short[param.Length];
			for (int i = 0; i < param.Length; i++) 
			{
				result[i] = Convert.ToInt16(param[i]);
			}
			return result;
		}

		public string[] testFloatArrToString(float[] param) {
			String[] result = new String[param.Length];
			for (int i = 0; i < param.Length; i++) 
			{
				result[i] = Convert.ToString(param[i]);
			}
		
			return result;
		}

		public float[] testStringArrToFloat(string[] param) {
			float[] result = new float[param.Length];
			for (int i = 0; i < param.Length; i++) 
			{
				result[i] = Convert.ToSingle(param[i]);
			}
			return result;
		}

		public string[] testByteArrToString(byte[] param) {
			String[] result = new String[param.Length];
			for (int i = 0; i < param.Length; i++) 
			{
				result[i] = Convert.ToString(param[i]);
			}
		
			return result;
		}

		public byte[] testStringArrToByte(string[] param) {
			byte[] result = new byte[param.Length];
			for (int i = 0; i < param.Length; i++) 
			{
				result[i] = Convert.ToByte(param[i]);
			
			}
			return result;
		}

		public string[] testBoolArrToString(bool[] param) {
			String[] result = new String[param.Length];
			for (int i = 0; i < param.Length; i++) 
			{
				result[i] = Convert.ToString(param[i]);
			}
		
			return result;
		}

		public bool[] testStringArrToBool(string[] param) {
			bool[] result = new bool[param.Length];
			for (int i = 0; i < param.Length; i++) 
			{
				result[i] = Convert.ToBoolean(param[i]);			
			}
			return result;
		}

		public string[] testCharArrToString(char[] param) {
			String[] result = new String[param.Length];
			for (int i = 0; i < param.Length; i++) 
			{
				result[i] = Convert.ToString(param[i]);
			}
		
			return result;
		}

		public char[] testStringArrToChar(string[] param) {
			char[] result = new char[param.Length];
			for (int i = 0; i < param.Length; i++) 
			{
				result[i] = param[i].ToCharArray()[0];
			}
			return result;
		}

		public Hashtable testHashMap(string[] keys, string[] values) {
			Hashtable hMap = new Hashtable();
			for(int i = 0; i<keys.Length; i++)
			{
				hMap.Add(keys[i], values[i]);
			}
			return hMap; 
		}

		public string testHashMapParam(Hashtable param) {
			string result = "";
			
			ICollection col = param.Keys;
			foreach(String key in col) 
			{
				result += key;
				result +=" ";
				result += param[key];
			}
			
			return result;
		}

		public ParamObject testParamObject(ParamObject param) {
			
			
			ParamObject result = new ParamObject();
			Hashtable test = new Hashtable();			
			if(param==null) 
			{
				result = new ParamObject();
				result.setStringVar("ParamObject was empty");
				test.Add("Message", "No Message");
			}
			else 
			{
				result.setStringVar("ParamObject was not empty");				
				test.Add("Message", (param.getStringVar()!=null) ? param.getStringVar():"No Message");
			}
			result.setHashVar(test);
		
			return result;
		}

		public string testSendParamObject(ParamObject param) {			
			return param.getStringVar();
		}

		public ParamObject testReceiveParamObject(String param) {
			ParamObject result = new ParamObject();
			result.setStringVar(param);
			return result;
		}

		public string testArrayListParam(ArrayList param) {
			string result = "";
			foreach(object s in param) {
				result += s.ToString();
				result +=" ";
			}
			
			return result;
		}

		public ArrayList testArrayList(String[] values) {
			ArrayList hArrList = new ArrayList();
			foreach(String s in values) {
				hArrList.Add(s);
			}
			
			return hArrList; 
		}

		public bool testList(IList values) {
			return values.Count == 0;
		}

		public DateTime testStringToDate(string param) {
			DateTime date = DateTime.Now;
		
			return date;
		}

		public string testDateToString(DateTime param) {
			return param.ToString();
		}

		public char testChar(char param) {
			return Convert.ToChar(param);
		}
	}
}
