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
	/// Parent of the HessianInput class.
	/// Declares read operations (access methods) from an InputStream.
	/// </summary>
	public abstract class AbstractHessianInput : CHessianProtocolConstants  {
		#region CLASS_FIELDS
		// serializer factory
		protected CSerializerFactory m_serializerFactory = null;

		/// <summary>
		/// the method for a call
		/// </summary>
		protected String m_strMethod;

		#endregion
		
		#region PROPERTIES

		/// <summary> 
		/// The methodname
		/// </summary>
		public string Method
		{
			set { m_strMethod = value; }
			get { return m_strMethod; }
		}


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
		/// Reads an arbitrary object from the input stream.
		/// </summary>
		/// <param name="expectedType">The expected class of the value</param>
		/// <returns>object value</returns>
		public abstract object ReadObject(Type expectedType);

		/// <summary>
		/// Reads an arbitrary object from the input stream.
		/// </summary>
		/// <returns>Object value</returns>
		public abstract object ReadObject();
		/// <summary>
		/// Reads a reply as an object.
		/// If the reply has a fault, throws the exception.
		/// </summary>
		/// <param name="expectedType"> Expected class of the value</param>
		/// <returns>Reply value</returns>
		public abstract Object ReadReply(Type expectedType);
		/// <summary>
		/// Reads an integer
		/// 
		/// <code>
		/// I b32 b24 b16 b8
		/// </code>
		/// </summary>
		/// <returns></returns>
		public abstract int ReadInt();
				
		/// <summary>
		/// Reads a boolean
		/// 
		/// <code>
		/// T
		/// F
		/// </code>
		/// </summary>
		/// <returns>boolean value</returns>
		public abstract bool ReadBoolean();
		
		/// <summary>
		/// Reads a string encoded in UTF-8
		/// 
		/// <code>
		/// s b16 b8 non-final string chunk
		/// S b16 b8 final string chunk
		/// </code>
		/// </summary>
		/// <returns>string encoded in UTF-8</returns>
		public abstract string ReadString();
		
		/// <summary>
		/// Reads a long
		/// <code>
		/// L b64 b56 b48 b40 b32 b24 b16 b8
		/// </code>
		/// </summary>
		/// <returns>long value</returns>
		public abstract long ReadLong();
		
		/// <summary>
		/// Reads a double.
		/// <code>
		/// D b64 b56 b48 b40 b32 b24 b16 b8
		/// </code>
		/// </summary>
		/// <returns>double value</returns>
		public abstract double ReadDouble();


		/// <summary>
		/// Reads the start of a list
		/// </summary>
		/// <returns>start of the list</returns>
		public abstract int ReadListStart();

		/// <summary>
		///  Read the end byte
		/// </summary>
		public abstract void ReadListEnd();

		/// <summary>
		///   Adds an object reference.
		/// </summary>
		/// <param name="objReference">Reference object</param>
		/// <returns>Reference number</returns>
		public abstract int AddRef(object objReference);

		/// <summary>
		///   Read the end byte
		/// </summary>
		public abstract void ReadEnd();

		/// <summary>
		///   Returns true if the data has ended.
		/// </summary>
		/// <returns>True if the data has ended, otherwiese false</returns>
		public abstract bool IsEnd();

		/// <summary>
		///   Reads an object type.
		/// </summary>
		/// <returns>Type name</returns>
		public abstract String ReadType();

		/// <summary>
		///  Reads the length of a list.
		/// </summary>
		/// <returns>Length of the list</returns>
		public abstract int ReadLength();

		/// <summary>
		///  Reads a byte array.
		/// </summary>
		/// b b16 b8 non-final binary chunk
		/// B b16 b8 final binary chunk
		/// <returns>Byte array</returns>
		public abstract byte []ReadBytes();
		///<summary>
		/// Reads the start of a list.
		///</summary>
		///<returns>Code for the map start</returns>
		public abstract int ReadMapStart();
		///<summary>
		/// Reads the end of a list.
		///</summary>
		public abstract void  ReadMapEnd();
		/// <summary>
		/// Reads utc date
		/// </summary>
		/// <returns>Read date as miliseconds since the epoche</returns>
		public abstract long ReadUTCDate();
		/// <summary>
		/// Reads a reference.
		/// </summary>
		/// <returns>reference object</returns>
		public abstract object ReadRef();
		/// <summary>
		/// Reads a InputStream.
		/// </summary>
		/// <returns>stream</returns>
		public abstract Stream ReadInputStream();
		/// <summary>
		/// Starts reading the reply
		/// A successful completion will have a single value:
		/// r
		/// </summary>
		/// <exception cref="CHessianException"/>
		public abstract void StartReply();		
		/// <summary>
		/// Completes reading the call
		/// A successful completion will have a single value:
		/// z
		/// </summary>
		/// <exception cref="CHessianException"/>
		public abstract void CompleteReply();
		/// <summary>
		/// Starts reading the call
		/// A successful completion will have a single value:
		/// c major minor
		/// m b16 b8 method
		/// </summary>		
		public abstract void StartCall();
		
		/// <summary>
		/// Completes reading the call
		/// A successful completion will have a single value:
		/// z
		/// </summary>
		/// <exception cref="CHessianException"/>
		public abstract void CompleteCall();

		#endregion
	}
}