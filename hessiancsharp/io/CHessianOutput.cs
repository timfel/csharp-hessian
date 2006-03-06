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
* 2006-03-06 PrintString UTF8 bugfix - Matthias
******************************************************************************************************
*/

#region NAMESPACES
using System;
using System.Collections;
using System.IO;
#endregion

namespace hessiancsharp.io 
{
	/// <summary>
	/// Output stream for Hessian requests
	///<p>HessianOutput is unbuffered, so any client needs to provide
	/// its own buffering.
	///</p> 
	/// </summary>
	public class CHessianOutput : AbstractHessianOutput {
		
		#region CLASS_FIELDS
		/// <summary>
		/// the output stream
		/// </summary>
		protected Stream m_srOutput;
		
		/// <summary>
		/// map of references
		/// </summary>
		private Hashtable m_htRefs;
		#endregion

		#region CONSTRUCTORS
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="srOutput">Output stream</param>
		public CHessianOutput(Stream srOutput) 
		{
			Init(srOutput);
		}
		#endregion
		
		#region PUBLIC_METHODS
		/// <summary>
		/// Initializes the output
		/// </summary>
		/// <param name="srOutput">Output stream</param>
		public override void Init(Stream srOutput) 
		{
			this.m_srOutput = srOutput;

			this.m_htRefs = null;

			if (base.m_serializerFactory == null) 
			{
				base.m_serializerFactory = new CSerializerFactory();
			}
		}

		/// <summary>
		/// Writes a complete method call.
		/// </summary>
		/// <param name="strMethod">Method name</param>
		/// <param name="args">Method args</param>
		public void Call(string strMethod, object[] args) 
		{
			StartCall(strMethod);

			if (args != null) 
			{
				for (int i = 0; i < args.Length; i++) 
				{
					WriteObject(args[i]);
				}
			}
			CompleteCall();

		}

		/// <summary>
		/// Starts the method call.  Clients would use <code>startCall</code>
		/// instead of <code>call</code> if they wanted finer control over
		/// writing the arguments, or needed to write headers.
		/// <code>
		/// c major minor
		/// m b16 b8 method-name
		///</code>
		/// </summary>
		/// <param name="strMethod">method the method name to call.</param>
		public override void StartCall(string strMethod) 
		{
			m_srOutput.WriteByte((byte) (PROT_CALL_START));
			m_srOutput.WriteByte(0);
			m_srOutput.WriteByte(1);
			m_srOutput.WriteByte((byte) (PROT_METHOD));
			int intLength = strMethod.Length;
			m_srOutput.WriteByte((byte) (intLength >> 8));
			m_srOutput.WriteByte((byte) intLength);
			PrintString(strMethod, 0, intLength);
		}

		/// <summary>
		/// Completes call
		/// <code>
		/// z
		/// </code>
		/// </summary>
		public override void CompleteCall() 
		{
			m_srOutput.WriteByte((byte) (PROT_REPLY_END));
            m_srOutput.Flush();

		}

		
		/// <summary>
		/// Starts the reply.
		/// A successful completion will have a single value:
		/// <code>
		/// r
		/// </code>
		/// </summary>
		public override void StartReply() 
		{
			m_srOutput.WriteByte((byte) (PROT_REPLY_START));
			m_srOutput.WriteByte(1);
			m_srOutput.WriteByte(0);
		}

		/// <summary>
		/// Completes reading the reply.
		/// A successful completion will have a single value:
		/// <code>
		/// z
		/// </code> 
		/// </summary>
		public override void CompleteReply() 
		{
			m_srOutput.WriteByte((byte) (PROT_REPLY_END));
            m_srOutput.Flush();

		}

		/// <summary>
		/// Writes a header name.  The header value must immediately follow.
		/// <code>
		/// H b16 b8 foo <i>values</i>
		/// </code>
		/// </summary>
		/// <param name="strHeaderName">Header name</param>
		public void WriteHeader(string strHeaderName) 
		{
			int intLength = strHeaderName.Length;

			m_srOutput.WriteByte((byte) (PROT_HEADER));
			m_srOutput.WriteByte((byte) (intLength >> 8));
			m_srOutput.WriteByte((byte) intLength);

			PrintString(strHeaderName);
		}


		
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
		public override void WriteFault(string strCode, string strMessage, object objDetail) 
		{
			m_srOutput.WriteByte((byte)CHessianProtocolConstants.PROT_REPLY_FAULT);

			WriteString("code");
			WriteString(strCode);

			WriteString("message");
			WriteString(strMessage);

			if (objDetail != null) 
			{
				WriteString("detail");
				WriteObject(objDetail);
			}
			m_srOutput.WriteByte((byte) (PROT_REPLY_END));
		}

		/// <summary>
		/// Writes any object to the output stream.
		/// </summary>
		/// <param name="obj">object to write</param>
		public override void WriteObject(object obj) 
		{
			if (obj == null) 
			{
				WriteNull();
				return;
			}

			AbstractSerializer abstractSerializer = m_serializerFactory.GetSerializer(obj.GetType());

			abstractSerializer.WriteObject(obj, this);
		}

		/// <summary>
		/// Writes the list header to the stream.  List writers will call
		/// <code>writeListBegin</code> followed by the list contents and then
		/// call <code>writeListEnd</code>.
		/// 
		/// <code>
		/// V
		/// t b16 b8 type
		/// l b32 b24 b16 b8
		/// </code>
		/// </summary>
		/// <param name="intLength"></param>
		/// <param name="strType"></param>
		public override void WriteListBegin(int intLength, string strType) 
		{
			m_srOutput.WriteByte((byte) (PROT_LIST_TYPE));
			m_srOutput.WriteByte((byte) (PROT_TYPE));
			PrintLenString(strType);

			m_srOutput.WriteByte((byte) (PROT_LENGTH));
			m_srOutput.WriteByte((byte) (intLength >> 24));
			m_srOutput.WriteByte((byte) (intLength >> 16));
			m_srOutput.WriteByte((byte) (intLength >> 8));
			m_srOutput.WriteByte((byte) (intLength));
		}

		/// <summary>
		/// Writes the tail of the list to the stream.
		/// </summary>
		public override void WriteListEnd() 
		{
			m_srOutput.WriteByte((byte) (PROT_REPLY_END));
		}

		/// <summary>
		/// Writes the map header to the stream.  Map writers will call
		/// <code>writeMapBegin</code> followed by the map contents and then
		/// call <code>writeMapEnd</code>.
		/// <code>
		///  Mt b16 b8 (&lt;key&gt; &lt;value&gt;)z
		/// </code>
		/// </summary>
		/// <param name="strType">Map - Type</param>
		public override void WriteMapBegin(string strType) 
		{
			m_srOutput.WriteByte((byte) (PROT_MAP_TYPE));
			m_srOutput.WriteByte((byte) (PROT_TYPE));
			PrintLenString(strType);
		}

		
		/// <summary>
		/// Writes the tail of the map to the stream.
		/// </summary>
		public override void WriteMapEnd() 
		{
			m_srOutput.WriteByte((byte) (PROT_REPLY_END));
		}

		/// <summary>
		/// Writes a remote object reference to the stream.  The type is the
		/// type of the remote interface.
		/// <code>
		/// 'r' 't' b16 b8 type url
		/// </code>
		/// </summary>
		/// <param name="strType">Type of the remote instance</param>
		/// <param name="strUrl">Url of the remote instance</param>
		public override void WriteRemote(string strType, string strUrl) 
		{
			m_srOutput.WriteByte((byte) ('r'));
			m_srOutput.WriteByte((byte) ('t'));
			PrintLenString(strType);
			m_srOutput.WriteByte((byte) ('S'));
			PrintLenString(strUrl);
		}

		/// <summary>
		/// Writes a boolean value to the stream.  The boolean will be written
		/// with the following syntax:
		/// <code>
		/// T
		/// F
		/// </code>
		/// </summary>
		/// <param name="blnValue">the boolean value to write.</param>
		public override void WriteBoolean(bool blnValue) 
		{
			if (blnValue) 
			{
				m_srOutput.WriteByte((byte) (PROT_BOOLEAN_TRUE));
			} 
			else 
			{
				m_srOutput.WriteByte((byte) (PROT_BOOLEAN_FALSE));
			}
		}

		/// <summary>
		/// Writes an integer value to the stream.  The integer will be written
		/// with the following syntax:
		/// <code>
		/// I b32 b24 b16 b8
		/// </code>
		/// </summary>
		/// <param name="intValue">the integer value to write.</param>
		public override void WriteInt(int intValue) 
		{
			m_srOutput.WriteByte((byte) (PROT_INTEGER_TYPE));
			m_srOutput.WriteByte((byte) (intValue >> 24));
			m_srOutput.WriteByte((byte) (intValue >> 16));
			m_srOutput.WriteByte((byte) (intValue >> 8));
			m_srOutput.WriteByte((byte) (intValue));
		}

		/// <summary>
		/// Writes a long value to the stream.  The long will be written
		/// with the following syntax:
		/// <code>
		/// L b64 b56 b48 b40 b32 b24 b16 b8
		/// </code>
		/// </summary>
		/// <param name="lngValue">the long value to write.</param>
		public override void WriteLong(long lngValue) 
		{
			m_srOutput.WriteByte((byte) (PROT_LONG_TYPE));
			m_srOutput.WriteByte((byte) (lngValue >> 56));
			m_srOutput.WriteByte((byte) (lngValue >> 48));
			m_srOutput.WriteByte((byte) (lngValue >> 40));
			m_srOutput.WriteByte((byte) (lngValue >> 32));
			m_srOutput.WriteByte((byte) (lngValue >> 24));
			m_srOutput.WriteByte((byte) (lngValue >> 16));
			m_srOutput.WriteByte((byte) (lngValue >> 8));
			m_srOutput.WriteByte((byte) (lngValue));
		}

		/// <summary>
		/// Writes a double value to the stream.  The double will be written
		/// with the following syntax:
		/// <code>
		/// D b64 b56 b48 b40 b32 b24 b16 b8
		/// </code>
		/// </summary>
		/// <param name="dblValue">the double value to write</param>
		public override void WriteDouble(double dblValue) 
		{
			byte [] lngBytes = BitConverter.GetBytes(dblValue);
			long lngBits = BitConverter.ToInt64(lngBytes, 0)	;
			m_srOutput.WriteByte((byte) (PROT_DOUBLE_TYPE));
			m_srOutput.WriteByte((byte) (lngBits >> 56));
			m_srOutput.WriteByte((byte) (lngBits >> 48));
			m_srOutput.WriteByte((byte) (lngBits >> 40));
			m_srOutput.WriteByte((byte) (lngBits >> 32));
			m_srOutput.WriteByte((byte) (lngBits >> 24));
			m_srOutput.WriteByte((byte) (lngBits >> 16));
			m_srOutput.WriteByte((byte) (lngBits >> 8));
			m_srOutput.WriteByte((byte) (lngBits));
		}

		/// <summary>
		/// Writes a date to the stream.
		/// <code>
		/// d b64 b56 b48 b40 b32 b24 b16 b8
		/// </code>
		/// </summary>
		/// <param name="lngTime">the date in milliseconds from the epoch in UTC</param>
		public override void WriteUTCDate(long lngTime) 
		{
			m_srOutput.WriteByte((byte) (PROT_DATE_TYPE));
			m_srOutput.WriteByte((byte) (lngTime >> 56));
			m_srOutput.WriteByte((byte) (lngTime >> 48));
			m_srOutput.WriteByte((byte) (lngTime >> 40));
			m_srOutput.WriteByte((byte) (lngTime >> 32));
			m_srOutput.WriteByte((byte) (lngTime >> 24));
			m_srOutput.WriteByte((byte) (lngTime >> 16));
			m_srOutput.WriteByte((byte) (lngTime >> 8));
			m_srOutput.WriteByte((byte) (lngTime));
		}

		/// <summary>
		/// Writes a null value to the stream.
		/// The null will be written with the following syntax
		/// <code>
		/// N
		/// </code>
		/// </summary>
		public override void WriteNull() 
		{
			m_srOutput.WriteByte((byte) (PROT_NULL));
		}

		/// <summary>
		/// Writes a string value to the stream using UTF-8 encoding.
		/// The string will be written with the following syntax:
		/// <code>
		/// S b16 b8 string-value
		/// </code>
		/// </summary>
		/// <param name="strValue">the string value to write.</param>
		public override void WriteString(string strValue) 
		{
			if (strValue == null) 
			{
				m_srOutput.WriteByte((byte) (PROT_NULL));
			} 
			else 
			{
				int intLength = strValue.Length;
				int intOffset = 0;

				while (intLength > 0x8000) 
				{
					int intSublen = 0x8000;

					m_srOutput.WriteByte((byte) (PROT_STRING_INITIAL));
					m_srOutput.WriteByte((byte) (intSublen >> 8));
					m_srOutput.WriteByte((byte) (intSublen));

					PrintString(strValue, intOffset, intSublen);

					intLength -= intSublen;
					intOffset += intSublen;
				}

				m_srOutput.WriteByte((byte) (PROT_STRING_FINAL));
				m_srOutput.WriteByte((byte) (intLength >> 8));
				m_srOutput.WriteByte((byte) (intLength));

				PrintString(strValue, intOffset, intLength);
			}
		}

		
		/// <summary>
		/// Writes a string value to the stream using UTF-8 encoding.
		/// The string will be written with the following syntax:
		/// <code>
		/// S b16 b8 string-value
		/// </code>
		/// </summary>
		/// <param name="arrBuffer">char buffer with data for writing</param>
		/// <param name="intLength">offset for writing</param>
		/// <param name="intOffset">length of chars, that have to be written</param>
		public override void WriteString(char[] arrBuffer, int intOffset, int intLength) 
		{
			if (arrBuffer == null) 
			{
				m_srOutput.WriteByte((byte) (PROT_NULL));
			} 
			else 
			{
				while (intLength > 0x8000) 
				{
					int intSublen = 0x8000;

					m_srOutput.WriteByte((byte) (PROT_STRING_INITIAL));
					m_srOutput.WriteByte((byte) (intSublen >> 8));
					m_srOutput.WriteByte((byte) (intSublen));

					PrintString(arrBuffer, intOffset, intSublen);

					intLength -= intSublen;
					intOffset += intSublen;
				}
				m_srOutput.WriteByte((byte) (PROT_STRING_FINAL));
				m_srOutput.WriteByte((byte) (intLength >> 8));
				m_srOutput.WriteByte((byte) intLength);

				PrintString(arrBuffer, intOffset, intLength);
			}
		}
		
		/// <summary>
		/// Writes a byte array to the stream.
		/// The array will be written with the following syntax:
		/// <code>
		/// B b16 b18 bytes
		/// </code>
		/// </summary>
		/// <param name="arrBuffer">the string value to write</param>
		public override void WriteBytes(byte[] arrBuffer) 
		{
			if (arrBuffer == null) 
			{
				m_srOutput.WriteByte((byte) (PROT_NULL));
			} 
			else 
			{
				WriteBytes(arrBuffer, 0, arrBuffer.Length);
			}
		}

		/// <summary>
		/// Writes a byte array to the stream.
		/// The array will be written with the following syntax:
		/// <code>
		/// B b16 b18 bytes
		/// </code>
		/// </summary>
		/// <param name="arrBuffer">the array with the data to write</param>
		/// <param name="intOffset">data offset</param>
		/// <param name="intLength">data length</param>
		public override void WriteBytes(byte[] arrBuffer, int intOffset, int intLength) 
		{
			if (arrBuffer == null) 
			{
				m_srOutput.WriteByte((byte) (PROT_NULL));
			} 
			else 
			{
				while (intLength > 0x8000) 
				{
					int intSublen = 0x8000;

					m_srOutput.WriteByte((byte) (PROT_BINARY_START));
					m_srOutput.WriteByte((byte) (intSublen >> 8));
					m_srOutput.WriteByte((byte) (intSublen));

					m_srOutput.Write(arrBuffer, intOffset, intSublen);

					intLength -= intSublen;
					intOffset += intSublen;
				}

				m_srOutput.WriteByte((byte) (PROT_BINARY_END));
				m_srOutput.WriteByte((byte) (intLength >> 8));
				m_srOutput.WriteByte((byte) intLength);
				m_srOutput.Write(arrBuffer, intOffset, intLength);
			}
		}

		/// <summary>
		/// Writes a part of the byte buffer to the stream
		/// <code>
		/// b b16 b18 bytes
		/// </code>
		/// </summary>
		/// <param name="arrBuffer">Array with bytes to write</param>
		/// <param name="intOffset">Vslue offset</param>
		/// <param name="intLength">Value length</param>
		public override void WriteByteBufferPart(byte[] arrBuffer, int intOffset, int intLength) 
		{
			while (intLength > 0) 
			{
				int intSublen = intLength;

				if (0x8000 < intSublen) 
				{
					intSublen = 0x8000;
				}

				m_srOutput.WriteByte((byte) ('b'));
				m_srOutput.WriteByte((byte) (intSublen >> 8));
				m_srOutput.WriteByte((byte) (intSublen));

				m_srOutput.Write(arrBuffer, intOffset, intSublen);

				intLength -= intSublen;
				intOffset += intSublen;
			}
		}

		
		/// <summary>
		/// Writes the last chunk of a byte buffer to the stream
		/// <code>
		/// b b16 b18 bytes
		/// </code>
		/// </summary>
		/// <param name="arrBuffer">Array with bytes to write</param>
		/// <param name="intOffset">Vslue offset</param>
		/// <param name="intLength">Value length</param>
		public override void WriteByteBufferEnd(byte[] arrBuffer, int intOffset, int intLength) 
		{
			WriteBytes(arrBuffer, intOffset, intLength);
		}

		/// <summary>
		/// Writes a reference
		/// <code>
		/// R b32 b24 b16 b8
		/// </code>
		/// </summary>
		/// <param name="intValue">he integer value to write</param>
		public override void WriteRef(int intValue) 
		{
			m_srOutput.WriteByte((byte) PROT_REF_TYPE);
			m_srOutput.WriteByte((byte) (intValue << 24));
			m_srOutput.WriteByte((byte) (intValue << 16));
			m_srOutput.WriteByte((byte) (intValue << 8));
			m_srOutput.WriteByte((byte) intValue);
		}

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
		public override bool AddRef(object objReference) 
		{
			if (m_htRefs == null) 
			{
				m_htRefs = new Hashtable();
			}
			
			if (m_htRefs.Contains(objReference)) 
			{
				int t_ref = (int) m_htRefs[objReference];
				int value = t_ref;
				WriteRef(value);
				return true;
			} 
			else 
			{
				m_htRefs.Add(objReference,m_htRefs.Count);
				return false;
			}

		}

		
		/// <summary>
		/// Removes a reference
		/// </summary>
		/// <param name="objReference">Object reference to remove</param>
		/// <returns>True, if the refernece was successfully removed, otherwiese False</returns>
		public override bool RemoveRef(object objReference) 
		{
			if (m_htRefs != null) 
			{
				m_htRefs.Remove(objReference);

				return true;
			} 
			else 
			{
				return false;
			}
		}

		/// <summary>
		/// Replaces a reference from one object to another
		/// </summary>
		/// <param name="objOldReference">Old object reference</param>
		/// <param name="objNewReference">New object reference</param>
		/// <returns>True, if the refernece was successfully replaced, otherwiese False</returns>
		public override bool ReplaceRef(object objOldReference, object objNewReference) 
		{			
			if (m_htRefs.Contains(objOldReference)) 
			{				
				int value = (int) m_htRefs[objNewReference];
				m_htRefs.Remove(objOldReference);
				m_htRefs.Add(objNewReference,value);
				return true;
				
			} 
			else 
			{
				return false;
			}
		}

		/// <summary>
		/// Prints a string to the stream, encoded as UTF-8 with preceeding length
		/// </summary>
		/// <param name="strValue">the string to print</param>
		public void PrintLenString(string strValue) 
		{
			if (strValue == null) 
			{
				m_srOutput.WriteByte(0);
				m_srOutput.WriteByte(0);
			} 
			else 
			{
				int intLength = strValue.Length;
				m_srOutput.WriteByte((byte) (intLength >> 8));
				m_srOutput.WriteByte((byte) intLength);
				PrintString(strValue, 0, intLength);
			}
		}

		/// <summary>
		/// Prints a string to the stream, encoded as UTF-8
		/// </summary>
		/// <param name="strValue">the string to print</param>
		public void PrintString(String strValue) 
		{
			PrintString(strValue, 0, strValue.Length);
		}

		/// <summary>
		/// Prints a string to the stream, encoded as UTF-8
		/// </summary>
		/// <param name="strValue">the string to print</param>
		/// <param name="intOffset">data offset</param>
		/// <param name="intLength">data length</param>
		public void PrintString(string strValue, int intOffset, int intLength) 
		{
            PrintString(strValue.ToCharArray(), intOffset, intLength);

		}

		/// <summary>
		/// Prints a char array to the stream, encoded as UTF-8
		/// </summary>
		/// <param name="arrData">the char data to print</param>
		/// <param name="intOffset">data offset</param>
		/// <param name="intLength">data length</param>
		public void PrintString(char[] arrData, int intOffset, int intLength) 
		{			
            byte[] utfData = System.Text.Encoding.UTF8.GetBytes(arrData, intOffset, intLength);
            // m_srOutput.Write(utfData, intOffset, intLength);
            m_srOutput.Write(utfData, 0, utfData.Length);
		}

		
		#endregion
	}

}