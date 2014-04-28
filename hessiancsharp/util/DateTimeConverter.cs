/*
***************************************************************************************************** 
* HessianCharp - The .Net implementation of the Hessian Binary Web Service Protocol (www.caucho.com) 
* Copyright (C) 2004-2005  by D. Minich, V. Byelyenkiy, A. Voltmann, M. Wuttke
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
* 2011-03-31 First version; wuttke
* 2014-04-25 Gilles Meyer <gillesmy@gmail.com>
*            Prefer UTC DateTime over LocalTime as internal DateTime representation.
*            Leave the consumer free to interpret the DateTime as he wishes afterward.
******************************************************************************************************
*/

using System;

namespace hessiancsharp.util
{

    /// <summary>
    /// Convert milliseconds since epoch defined by the hessian protocol 
    /// (Java convention) into C# UTC DateTime object and vice-versa.
    /// </summary>
    public class DateTimeConverter
    {
        // Offset in milliseconds between Hessian date time reference (January 1, 1970, 00:00:00 UTC)
        // and C# date time reference (January 1, 0001 at 00:00:00.000)
        private const long CSharpRefHessianRefOffsetMillis = 62135596800000;

        // One millisecond equals 10^4 x 100 nanoseconds
        // This is used to go from Hessian millisecond resolution to C# 100-nanosecond resolution and vice versa.
        private const long MillisInHundredNanos = 10000;

        /// <summary>
        /// Convert C#-UTC-DateTime Object to Java-UTC-Ticks
        /// </summary>
        public static long ConvertUtcDateTimeToJavaUtcTicks(DateTime dt)
        {
            long utcJavaTicks = dt.Ticks / MillisInHundredNanos - CSharpRefHessianRefOffsetMillis;
            return utcJavaTicks;
        }

        /// <summary>
        /// Convert Java-UTC-Ticks into a C#-UTC-DateTime Object.
        /// </summary>
        public static DateTime ConvertJavaUtcTicksToUtcDateTime(long utcTicks)
        {
            long ticks = (utcTicks + CSharpRefHessianRefOffsetMillis) * MillisInHundredNanos;
            return new DateTime(ticks, DateTimeKind.Utc);
        }

    }
}
