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
using System.Text;
using System.Collections;
#endregion

namespace hessiancsharp.io 
{
	/// <summary>
	/// Input stream for Hessian requests.
	/// <p>HessianInput is unbuffered, so any client needs to provide
	/// its own buffering.
	/// <code>
	/// InputStream is = ...; // from http connection
	/// HessianInput in = new HessianInput(is);
	/// String value;
	/// in.startReply();         // read reply header
	/// value = in.readString(); // read string value
	/// in.completeReply();      // read reply footer
	/// </code>
	/// </summary>
	public class CHessianInput : AbstractHessianInput 
	{
		#region CLASS_FIELDS

		/// <summary>
		/// InputStream what is reading from
		/// </summary>
		private Stream m_srInput = null;

		/// <summary>
		/// a peek character
		/// </summary>
		private int m_intPeek = -1;

		/// <summary>
		/// true if this is the last chunk
		/// </summary>
		private bool m_blnIsLastChunk;
		
		

		/// <summary>
		/// the chunk length
		/// </summary>
		private int m_intChunkLength;
		/// <summary>
		/// Array with object references
		/// </summary>
		private ArrayList m_arrRefs;

        private IDictionary m_deserializers;


		#endregion

		#region CONSTRUCTORS
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="srInput">InputStream what have to read from</param>
		public CHessianInput(Stream srInput) 
		{
			Init(srInput);
		}
		#endregion
		#region PROPERTIES
		
		

		#endregion
		#region PRIVATE_METHODS
		/// <summary>
		/// Parses a 32-bit integer value from the stream.
		/// <code>
		/// b32 b24 b16 b8
		/// </code>
		/// </summary>
		/// <returns>integer value</returns>
		/// <exception cref="hessiancsharp.io.hessiannet.hessian.io.HessianProtocolException"/>
		private int ParseInt() 
		{
			int b32 = Read();
			int b24 = Read();
			int b16 = Read();
			int b8 = Read();

			return (b32 << 24) + (b24 << 16) + (b16 << 8) + b8;
		}
		

        /// <summary>
        /// Prepares the fault.
        /// </summary>
        /// <returns>HessianProtocolException with fault reason</returns>
        private Exception PrepareFault()
        {
            Exception exp = null;
            Hashtable htFault = this.ReadFault();
            object objDetail = htFault["detail"];
            string strMessage = (String)htFault["message"];
            string exceptionMessage = strMessage;
            if (objDetail != null && typeof(Exception).IsAssignableFrom(objDetail.GetType()))
            {
                exp = objDetail as Exception;
            }
            else
            {
                exp = new CHessianException(strMessage);
            }
            return exp;
        }

		
		/// <summary>
		/// Starts reading the reply
		/// A successful completion will have a single value:
		/// r
		/// </summary>
		/// <exception cref="CHessianException"/>
		public override void StartReply()			
		{
			int tag = Read();
    
		   if (tag != 'r')
			   throw new CHessianException("expected hessian reply");
		

			int major = Read();
			int minor = Read();
    
			tag = Read();
			if (tag == 'f')
				throw new CHessianException("False Code: 'f'");
			else
				m_intPeek = tag;
	   }

		/// <summary>
		/// Reads a fault.
		/// </summary>
		/// <exception cref="CHessianException"/>
		/// <returns>HashMap with fault details</returns>
		private Hashtable ReadFault()			
		{
			Hashtable htFault = new Hashtable();

			int intCode = Read();
			for (; intCode > 0 && intCode != 'z'; intCode = Read()) 
			{
				m_intPeek = intCode;
      
				object objKey = ReadObject();
				object objValue = ReadObject();

				if (objKey != null && objValue != null)
					htFault.Add(objKey, objValue);
			}

			if (intCode != PROT_REPLY_END)
				throw new CHessianException("unknown fault reason");

			return htFault;
		}

		/// <summary>
		/// Reads the next byte from the inputStream
		/// </summary>
		/// <returns>-1 if no bytes can be read,
		/// otherwiese an int with the range 0-255</returns>
		private int Read() 
		{
			int intResult = -1;
			if (m_intPeek >= 0) 
			{
				intResult = m_intPeek;
				m_intPeek = -1;
			} 
			else 
			{
				intResult = this.m_srInput.ReadByte();
			}
			return intResult;
		}
		/// <summary>
		/// Reads string from stream and builds Xml - Node
		/// </summary>
		/// <exception cref="CHessianException"/>
		/// <returns>Root Node of the Xml - Content</returns>
		private System.Xml.XmlNode parseXML() 
		{
			System.Xml.XmlNode xnodResult = null;
			int intData;
			StringBuilder sbTemp = new StringBuilder();
			while ((intData = ParseChar()) >= 0) 
			{
				sbTemp.Append((char) intData);
			}
			System.Xml.XmlDocument xdoc = new System.Xml.XmlDocument();
			try 
			{
				//xdoc.LoadXml(sbTemp.ToString());
                xdoc.LoadXml(LoadString());

				xnodResult = xdoc.DocumentElement;
			} 
			catch( Exception ex) 
			{
				throw new CHessianException("Error while parsing XMl-Content:\n" + ex.Message);
			}
			return xnodResult;
		}

		///<summary>
		/// Reads a character from the underlying stream.
		///</summary>
		///<exception cref="CHessianException"/>
		///<returns>UTF8-Character</returns>
		private int ParseChar()
		{
			while (m_intChunkLength <= 0) 
			{
				if (m_blnIsLastChunk)
					return -1;

				int intCode = Read();

				switch (intCode) 
				{
					case PROT_STRING_INITIAL:
					case PROT_XML_INITIAL:
						m_blnIsLastChunk = false;
						m_intChunkLength = (Read() << 8) + Read();
						break;
					case PROT_STRING_FINAL:
					case PROT_XML_FINAL:
						m_blnIsLastChunk = true;
						m_intChunkLength = (Read() << 8) + Read();
						break;
					default:
						throw new CHessianException("unknown code:" + (char) intCode);
				}
			}
			m_intChunkLength--;
			return ParseUTF8Char();
		}

		/// <summary>
		/// Reads a byte from the underlying stream.
		/// </summary>
		/// <returns>Byte value as int</returns>
		/// <exception cref="CHessianException"/>
		private int ParseByte() 
		{
			while (m_intChunkLength <= 0) 
			{
				if (m_blnIsLastChunk) 
				{
					return -1;
				}

				int intCode = Read();

				switch (intCode) 
				{
					case PROT_BINARY_START:
						m_blnIsLastChunk = false;

						m_intChunkLength = (Read() << 8) + Read();
						break;
        
					case PROT_BINARY_END:
						m_blnIsLastChunk = true;

						m_intChunkLength = (Read() << 8) + Read();
						break;

					default:
						throw new CHessianException("Exception in parseByte");
				}
			}

			m_intChunkLength--;

			return Read();
		}
		/// <summary>
		/// Parses a 64-bit long value from the stream.
		/// <code>
		/// b64 b56 b48 b40 b32 b24 b16 b8
		/// </code>
		/// </summary>
		/// <returns>64-bit long value</returns>
		private long ParseLong()
		{
			long b64 = Read();
			long b56 = Read();
			long b48 = Read();
			long b40 = Read();
			long b32 = Read();
			long b24 = Read();
			long b16 = Read();
			long b8 = Read();

			return ((b64 << 56) +
				(b56 << 48) +
				(b48 << 40) +
				(b40 << 32) +
				(b32 << 24) +
				(b24 << 16) +
				(b16 << 8) +
				b8);
		}
  
		/// <summary>
		/// Parses a 64-bit double value from the stream.
		/// <code>
		/// b64 b56 b48 b40 b32 b24 b16 b8
		/// </code>
		/// </summary>
		/// <exception cref="CHessianException"/>
		/// <returns>64-bit double value</returns>
		private double ParseDouble()
		{
			long b64 = Read();
			long b56 = Read();
			long b48 = Read();
			long b40 = Read();
			long b32 = Read();
			long b24 = Read();
			long b16 = Read();
			long b8 = Read();
			long lngBits = ((b64 << 56) +
				(b56 << 48) +
				(b48 << 40) +
				(b40 << 32) +
				(b32 << 24) +
				(b24 << 16) +
				(b16 << 8) +
				b8);
			byte [] lngBytes = BitConverter.GetBytes(lngBits);
			return BitConverter.ToDouble(lngBytes, 0);
		}
		

		#endregion

		#region PUBLIC_METHODS
		/// <summary>
		/// Initialization with the stream
		/// that will be read from
		/// </summary>
		/// <param name="srInput">stream
		/// that will be read from</param>
		public void Init(Stream srInput) 
		{
			this.m_srInput = srInput;
			this.m_blnIsLastChunk = true;
			this.m_intChunkLength = 0;
			this.m_intPeek = -1;
			this.m_arrRefs = null;
            this.m_deserializers = new Hashtable(100);

			if (base.m_serializerFactory == null) 
			{
				base.m_serializerFactory = new CSerializerFactory();
			}

		}
		/// <summary>
		/// Completes reading the call
		/// A successful completion will have a single value:
		/// z
		/// </summary>
		/// <exception cref="CHessianException"/>
		public override void CompleteCall()			
		{
			int intTag = Read();
    
			if (intTag != 'z')
				throw new CHessianException("Expected end of call");			
		}
		/// <summary>
		/// Completes reading the call
		/// A successful completion will have a single value:
		/// z
		/// </summary>
		/// <exception cref="CHessianException"/>
		public override void CompleteReply()
		{
			int tag = Read();    
			if (tag != 'z')
				throw new CHessianException("Expected iend of reply");				
		}

		/// <summary>
		/// Reads an arbitrary object from the input stream.
		/// </summary>
		/// <returns>Read object</returns>
		/// <exception cref="CHessianException"/>
		public override object ReadObject() 
		{
			int intTag = Read();

			switch (intTag) 
			{
				case PROT_NULL:
					return null;
				case PROT_BOOLEAN_TRUE:
					return true;
				case PROT_BOOLEAN_FALSE:
					return false;
				case PROT_INTEGER_TYPE:
					return ParseInt();
				case PROT_LONG_TYPE:
					return ParseLong();
				case PROT_DOUBLE_TYPE:
					return ParseDouble();
				case PROT_STRING_INITIAL:
				case PROT_STRING_FINAL: 
				{
                    m_blnIsLastChunk = intTag == PROT_STRING_FINAL;
                    m_intChunkLength = (Read() << 8) + Read();
                    return LoadString();


				}
				case PROT_MAP_TYPE: 
				{
					String strType = ReadType();
					return m_serializerFactory.ReadMap(this, strType);
				}
				case PROT_DATE_TYPE:
					return DateTime.FromFileTimeUtc(ParseLong());
				case PROT_REF_TYPE: 
				{
					int intRefNumber = ParseInt();
					return m_arrRefs[intRefNumber];
				}
				case 'r': 
				{
					throw new CHessianException("remote type is not implemented");
				}
				case PROT_XML_INITIAL:
				case PROT_XML_FINAL: 
				{
					m_blnIsLastChunk = intTag == PROT_XML_FINAL;
					m_intChunkLength = (Read() << 8) + Read();
					return parseXML();
				}
				case PROT_BINARY_START:
				case PROT_BINARY_END: 
				{
					m_blnIsLastChunk = intTag == PROT_BINARY_END;
					m_intChunkLength = (Read() << 8) + Read();

					int intData;
					MemoryStream memoryStream = new MemoryStream();
					while ((intData = ParseByte()) >= 0)
					{
						memoryStream.WriteByte((byte)intData);
					}
					return memoryStream.ToArray();
				}
				case PROT_LIST_TYPE: 
				{
					string strType = this.ReadType();
					int intLength = this.ReadLength();
					return m_serializerFactory.ReadList(this, intLength, strType);
				}
				default:
					throw new CHessianException("unknown code:" + (char) intTag);
			}
		}

		
		/// <summary>
		/// Reads an integer
		/// <code>
		/// b32 b24 b16 b8
		/// </code>
		/// </summary>
		/// <exception cref="CHessianException"/>
		/// <returns>integer value</returns>
		/// <exception cref="hessiancsharp.io.hessiannet.hessian.io.HessianProtocolException"/>
		public override int ReadInt() 
		{
			int intTag = Read();

			switch (intTag) 
			{
				case PROT_BOOLEAN_TRUE:
					return 1;
				case PROT_BOOLEAN_FALSE:
					return 0;
				case PROT_INTEGER_TYPE:
					return ParseInt();
				case PROT_LONG_TYPE: 
					return (int) ParseLong();
				case PROT_DOUBLE_TYPE: 
					return (int) ParseDouble();
				default:
					throw new CHessianException("Expected integer but found :" + intTag.ToString());
			}
		}

		/// <summary>
		/// Starts reading the call
		/// A successful completion will have a single value:
		/// c major minor
		/// m b16 b8 method
		/// </summary>		
		public override void StartCall()			
		{			
			ReadCall();
			
			while (ReadHeader() != null) 
			{
				ReadObject();
			}
			
			ReadMethod();			
		}
		/// <summary>
		/// Starts reading the call
		/// c major minor
		/// </summary>
		/// <exception cref="CHessianException"/>	
		public int ReadCall()
		{
			int intTag = Read();
    
			if (intTag != 'c')
			{
				throw new CHessianException("expected hessian call ('c') at code=" + intTag + " ch=" + (char) intTag);		
			}
			int major = Read();
			int minor = Read();

			return (major << 16) + minor;
		}

		
		/// <summary>
		/// Reads a header, returning null if there are no headers.
		/// H b16 b8 value
		/// </summary>
		public String ReadHeader()			
		{
			int tag = Read();

			if (tag == 'H') 
			{
                m_blnIsLastChunk = true;
                m_intChunkLength = (Read() << 8) + Read();

                return LoadString();

			}

			m_intPeek = tag;

			return null;
		}
		
		/// <summary>
		/// Starts reading the call
		/// A successful completion will have a single value:
		/// m b16 b8 method
		/// </summary>
		/// <exception cref="CHessianException"/>
		public String ReadMethod()			
		{
			int intTag = Read();

			if (intTag != 'm')
			{
				throw new CHessianException("expected hessian call ('m') at code=" + intTag + " ch=" + (char) intTag);		
			}
			int d1 = Read();
			int d2 = Read();
			
			m_blnIsLastChunk = true;
			
			m_intChunkLength = d1 * 256 + d2;

            m_strMethod = LoadString();
            return m_strMethod;

		}
		/// <summary>
		/// Reads a string
		/// <code>
		/// s b16 b8 non-final string chunk
		/// S b16 b8 final string chunk
		/// </code>
		/// </summary>
		/// <exception cref="CHessianException"/>
		/// <returns></returns>
		public override string ReadString() 
		{
			int intTag = Read();

			switch (intTag) 
			{
				case PROT_NULL:
					return null;
				case PROT_INTEGER_TYPE:
					return ParseInt().ToString();
				case PROT_LONG_TYPE:
					return ParseLong().ToString();
				case PROT_DOUBLE_TYPE:
					return ParseDouble().ToString();
				case PROT_STRING_FINAL:
				case PROT_STRING_INITIAL:
				case PROT_XML_FINAL:
				case PROT_XML_INITIAL:
					m_blnIsLastChunk = intTag == PROT_STRING_FINAL || intTag == PROT_XML_FINAL;
					m_intChunkLength = (Read() << 8) + Read();

                    return LoadString();
                default:
                    throw new CHessianException("expected an string but recieved " + intTag);

			}
		}

        private string LoadString()
        {
            StringBuilder sbTemp = new StringBuilder();
            int intChar;
            char[] tempChar = new char[8];
            int i = 0;
            while ((intChar = ParseChar()) >= 0)
            {
                tempChar[i] = (char)intChar;
                if (i != (tempChar.Length - 1))
                    i++;
                else
                {
                    i = 0;
                    sbTemp.Append(tempChar);
                }
            }
            if (i != 0)
            {
                sbTemp.Append(tempChar, 0, i);
            }
            return sbTemp.ToString();
        }

				
		/// <summary>
		/// Reads a single UTF8 character.
		/// </summary>
		/// <exception cref="CHessianException"/>
		/// <returns>Single UTF8 character</returns>
		private int ParseUTF8Char() 
		{
			int intChar = Read();

			if (intChar < 0x80) 
			{
				return intChar;
			} 
			else if ((intChar & 0xe0) == 0xc0) 
			{
				int intCh1 = Read();
				int intV = ((intChar & 0x1f) << 6) + (intCh1 & 0x3f);
				return intV;
			} 
			else if ((intChar & 0xf0) == 0xe0) 
			{
				int intCh1 = Read();
				int intCh2 = Read();
				int intV = ((intChar & 0x0f) << 12) + ((intCh1 & 0x3f) << 6) + (intCh2 & 0x3f);
				return intV;
			} 
			else 
			{
				throw new CHessianException("bad utf-8 encoding");
			}
		}

		/// <summary>
		/// Reads a boolean value
		/// <code>
		/// T
		/// F
		/// </code>
		/// </summary>
		/// <exception cref="CHessianException"/>
		/// <returns>Boolean value</returns>
		public override bool ReadBoolean() 
		{
			int intTag = Read();

			switch (intTag) 
			{
				case PROT_BOOLEAN_TRUE:
					return true;
				case PROT_BOOLEAN_FALSE:
					return false;
				case PROT_INTEGER_TYPE:
					return ParseInt() == 0;
				case PROT_LONG_TYPE: 
					return ParseLong() == 0;
				case PROT_DOUBLE_TYPE: 
					return ParseDouble() == 0.0;
				case PROT_NULL:
					return false;
				default:
					throw new CHessianException("expected an boolean but recieved " + intTag);
			}
		}


		/// <summary>
		/// Reads an arbitrary object from the input stream.
		/// </summary>
		/// <param name="expectedType">the expected class if the protocol doesn't supply it.</param>
		/// <returns>Read object</returns>
		/// <exception cref="CHessianException"/>
		/// <exception cref="CHessianException"/>
		public override object ReadObject(Type expectedType) 
		{
			object objResult = null;
			if (expectedType == null || expectedType.Equals(typeof (object))) 
			{
				objResult = ReadObject();
			} 
			else 
			{
				int intTag = Read();
				if (intTag != PROT_NULL) 
				{
					switch (intTag) 
					{
						case PROT_MAP_TYPE:
						{
							string strType = ReadType();
							
                            AbstractDeserializer deserializer = GetObjectDeserializer(strType);

							if (expectedType != deserializer.GetOwnType() && expectedType.IsAssignableFrom(deserializer.GetOwnType()))
								return deserializer.ReadMap(this);

							deserializer = GetDeserializer(expectedType);


							return deserializer.ReadMap(this);
						}
						case PROT_REF_TYPE:
						{
							int intRefNumber = ParseInt();
							return m_arrRefs[intRefNumber];
						}
						
						case 'r':
						{
							throw new CHessianException("remote type is not implemented");
						}
						case PROT_LIST_TYPE:
						{
							string strType = this.ReadType();
							int intLength = this.ReadLength();
							AbstractDeserializer deserializer = this.m_serializerFactory.GetObjectDeserializer(strType);
							if (expectedType != deserializer.GetType() && expectedType.IsAssignableFrom(deserializer.GetType()))
							//if (expectedType != deserializer.GetOwnType() && expectedType.IsAssignableFrom(deserializer.GetOwnType()))
								return deserializer.ReadList(this, intLength);
							deserializer = m_serializerFactory.GetDeserializer(expectedType);
							return deserializer.ReadList(this, intLength);
						}
					}

					m_intPeek = intTag;

					objResult = m_serializerFactory.GetDeserializer(expectedType).ReadObject(this);
				}
			}
			return objResult;
		}

        private AbstractDeserializer GetObjectDeserializer(string strType)
        {
            if (!m_deserializers.Contains(strType))
            {
                m_deserializers[strType] = m_serializerFactory.GetObjectDeserializer(strType);
            }
            return (AbstractDeserializer)m_deserializers[strType];
        }

        private AbstractDeserializer GetDeserializer(Type expType)
        {
            if (!m_deserializers.Contains(expType))
            {
                m_deserializers[expType] = m_serializerFactory.GetDeserializer(expType);
            }
            return (AbstractDeserializer)m_deserializers[expType];
        }


		/// <summary>
		/// Reads a reply as an object.
		/// </summary>
		/// <param name="expectedType">the expected class if the protocol doesn't supply it.</param>
		/// <returns>Reply as an object</returns>
		/// <exception cref="CHessianException"/>
		public override object ReadReply(Type expectedType) 
		{
			object objResult = null;
			int intTag = this.Read();

			if (intTag != PROT_REPLY_START) 
			{
				throw new CHessianException(MESSAGE_WRONG_REPLY_START);
			}
			int intMajor = Read();
			int intMinor = Read();

			intTag = Read();

			if (intTag == PROT_REPLY_FAULT) 
			{
				throw PrepareFault();
			} 
			else 
			{
				m_intPeek = intTag;

				objResult = ReadObject(expectedType);

				CompleteValueReply();

			}
			return objResult;
		}

		/// <summary>
		/// Completes reading the call
		/// <p>A successful completion will have a single value:
		/// <code>
		/// z
		/// </code>
		/// <exception cref="CHessianException"/>
		/// </summary>
		public void CompleteValueReply() 
		{
			int intTag = Read();
			if (intTag != PROT_REPLY_END) 
			{
				throw new CHessianException(MESSAGE_WRONG_REPLY_END);
			}
		}

		
		/// <summary>
		/// Reads a double.
		/// <code>
		/// D b64 b56 b48 b40 b32 b24 b16 b8
		/// </code>
		/// </summary>
		/// <exception cref="CHessianException"/>
		/// <returns>Double value</returns>
		public override double ReadDouble()
		{
			int intTag = Read();

			switch (intTag) 
			{
				case PROT_BOOLEAN_TRUE: return 1;
				case PROT_BOOLEAN_FALSE: return 0;
				case PROT_INTEGER_TYPE: return ParseInt();
				case PROT_LONG_TYPE: return (double) ParseLong();
				case PROT_DOUBLE_TYPE: return ParseDouble();
      
				default:
					throw new CHessianException("expected an double but recieved " + intTag);
			}
		}

		/// <summary>
		/// Reads the start of a list
		/// </summary>
		public override int ReadListStart() 
		{
			return Read();
		}

		/// <summary>
		///  Read the end byte
		/// </summary>
		/// <exception cref="CHessianException"/>
		public override void ReadListEnd() 
		{
			int intCode = Read();

			if (intCode != PROT_REPLY_END)
				new CHessianException("unknown code " + (char) intCode);
		}

		/// <summary>
		///   Adds an object reference.
		/// </summary>
		public override int AddRef(Object obj) 
		{
			if (m_arrRefs == null)
				m_arrRefs = new ArrayList();
			
			m_arrRefs.Add(obj);

			return m_arrRefs.Count -1;
		}
		
		/// <summary>
		/// Reads a reference.
		/// </summary>
		/// <returns>reference object</returns>
		public override object ReadRef()
		{
			return m_arrRefs[ParseInt()];
		}

		/// <summary>
		///   Read the end byte
		/// </summary>
		/// <exception cref="CHessianException"/>
		public override void ReadEnd() 
		{
			int intCode = Read();

			if (intCode != PROT_REPLY_END)
				new CHessianException("unknown code " + (char) intCode);
		}

		/// <summary>
		///   Returns true if the data has ended.
		/// </summary>
		public override bool IsEnd() 
		{
			int intCode = Read();

			m_intPeek = intCode;

			return (intCode < 0 || intCode == PROT_REPLY_END);
		}

		/// <summary>
		///   Reads an object type.
		/// </summary>
		public override String ReadType() 
		{
			int intCode = Read();

			if (intCode != 't') 
			{
				m_intPeek = intCode;
				return "";
			}

			m_blnIsLastChunk = true;
			m_intChunkLength = (Read() << 8) + Read();

			return LoadString();


		}

		/// <summary>
		///  Reads the length of a list.
		/// </summary>
		/// <returns>Length of a list</returns>
		public override int ReadLength() 
		{
			int intCode = Read();

			if (intCode != 'l') 
			{
				m_intPeek = intCode;
				return -1;
			}

			return ParseInt();
		}
		
		/// <summary>
		/// Reads the byte array
		/// </summary>
		/// <exception cref="CHessianException"/>
		/// <returns>Byte array</returns>
		public override byte[] ReadBytes() 
		{
			int intTag = Read();

			switch (intTag) 
			{
				case PROT_NULL:
					return null;
				case PROT_BINARY_END:
				case PROT_BINARY_START:
					m_blnIsLastChunk = intTag == PROT_BINARY_END;
					m_intChunkLength = (Read() << 8) + Read();
					MemoryStream memoryStream = new MemoryStream();
					int intData;
					while ((intData = ParseByte()) >= 0)
						memoryStream.WriteByte((System.Byte) intData);
					return memoryStream.ToArray(); 
				default:
					throw new CHessianException("bytes " + intTag);
			}

			
		}


		/// <summary>
		/// Reads a long
		/// <code>
		/// L b64 b56 b48 b40 b32 b24 b16 b8
		/// </code>
		/// </summary>
		/// <exception cref="CHessianException"/>
		/// <returns>Long value</returns>
		public override long ReadLong()
		{
			int intTag = Read();
			switch (intTag) 
			{
				case PROT_BOOLEAN_TRUE: return 1;
				case PROT_BOOLEAN_FALSE: return 0;
				case PROT_INTEGER_TYPE: return ParseInt();
				case PROT_LONG_TYPE: return ParseLong();
				case PROT_DOUBLE_TYPE: return (long) ParseDouble();
				default:
					throw new CHessianException("expected an long but recieved " + intTag);
			}
		}
		
		
		/// <summary> Reads a float
		/// *
		/// <code>
		/// D b64 b56 b48 b40 b32 b24 b16 b8
		/// </code>
		/// </summary>
		public virtual float ReadFloat()
		{
			return (float) ReadDouble();
		}

	
		/// <summary>
		/// Reads a byte from the stream.
		/// </summary>
		/// <returns>Byt value as int</returns>
		public virtual int ReadByte()
		{
			if (m_intChunkLength > 0)
			{
				m_intChunkLength--;
				if (m_intChunkLength == 0 && m_blnIsLastChunk)
					m_intChunkLength = END_OF_DATA;
				
				return Read();
			}
			else if (m_intChunkLength == END_OF_DATA)
			{
				m_intChunkLength = 0;
				return - 1;
			}
			
			int intTag = Read();
			
			switch (intTag)
			{
				
				case PROT_NULL: 
					return - 1;
				
				
				case PROT_BINARY_END: 
				case PROT_BINARY_START: 
					m_blnIsLastChunk = intTag == PROT_BINARY_END;
					m_intChunkLength = (Read() << 8) + Read();
					
					int intValue_Renamed = ParseByte();
					
					// special code so successive read byte won't
					// be read as a single object.
					if (m_intChunkLength == 0 && m_blnIsLastChunk)
						m_intChunkLength = END_OF_DATA;
					
					return intValue_Renamed;
				
				
				default: 
					throw new CHessianException("expected " + PROT_BINARY_END +" at " + (char) intTag);
				
			}
		}
		
		/// <summary>
		/// Reads a byte array from the stream.
		/// </summary>
		/// <param name="arrBuffer">Buffer for read</param>
		/// <param name="intOffset">Offset</param>
		/// <param name="intLength">Length</param>
		/// <returns>Length of read bytes</returns>
		/// <exception cref="CHessianException"/>
		public virtual int ReadBytes(sbyte[] arrBuffer, int intOffset, int intLength)
		{
			int intReadLength = 0;
			
			if (m_intChunkLength == END_OF_DATA)
			{
				m_intChunkLength = 0;
				return - 1;
			}
			else if (m_intChunkLength == 0)
			{
				int intTag = Read();
				
				switch (intTag)
				{
					case PROT_NULL: 
						return - 1;
					case PROT_BINARY_END: 
					case PROT_BINARY_START:  
						m_blnIsLastChunk = intTag == PROT_BINARY_END;
						m_intChunkLength = (Read() << 8) + Read();
						break;
					default: 
						throw new CHessianException("expected 'B' at " + (char) intTag);
				}
			}
			while (intLength > 0)
			{
				if (m_intChunkLength > 0)
				{
					arrBuffer[intOffset++] = (sbyte) Read();
					m_intChunkLength--;
					intLength--;
					intReadLength++;
				}
				else if (m_blnIsLastChunk)
				{
					if (intReadLength == 0)
						return - 1;
					else
					{
						m_intChunkLength = END_OF_DATA;
						return intReadLength;
					}
				}
				else
				{
					int tag = Read();
					switch (tag)
					{
						case PROT_BINARY_END: 
						case PROT_BINARY_START: 
							m_blnIsLastChunk = tag == PROT_BINARY_END;
							m_intChunkLength = (Read() << 8) + Read();
							break;
						default: 
							throw new CHessianException("expected 'B' at " + (char) tag);
						
					}
				}
			}
			
			if (intReadLength == 0)
				return - 1;
			else if (m_intChunkLength > 0 || !m_blnIsLastChunk)
				return intReadLength;
			else
			{
				m_intChunkLength = END_OF_DATA;
				return intReadLength;
			}
		}

		
		///<summary>
		/// Reads the start of a list.
		///</summary>
		///<returns>Value of the map start byte</returns>
		public override int ReadMapStart()
		{
			return Read();
		}

		/// <summary> 
		/// Reads the end byte.
		/// </summary>
		/// <exception cref="CHessianException"/>
		///<returns>Value of the map end byte</returns>
		public override void  ReadMapEnd()
		{
			int intCode = Read();
			
			if (intCode != PROT_REPLY_END)
				throw new CHessianException("expected " + PROT_REPLY_END +" at " + (char) intCode);				
		}
		
		/// <summary> 
		/// Reads a date.
		/// <code>
		/// T b64 b56 b48 b40 b32 b24 b16 b8
		/// </code>
		/// <exception cref="CHessianException"/>
		/// </summary>
		public override long ReadUTCDate() 
		{
			int tag = Read();

			if (tag != PROT_DATE_TYPE)
				throw new CHessianException("expected " + PROT_DATE_TYPE +" for Date at " + (char) tag);

			return this.ParseLong();
		}
		/// <summary> 
		/// Reads bytes from the underlying stream.
		/// </summary>
		int Read(byte []buffer, int offset, int length)
		{
			int intReadLength = 0;
			  
			while (length > 0) 
			{
				while (m_intChunkLength <= 0) 
				{
					if (m_blnIsLastChunk)
						return intReadLength == 0 ? -1 : intReadLength;

					int code = Read();

					switch (code) 
					{
						case 'b':
							m_blnIsLastChunk = false;

							m_intChunkLength = (Read() << 8) + Read();
							break;
			        
						case 'B':
							m_blnIsLastChunk = true;

							m_intChunkLength = (Read() << 8) + Read();
							break;

						default:
							throw new CHessianException("expected 'byte[]' at " + (char) code);
					}
				}

				int sublen = m_intChunkLength;
				if (length < sublen)
					sublen = length;

				sublen = m_srInput.Read(buffer, offset, sublen);
				offset += sublen;
				intReadLength += sublen;
				length -= sublen;
				m_intChunkLength -= sublen;
			}
			return intReadLength;
		}
		/// <summary> Reads bytes based on an input stream.
		/// </summary>
		public override Stream ReadInputStream()
		{
			int tag = Read();
			
			switch (tag)
			{
				
				case 'N': 
					return null;
				
				
				case 'B': 
				case 'b': 
					m_blnIsLastChunk = tag == 'B';
					m_intChunkLength = (Read() << 8) + Read();
					break;
				
				
				default:
					throw new CHessianException("expected  inputStream at " + (char) tag);								
				
			}
			
			return new HessianInputStream(this);
		}

		
		

		#endregion

		#region ANANYMECLASS
		/// <summary> 
		/// A Stream for HessianInput.
		/// </summary>
		private sealed class  HessianInputStream:Stream
		{
			public HessianInputStream(CHessianInput hessInput)
			{
				this.hessianInput = hessInput;
			}
			private CHessianInput hessianInput;
			
			public CHessianInput HessianInput
			{
				get
				{
					return hessianInput;
				}
				
			}
			internal bool _isClosed = false;
			
			public override int ReadByte()
			{
				if (_isClosed)
					return - 1;
				
				int ch = HessianInput.ParseByte();
				if (ch < 0)
					_isClosed = true;
				
				return ch;
			}			

			public override int Read(byte[] buffer, int offset, int length)
			{
				if (_isClosed)
					return - 1;
				int len = HessianInput.Read(buffer, offset, length);				
				if (len < 0)
					_isClosed = true;
				
				return len;
			}
			
			public override void  Close()
			{
				while (ReadByte() >= 0)
				{
				}
			}

			public override void Flush() 
			{
				throw new CHessianException("not-implemented");
			}

			public override long Seek(long offset, SeekOrigin origin) 
			{
				throw new CHessianException("not-implemented");
			}

			public override void SetLength(long value) 
			{
				throw new CHessianException("not-implemented");
			}			

			public override void Write(byte[] buffer, int offset, int count) 
			{
				throw new CHessianException("not-implemented");
			}

			public override bool CanRead 
			{
				get { return true; }
			}

			public override bool CanSeek 
			{
				get { throw new CHessianException("not-implemented"); }
			}

			public override bool CanWrite 
			{
				get { return false; }
			}

			public override long Length 
			{
				get { throw new CHessianException("not-implemented"); }
			}

			public override long Position 
			{
				get { throw new CHessianException("not-implemented"); }
				set { throw new CHessianException("not-implemented"); }
			}

		}
		#endregion
	}
}