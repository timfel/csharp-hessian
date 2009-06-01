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
* 
* 2009-05-31: mwuttke, min/max utc ticks
******************************************************************************************************
*/

#region NAMESPACES
using System;
using System.Reflection;
#endregion


namespace hessiancsharp.io
{
	/// <summary>
	/// Date - Deserialization.
	/// </summary>
	public class CDateDeserializer:AbstractDeserializer
	{

        private const long MIN_UTC_TICKS = 0L;
        private const long MAX_UTC_TICKS = 3155378939999999999L;

		#region PUBLIC_METHODS
        /// <summary>
        /// Makes a C# DateTime object from a Java ticks value
        /// from java.util.Date.getTime().
        /// </summary>
        /// <param name="javaDate"></param>
        /// <returns></returns>
        public static DateTime MakeCSharpDate(long javaDate)
        {
            const long timeShift = 62135596800000;
            long ticks = (javaDate + timeShift) * 10000;

            if (ticks < MIN_UTC_TICKS)
                ticks = MIN_UTC_TICKS;
            if (ticks > MAX_UTC_TICKS)
                ticks = MAX_UTC_TICKS;

            DateTime dt = new DateTime(ticks, DateTimeKind.Utc);
            dt = dt.ToLocalTime(); // der Einfachheit halber
            return dt;
        }

		/// <summary>
		/// Reads date
		/// </summary>
		/// <param name="abstractHessianInput">HessianInput - Instance</param>
		/// <returns>DateTime - Instance</returns>
		public override object ReadObject(AbstractHessianInput abstractHessianInput)
		{
            long javaTime = abstractHessianInput.ReadUTCDate();
            return MakeCSharpDate(javaTime);
			//return DateTime.FromFileTimeUtc( abstractHessianInput.ReadUTCDate());
		}
		#endregion

	}
}
