using System;
using hessiancsharp.server;
using hessiancsharp.test;

namespace HessianNetTest.hessiancsharp.test
{
	/// <summary>
	/// Zusammenfassung für CMathService.
	/// </summary>
	public class CMathService:CHessianHandler, IMathService
	{
		public CMathService()
		{
			
		}

		public int add(int a, int b) {
			return a + b;
		}

		public int sub(int a, int b) 
		{

			return a - b;
		}

		public int mul(int a, int b) {
			return a * b;
		}

		public int div(int a, int b) {
			
			return a/b;
		}

		public int addArray(int[] a) {
			int result  = 0;
			foreach (int i in a) {
				result = result +i;
			}
			return result;
		}

		public string testString(string f) {
			return f+"Dimi";
		}


	}
}
