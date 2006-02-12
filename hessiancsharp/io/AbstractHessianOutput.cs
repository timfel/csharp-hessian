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
#endregion

namespace hessiancsharp.io 
{
	/// <summary>
	/// Parent of the HessianOutput class.
	/// Declares write operations (access methods) to an OutputStream
	/// </summary>
	public abstract class AbstractHessianOutput : CHessianProtocolConstants {
		#region CLASS_FIELDS
		// serializer factory
		protected CSerializerFactory m_serializerFactory = null;
		#endregion
		
		#region PROPERTIES
		/// <summary>
		/// Sets the serializer factory
		/// </summary>
		public CSerializerFactory CSerializerFactory
		{
			set { m_serializerFactory = value; }
		}
		#endregion
		
		#region PUBLIC_METHODS
		/// <summary>
		/// Initialize the output with a new underlying stream
		/// </summary>
		/// <param name="stream">Stream - Instance for output</param>
		public abstract void Init(Stream stream);

		/// <summary>
		/// Writes the method call:
		/// </summary>
		/// 
		/// <code>
		/// c major minor
		/// m b16 b8 method-namek
		/// </code>
		/// <param name="strMethod">the method name to call</param>
		public abstract void StartCall(string strMethod);


		/// <summary>
		/// Writes the method call
		/// <code>
		/// z
		/// </code>
		/// </summary>
		public abstract void CompleteCall();

		/// <summary>
		/// Writes a boolean value to the stream.  The boolean will be written
		/// with the following syntax:
		/// <code>
		/// T
		/// F
		/// </code>
		/// </summary>
		/// <param name="blnValue">the boolean value to write</param>
		public abstract void WriteBoolean(bool blnValue);

		/// <summary>
		/// Writes an integer value to the stream.  The integer will be written
		/// with the following syntax:
		/// <code>
		/// I b32 b24 b16 b8
		/// </code>
		/// </summary>
		/// <param name="intValue">the integer value to write</param>
		public abstract void WriteInt(int intValue);

		/// <summary>
		/// Writes a long value to the stream.  The long will be written
		/// with the following syntax:
		/// <code>
		/// L b64 b56 b48 b40 b32 b24 b16 b8
		/// </code>
		/// </summary>
		/// <param name="lngValue">the long value to write</param>
		public abstract void WriteLong(long lngValue);

		/// <summary>
		/// Writes a double value to the stream.  The double will be written
		/// with the following syntax:
		/// <code>
		/// D b64 b56 b48 b40 b32 b24 b16 b8
		/// </code>
		/// </summary>
		/// <param name="dblValue">the double value to write</param>
		public abstract void WriteDouble(double dblValue);

		/// <summary>
		/// Writes a date to the stream
		/// <code>
		/// T  b64 b56 b48 b40 b32 b24 b16 b8
		/// </code>
		/// </summary>
		/// <param name="time">the date in milliseconds from the epoch in UTC</param>
		public abstract void WriteUTCDate(long time);

		/// <summary>
		/// Writes a null value to the stream.
		/// The null will be written with the following syntax
		/// <code>
		/// N
		/// </code>
		/// </summary>
		public abstract void WriteNull();

		/// <summary>
		/// Writes a string value to the stream using UTF-8 encoding.
		/// The string will be written with the following syntax:
		/// <code>
		/// S b16 b8 string-value
		/// </code>
		/// </summary>
		/// <param name="strValue">the string value to write</param>
		public abstract void WriteString(String strValue);

		/// <summary>
		/// Writes a string value to the stream using UTF-8 encoding.
		/// The string will be written with the following syntax:
		/// <code>
		/// S b16 b8 string-value
		/// </code>
		/// </summary>
		/// <param name="arrBuffer">the value to write as string value</param>
		/// <param name="intOffset">value offset</param>
		/// <param name="intLength">value length</param>
		public abstract void WriteString(char[] arrBuffer, int intOffset, int intLength);

		/// <summary>
		/// Writes a byte array to the stream.
		/// The array will be written with the following syntax:
		/// <code>
		/// B b16 b18 bytes
		/// </code>
		/// </summary>
		/// <param name="arrBuffer">Array with bytes to write</param>
		public abstract void WriteBytes(byte[] arrBuffer);
		
		/// <summary>
		/// Writes a byte array to the stream.
		/// The array will be written with the following syntax:
		/// <code>
		/// B b16 b18 bytes
		/// </code>
		/// </summary>
		/// <param name="arrBuffer">Array with bytes to write</param>
		/// <param name="intOffset">Vslue offset</param>
		/// <param name="intLength">Value length</param>
		public abstract void WriteBytes(byte[] arrBuffer, int intOffset, int intLength);

		/// <summary>
		/// Writes a part of the byte buffer to the stream
		/// <code>
		/// b b16 b18 bytes
		/// </code>
		/// </summary>
		/// <param name="arrBuffer">Array with bytes to write</param>
		/// <param name="intOffset">Vslue offset</param>
		/// <param name="intLength">Value length</param>
		public abstract void WriteByteBufferPart(byte[] arrBuffer,
			int intOffset,
			int intLength);

		/// <summary>
		/// Writes the last chunk of a byte buffer to the stream
		/// <code>
		/// b b16 b18 bytes
		/// </code>
		/// </summary>
		/// <param name="arrbuffer">Array with bytes to write</param>
		/// <param name="intOffset">Vslue offset</param>
		/// <param name="intLength">Value length</param>
		public abstract void WriteByteBufferEnd(byte[] arrbuffer,
			int intOffset,
			int intLength);

		/// <summary>
		/// Writes a reference
		/// <code>
		/// R b32 b24 b16 b8
		/// </code>
		/// </summary>
		/// <param name="intValue">he integer value to write</param>
		public abstract void WriteRef(int intValue);

		/// <summary>
		/// Removes a reference
		/// </summary>
		/// <param name="objReference">Object reference to remove</param>
		/// <returns>True, if the refernece was successfully removed, otherwiese False</returns>
		public abstract bool RemoveRef(object objReference);

		/// <summary>
		/// Replaces a reference from one object to another
		/// </summary>
		/// <param name="objOldReference">Old object reference</param>
		/// <param name="objNewReference">New object reference</param>
		/// <returns>True, if the refernece was successfully replaced, otherwiese False</returns>
		public abstract bool ReplaceRef(object objOldReference, object objNewReference);

		
		/// <summary>
		/// Adds an object to the reference list.  If the object already exists,
		/// writes the reference, otherwise, the caller is responsible for
		/// the serialization
		/// <code>
		/// R b32 b24 b16 b8
		/// </code>
		/// </summary>
		/// <param name="objReference">the object to add as a reference</param>
		/// <returns>true if the object has been written</returns>
		public abstract bool AddRef(object objReference);

		/// <summary>
		/// Writes a generic object to the output stream
		/// </summary>
		/// <param name="obj">Object to write</param>
		public abstract void WriteObject(object obj);

		/// <summary>
		/// Writes the list header to the stream.  List writers will call
		/// <code>writeListBegin</code> followed by the list contents and then
		/// call <code>writeListEnd</code> 
		/// <code>
		/// V
		/// t b16 b8 type
		/// l b32 b24 b16 b8
		///</code>
		/// </summary>
		/// <param name="intLength">Length of array</param>
		/// <param name="strType">Type name of the array</param>
		public abstract void WriteListBegin(int intLength, string strType);

		/// <summary>
		/// Writes the tail of the list to the stream
		/// </summary>
		public abstract void WriteListEnd();
		/// <summary>
		/// Writes the map header to the stream.  Map writers will call
		/// <code>writeMapBegin</code> followed by the map contents and then
		/// call <code>writeMapEnd</code>
		/// <code>
		/// Mt b16 b8 type (<key> <value>)z
		/// </code>
		/// </summary>
		/// <param name="strType">Type of map</param>
		public abstract void WriteMapBegin(string strType);

		/// <summary>
		/// Writes the tail of the map to the stream
		/// </summary>
		public abstract void WriteMapEnd();

		/// <summary>
		/// Writes a remote object reference to the stream.  The type is the
		/// type of the remote interface
		/// <code>
		/// 'r' 't' b16 b8 type url
		/// </code>
		/// </summary>
		/// <param name="strType">type of remote object</param>
		/// <param name="strUrl">URL of remote object</param>
		public abstract void WriteRemote(string strType, string strUrl);

		public abstract void StartReply();
		/// <summary>
		/// Writes a fault.  The fault will be written
		/// as a descriptive string followed by an object:
		/// <code>
		/// f
		/// &lt;string&gt;code
		/// &lt;string&gt;the fault code
		/// &lt;string&gt;message
		/// &lt;string&gt;the fault mesage
		/// &lt;string&gt;detail
		/// mt\x00\xnnException
		/// ...
		/// z
		/// z
		/// </code>
		/// </summary>
		/// <param name="strCode">code the fault code</param>
		/// <param name="strMessage">fault message</param>
		/// <param name="objDetail">fault detail</param>
		public abstract void WriteFault(string strCode, string strMessage, object objDetail);
		/// <summary>
		/// Completes reading the reply.
		/// A successful completion will have a single value:
		/// <code>
		/// z
		/// </code> 
		/// </summary>
		public abstract void CompleteReply();
		

		#endregion

	}
}