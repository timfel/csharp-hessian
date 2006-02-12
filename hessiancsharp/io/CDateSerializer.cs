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
	/// Serializing of DateTime - Instances.
	/// </summary>
	public class CDateSerializer:AbstractSerializer
	{
		#region CLASS_FIELDS
		private const long ticksDifference = 621355968000000000;
		private const int ticksDivider = 10000;
		#endregion
		#region PUBLIC_METHODS
		/// <summary>
		/// Writes Instance of the DateTime class
		/// </summary>
		/// <param name="objDate">Instance of the DateTime class</param>
		/// <param name="abstractHessianOutput">HessianOutput - Stream</param>
		public override void  WriteObject(Object objDate, AbstractHessianOutput abstractHessianOutput)
		{
			if (objDate == null)
				abstractHessianOutput.WriteNull();
			else
			{
				/*Type type = objDate.GetType();
				abstractHessianOutput.WriteMapBegin(type.FullName);
				abstractHessianOutput.WriteString("value");
				abstractHessianOutput.WriteUTCDate(((((DateTime) objDate).Ticks - ticksDifference) / ticksDivider) - (long) TimeZone.CurrentTimeZone.GetUtcOffset(((DateTime) objDate)).TotalMilliseconds);
				abstractHessianOutput.WriteMapEnd();
				*/
				abstractHessianOutput.WriteUTCDate(((DateTime)objDate).ToFileTimeUtc());
			}
		}
		#endregion
	}
}
