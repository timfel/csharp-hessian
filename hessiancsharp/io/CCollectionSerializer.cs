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
using System.Collections;
#endregion

namespace hessiancsharp.io
{
	/// <summary>
	/// Serializing of the Lists.
	/// </summary>
	public class CCollectionSerializer: AbstractSerializer
	{
		#region PUBLIC_METHODS
		/// <summary>
		/// Writes list objects (That extends from ICollection-Interfaces)
		/// </summary>
		/// <param name="objList">List object</param>
		/// <param name="abstractHessianOutput">HessianOutput - Instance</param>
		public override void WriteObject(object objList, AbstractHessianOutput abstractHessianOutput) 
		{
			if (abstractHessianOutput.AddRef(objList))
				return ;
			
			ICollection collection = ( ICollection) objList;
			Type type = objList.GetType();
			if (type.Equals(typeof(ArrayList)))
				abstractHessianOutput.WriteListBegin(collection.Count, null);
			else
				abstractHessianOutput.WriteListBegin(collection.Count, objList.GetType().FullName);
			IEnumerator enumerator =  collection.GetEnumerator();
			while (enumerator.MoveNext())
			{
				Object value = enumerator.Current;
				abstractHessianOutput.WriteObject(value);
			}
			abstractHessianOutput.WriteListEnd();
		}
		#endregion
	}
}
