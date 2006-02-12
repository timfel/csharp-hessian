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
using System.IO;
using System.Reflection;
#endregion

namespace hessiancsharp.io
{
	/// <summary>
	/// Serializing a string valued object.
	/// </summary>
	public class CStringValueDeserializer: AbstractDeserializer
	{
		#region CLASS_FIELDS
		/// <summary>
		/// String valued object type
		/// </summary>
		private Type m_type;
		/// <summary>
		/// Constructor instance (with one string argument)
		/// </summary>
		private ConstructorInfo m_constructor;
		#endregion

		#region PROPERTIES
		/// <summary>
		/// Gets the type of the string valued object
		/// </summary>
		public Type Type
		{
			get
			{
				return m_type;
			}
			
		}
		#endregion
				
		#region CONSTRUCTORS
		/// <summary>
		/// Const
		/// </summary>ructor
		/// <param name="type">Type of the objects for serialiazation</param>
		public CStringValueDeserializer(Type type)
		{
			m_type = type;
			m_constructor = m_type.GetConstructor(new Type[]{typeof(String)});
		}
		
		#endregion
		#region PUBLIC_METHODS
		/// <summary>
		/// Reads string valued object
		/// </summary>
		/// <param name="abstractHessianInput">HessianInput Instance</param>
		/// <returns>Read string valued object</returns>
		public override object ReadMap(AbstractHessianInput abstractHessianInput)
		{
			String strInitValue = null;
			
			while (!abstractHessianInput.IsEnd())
			{
				string strKey = abstractHessianInput.ReadString();
				string strValue = abstractHessianInput.ReadString();
				
				if (strKey.Equals("value"))
					strInitValue = strValue;
			}
			
			abstractHessianInput.ReadMapEnd();
			
			if (strInitValue == null)
				throw new IOException(m_type.FullName + " expects name.");
			
			try
			{
				return m_constructor.Invoke(new Object[]{strInitValue});
			}
			catch (Exception e)
			{
				throw new IOException(e.ToString());
			}
		}

		#endregion
	}
}
