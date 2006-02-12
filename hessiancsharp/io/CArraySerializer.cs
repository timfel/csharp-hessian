/*
***************************************************************************************************** 
* HessianCharp - The .Net implementation of the Hessian Binary Web Service Protocol (www.caucho.com) 
* Copyright (C) 2004-2005  by D. Minich, V. Byelyenkiy, A. Voltmann
* http://www.hessiancsharp.com
*
* This library is free software; you can redistribute it and/or
* modify it under the terms of the GNU Lesser General Public
* License as published by the Free Software Foundation; either
* version 2.1 of the License, or (at your option) any later version.
*
* This library is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
* Lesser General Public License for more details.
*
* You should have received a copy of the GNU Lesser General Public
* License along with this library; if not, write to the Free Software
* Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
* 
* You can find the GNU Lesser General Public here
* http://www.gnu.org/licenses/lgpl.html
* or in the license.txt file in your source directory.
******************************************************************************************************  
* You can find all contact information on http://www.hessiancsharp.com	
******************************************************************************************************
*
*
******************************************************************************************************
* Last change: 2005-08-14
* By Andre Voltmann	
* Licence added.
******************************************************************************************************
*/

#region NAMESPACES
using System;
#endregion


namespace hessiancsharp.io
{
	/// <summary>
	/// Serializing of arrays
	/// </summary>
	public class CArraySerializer: AbstractSerializer
	{
		#region PUBLIC_METHODS
		/// <summary>
		/// Writes array object
		/// </summary>
		/// <param name="objArrayToWrite">Array - Instance to write</param>
		/// <param name="abstractHessianOutput">HessianOutput-Instance</param>
		public override void  WriteObject(Object objArrayToWrite, AbstractHessianOutput abstractHessianOutput)
		{
			if (abstractHessianOutput.AddRef(objArrayToWrite))
				return ;
			
			System.Object[] array = (Object[]) objArrayToWrite;
			
			abstractHessianOutput.WriteListBegin(array.Length, getArrayType(objArrayToWrite.GetType()));
			
			for (int i = 0; i < array.Length; i++)
				abstractHessianOutput.WriteObject(array[i]);
			
			abstractHessianOutput.WriteListEnd();
		}
		#endregion
		
		#region PRIVATE_METHODS
		/// <summary>
		/// Returns the type name for a array
		/// </summary>
		/// <param name="type">Array type</param>
		/// <returns>type name for a array</returns>
		private string getArrayType(Type type)
		{
			if (type.IsArray)
				return '[' + getArrayType(type.GetElementType());
			
			String strTypeName = type.FullName;
			
			if (strTypeName.Equals("System.String"))
				return "string";
			else if (strTypeName.Equals("System.Object"))
				return "object";            
			else if (strTypeName.Equals("System.DateTime"))
				return "date";
			else
				return strTypeName;
		}
		#endregion
	}
}
