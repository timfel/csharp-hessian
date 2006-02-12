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
	/// This class conatains hessian protocol constants.
	/// </summary>
	public class CHessianProtocolConstants
	{
		#region CONSTANTS
		public const char PROT_CALL_START = 'c';
		public const char PROT_METHOD = 'm';
		public const char PROT_REPLY_START = 'r';
		public const char PROT_HEADER = 'H';
		public const string MESSAGE_WRONG_REPLY_START = "expected hessian reply";
		public const char PROT_REPLY_FAULT = 'f';
		public const string MESSAGE_WRONG_REPLY_END = "expected end of reply";
		public const char PROT_REPLY_END = 'z';
		public const char PROT_NULL = 'N';
		public const char PROT_BOOLEAN_TRUE = 'T';
		public const char PROT_BOOLEAN_FALSE = 'F';
		public const char PROT_INTEGER_TYPE = 'I';
		public const char PROT_STRING_FINAL = 'S';
		public const char PROT_STRING_INITIAL = 's';
		public const char PROT_XML_FINAL = 'X';
		public const char PROT_XML_INITIAL = 'x';
		public const char PROT_LONG_TYPE = 'L';
		public const char PROT_DOUBLE_TYPE = 'D';
		public const char PROT_MAP_TYPE = 'M';
		public const int END_OF_DATA = -2;
		public const char PROT_DATE_TYPE = 'd';
		public const char PROT_REF_TYPE = 'R';
		public const char PROT_BINARY_START = 'b';
		public const char PROT_BINARY_END = 'B';
		public const char PROT_LIST_TYPE = 'V';
		public const char PROT_TYPE = 't';
		public const char PROT_LENGTH = 'l';
		#endregion
	}
}
