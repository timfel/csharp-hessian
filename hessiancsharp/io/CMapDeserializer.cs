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
* Last change: 2005-08-14
* By Dimitri Minich
* ReadMap modified
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
	public class CMapDeserializer : AbstractDeserializer 
	{
		#region CLASS_FIELDS
		/// <summary>
		/// Type of map
		/// </summary>
		private Type m_type = null;
		#endregion
  
		#region CONSTRUCTORS
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="type">Type of map</param>
		public CMapDeserializer(Type type)
		{
			this.m_type = type;
		}
		#endregion
  
		#region PUBLIC_METHODS
		/// <summary>
		/// Reads map from input
		/// </summary>
		/// <param name="abstractHessianInput">Input stream</param>
		/// <returns>Read map or null</returns>
		public override object ReadMap(AbstractHessianInput abstractHessianInput)
		{
			IDictionary dictionary = null;
            if ((m_type == null) || (m_type.IsInterface && typeof(IDictionary).IsAssignableFrom(m_type)))
				dictionary = new Hashtable();
			else if (m_type.Equals(typeof(Hashtable)))
				dictionary = new Hashtable();
			else 
			{
				//dictionary = (IDictionary)Activator.CreateInstance(m_type);
                dictionary = new Hashtable();
				
			}
			abstractHessianInput.AddRef(dictionary);
			while (! abstractHessianInput.IsEnd()) 
			{
				dictionary.Add(abstractHessianInput.ReadObject(), abstractHessianInput.ReadObject());
			}
			abstractHessianInput.ReadEnd();
			return dictionary;
		}
		/// <summary>
		/// Reads map from input 
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
			return ReadMap(abstractHessianInput);
		}

		#endregion
	}

}
