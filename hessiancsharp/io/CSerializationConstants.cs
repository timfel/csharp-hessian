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
* Last change: 2005-12-16
* 	
* Licence added Andre Voltmann.
* 2005-08-04: SBYTE and SBYTE_ARRAY added (Dimitri Minich)
* 2005-12-16: SBYTE_ARRAY added (Dimitri Minich)
******************************************************************************************************
*/

namespace hessiancsharp.io 
{
	/// <summary>
	/// Contains definiton for serialization constants.
	/// </summary>
	public class CSerializationConstants {
		#region CONSTANTS

		public const int NULL = 0;
		public const int BOOLEAN = NULL + 1;
		public const int BYTE = BOOLEAN + 1;
        public const int SBYTE = BYTE + 1;
        public const int SHORT = SBYTE + 1;
		public const int INTEGER = SHORT + 1;
		public const int LONG = INTEGER + 1;
		public const int FLOAT = LONG + 1;
		public const int DOUBLE = FLOAT + 1;
		public const int CHARACTER = DOUBLE + 1;
		public const int STRING = CHARACTER + 1;
		public const int DATE = STRING + 1;

		public const int BOOLEAN_ARRAY = DATE + 1;
		public const int BYTE_ARRAY = BOOLEAN_ARRAY + 1;
        public const int SBYTE_ARRAY = BYTE_ARRAY + 1;
        public const int SHORT_ARRAY = SBYTE_ARRAY + 1;
		public const int INTEGER_ARRAY = SHORT_ARRAY + 1;
		public const int LONG_ARRAY = INTEGER_ARRAY + 1;
		public const int FLOAT_ARRAY = LONG_ARRAY + 1;
		public const int DOUBLE_ARRAY = FLOAT_ARRAY + 1;
		public const int CHARACTER_ARRAY = DOUBLE_ARRAY + 1;
		public const int STRING_ARRAY = CHARACTER_ARRAY + 1;
		public const int OBJECT_ARRAY = STRING_ARRAY + 1;
		

		#endregion
	}
}