/*
***************************************************************************************************** 
* HessianCharp - The .Net implementation of the Hessian Binary Web Service Protocol (www.caucho.com) 
* Copyright (C) 2004-2005  by D. Minich, V. Byelyenkiy, A. Voltmann
* http://www.hessiancsharp.org
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
* You can find all contact information on http://www.hessiancsharp.org
******************************************************************************************************
*
*
******************************************************************************************************
* Last change: 2007-06-05
* By Matthias Wuttke
* First version
******************************************************************************************************
*/

#region NAMESPACES
using System;
using System.Collections;
using System.Threading;
#endregion

namespace hessiancsharp.io
{
	/// <summary>
	/// Deserializing of Maps
	/// </summary>
	public class CEnumDeserializer : AbstractDeserializer 
	{
		#region CLASS_FIELDS
		/// <summary>
		/// Type of enum
		/// </summary>
		private Type e_type = null;
		#endregion
  
		#region CONSTRUCTORS
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="type">Type of enum</param>
		public CEnumDeserializer(Type type)
		{
			this.e_type = type;
		}
		#endregion
  
		#region PUBLIC_METHODS
		/// <summary>
		/// Reads enum from input 
		/// </summary>
		/// <param name="abstractHessianInput">Input stream</param>
		/// <returns>Read map or null</returns>
		public override object ReadObject(AbstractHessianInput abstractHessianInput)
		{
			//Read map start
			int code = abstractHessianInput.ReadMapStart();
			switch (code) 
			{
				case CHessianInput.PROT_NULL:
					return null;
				case CHessianInput.PROT_REF_TYPE:
					return abstractHessianInput.ReadRef();
				case 'r':
					throw new CHessianException("remote type is not implemented!");
			}

            string enumName = null;
            while (!abstractHessianInput.IsEnd())
            {
                string key = abstractHessianInput.ReadString();
                if (key.Equals("name"))
                    enumName = abstractHessianInput.ReadString();
                else
                    abstractHessianInput.ReadObject(); // ignore
            }
            abstractHessianInput.ReadMapEnd();

            return Enum.Parse(e_type, enumName);
		}

		#endregion
	}

}
