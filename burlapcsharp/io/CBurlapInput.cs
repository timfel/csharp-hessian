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
* Last change: 2005-12-25
* 2005-12-25 initial class definition by Dimitri Minich.
* ....
******************************************************************************************************
*/

#region NAMESPACES
using System;
using System.IO;
using System.Text;
using System.Collections;
using hessiancsharp.io;
using System.Xml;
#endregion

namespace burlapcsharp.io
{
    class CBurlapInput : AbstractBurlapInput
    {
        #region Constants
        public const int TAG_EOF = -1;

        public const int TAG_NULL = 0;
        public const int TAG_BOOLEAN = 1;
        public const int TAG_INT = 2;
        public const int TAG_LONG = 3;

        public const int TAG_DOUBLE = 4;
        public const int TAG_DATE = 5;
        public const int TAG_STRING = 6;
        public const int TAG_XML = 7;
        public const int TAG_BASE64 = 8;
        public const int TAG_MAP = 9;
        public const int TAG_LIST = 10;

        public const int TAG_TYPE = 11;
        public const int TAG_LENGTH = 12;

        public const int TAG_REF = 13;
        public const int TAG_REMOTE = 14;

        public const int TAG_CALL = 15;
        public const int TAG_REPLY = 16;
        public const int TAG_FAULT = 17;
        public const int TAG_METHOD = 18;
        public const int TAG_HEADER = 19;

        public const int TAG_NULL_END = TAG_NULL + 100;
        public const int TAG_BOOLEAN_END = TAG_BOOLEAN + 100;
        public const int TAG_INT_END = TAG_INT + 100;
        public const int TAG_LONG_END = TAG_LONG + 100;
        public const int TAG_DOUBLE_END = TAG_DOUBLE + 100;
        public const int TAG_DATE_END = TAG_DATE + 100;
        public const int TAG_STRING_END = TAG_STRING + 100;
        public const int TAG_XML_END = TAG_XML + 100;
        public const int TAG_BASE64_END = TAG_BASE64 + 100;
        public const int TAG_MAP_END = TAG_MAP + 100;
        public const int TAG_LIST_END = TAG_LIST + 100;
        public const int TAG_TYPE_END = TAG_TYPE + 100;
        public const int TAG_LENGTH_END = TAG_LENGTH + 100;

        public const int TAG_REF_END = TAG_REF + 100;
        public const int TAG_REMOTE_END = TAG_REMOTE + 100;

        public const int TAG_CALL_END = TAG_CALL + 100;
        public const int TAG_REPLY_END = TAG_REPLY + 100;
        public const int TAG_FAULT_END = TAG_FAULT + 100;
        public const int TAG_METHOD_END = TAG_METHOD + 100;
        public const int TAG_HEADER_END = TAG_HEADER + 100;
        #endregion

        #region CLASS_FIELDS

        protected internal StringBuilder m_sBuild = new StringBuilder();
        protected internal StringBuilder m_sEntityBuild = new StringBuilder();

        /// <summary>
        /// InputStream what is reading from
        /// </summary>
        private Stream m_srInput = null;

        /// <summary>
        /// a peek character
        /// </summary>
        protected int m_intPeek = -1;

        /// <summary>
        /// a peek tag
        /// </summary>
        private int m_intPeekTag = -1;

        /// <summary>
        /// Array with object references
        /// </summary>
        private ArrayList m_arrRefs;


        private static Hashtable m_tagMap = null;

        private static int[] m_base64Decode;


        protected internal System.Globalization.Calendar m_utcCalendar;
        //protected internal System.Globalization.Calendar m_localCalendar;

        #endregion

        #region CONSTRUCTORS

        static CBurlapInput()
        {

            m_tagMap = new Hashtable();
            m_tagMap["null"] = TAG_NULL;

            m_tagMap["boolean"] = TAG_BOOLEAN;
            m_tagMap["int"] = TAG_INT;
            m_tagMap["long"] = TAG_LONG;
            m_tagMap["double"] = TAG_DOUBLE;

            m_tagMap["date"] = TAG_DATE;

            m_tagMap["string"] = TAG_STRING;
            m_tagMap["xml"] = TAG_XML;
            m_tagMap["base64"] = TAG_BASE64;

            m_tagMap["map"] = TAG_MAP;
            m_tagMap["list"] = TAG_LIST;

            m_tagMap["type"] = TAG_TYPE;
            m_tagMap["length"] = TAG_LENGTH;

            m_tagMap["ref"] = TAG_REF;
            m_tagMap["remote"] = TAG_REMOTE;

            m_tagMap["burlap:call"] = TAG_CALL;
            m_tagMap["burlap:reply"] = TAG_REPLY;
            m_tagMap["fault"] = TAG_FAULT;
            m_tagMap["method"] = TAG_METHOD;
            m_tagMap["header"] = TAG_HEADER;


            {
                m_base64Decode = new int[256];
                for (int i = 'A'; i <= 'Z'; i++)
                    m_base64Decode[i] = i - 'A';
                for (int i = 'a'; i <= 'z'; i++)
                    m_base64Decode[i] = i - 'a' + 26;
                for (int i = '0'; i <= '9'; i++)
                    m_base64Decode[i] = i - '0' + 52;
                m_base64Decode['+'] = 62;
                m_base64Decode['/'] = 63;
            }

        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="srInput">InputStream what have to read from</param>
        public CBurlapInput(Stream srInput)
        {
            Init(srInput);
        }

        #endregion

        #region PRIVATE_METHODS


        /// <summary>
        /// Checks the valid character
        /// </summary>
        /// <param name="ch">char
        /// that will be tested</param>
        /// <returns>Returns true if the character is a valid tag character.</returns>
        private bool IsTagChar(int ch)
        {
            return (ch >= 'a' && ch <= 'z' ||
                    ch >= 'A' && ch <= 'Z' ||
                    ch >= '0' && ch <= '9' ||
                    ch == ':' || ch == '-');
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
                intResult = m_srInput.ReadByte();
            }
            return intResult;
        }

        protected String ParseString()
        {
            m_sBuild.Length = 0;

            return ParseString(m_sBuild).ToString();
        }


        protected StringBuilder ParseString(StringBuilder sBuild)
        {
            int ch;

            while ((ch = ReadChar()) >= 0)
                sBuild.Append((char)ch);

            return sBuild;
        }

        /// <summary>
        /// Parses a 32-bit integer value from the stream.
        /// <returns>integer value</returns>
        private int ParseInt()
        {
            int sign = 1;

            int ch = Read();
            if (ch == '-')
            {
                sign = -1;
                ch = Read();
            }

            int value = 0;
            for (; ch >= '0' && ch <= '9'; ch = Read())
                value = 10 * value + ch - '0';

            m_intPeek = ch;

            return sign * value;

        }

        /// <summary>
        /// Parses a 64-bit long value from the stream.
        /// <returns>long value</returns>
        private long ParseLong()
        {
            int sign = 1;

            int ch = Read();
            if (ch == '-')
            {
                sign = -1;
                ch = Read();
            }

            long value = 0;
            for (; ch >= '0' && ch <= '9'; ch = Read())
                value = 10 * value + ch - '0';

            m_intPeek = ch;

            return sign * value;
        }

        internal virtual System.Xml.XmlNode ParseXML()
        {
            throw new CBurlapException("help!");
        }

        /// <summary>
        /// Parses a 64-bit double value from the stream.
        /// <returns>long value</returns>
        private double ParseDouble()
        {
            int ch = SkipWhitespace();

            m_sBuild.Length = 0;

            
            
            for (; ! IsWhitespace(ch) && ch != '<'; ch = Read())
              m_sBuild.Append((char) ch);

            m_intPeek = ch;
            
            return System.Double.Parse(m_sBuild.ToString());
        }


        /// <summary> 
        /// Parses a date value from the stream.
        /// </summary>
        protected internal virtual long ParseDate()
        {
            if (m_utcCalendar == null)
            {                
                m_utcCalendar = new System.Globalization.GregorianCalendar();
            }

            return ParseDate(m_utcCalendar);
        }

        /// <summary> 
        /// Parses a date value from the stream.
        /// </summary>
        protected internal virtual long ParseDate(System.Globalization.Calendar calendar)
        {
            int ch = SkipWhitespace();

            int year = 0;
            for (int i = 0; i < 4; i++)
            {
                if (ch >= '0' && ch <= '9')
                    year = 10 * year + ch - '0';
                else
                    throw ExpectedChar("year", ch);

                ch = Read();
            }

            int month = 0;
            for (int i = 0; i < 2; i++)
            {
                if (ch >= '0' && ch <= '9')
                    month = 10 * month + ch - '0';
                else
                    throw ExpectedChar("month", ch);

                ch = Read();
            }

            int day = 0;
            for (int i = 0; i < 2; i++)
            {
                if (ch >= '0' && ch <= '9')
                    day = 10 * day + ch - '0';
                else
                    throw ExpectedChar("day", ch);

                ch = Read();
            }

            if (ch != 'T')
                throw ExpectedChar("`T'", ch);

            ch = Read();

            int hour = 0;
            for (int i = 0; i < 2; i++)
            {
                if (ch >= '0' && ch <= '9')
                    hour = 10 * hour + ch - '0';
                else
                    throw ExpectedChar("hour", ch);

                ch = Read();
            }

            int minute = 0;
            for (int i = 0; i < 2; i++)
            {
                if (ch >= '0' && ch <= '9')
                    minute = 10 * minute + ch - '0';
                else
                    throw ExpectedChar("minute", ch);

                ch = Read();
            }

            int second = 0;
            for (int i = 0; i < 2; i++)
            {
                if (ch >= '0' && ch <= '9')
                    second = 10 * second + ch - '0';
                else
                    throw ExpectedChar("second", ch);

                ch = Read();
            }

            int ms = 0;
            if (ch == '.')
            {
                ch = Read();

                while (ch >= '0' && ch <= '9')
                {
                    ms = 10 * ms + ch - '0';

                    ch = Read();
                }
            }

            for (; ch > 0 && ch != '<'; ch = Read())
            {
            }

            

            m_intPeek = ch;
            //TODO: wenn es tut die Methode mit Parameter calender überarbeiten, calender entfernen.
            DateTime dt = new DateTime(year, month - 1, day, hour, minute, second, ms);


            return dt.Ticks;
        }




        /// <summary> 
        /// Reads a character from the underlying stream.
        /// </summary>
        internal virtual int ReadChar()
        {
            int ch = Read();

            if (ch == '<' || ch < 0)
            {
                m_intPeek = ch;
                return -1;
            }

            if (ch == '&')
            {
                ch = Read();

                if (ch == '#')
                {
                    ch = Read();

                    if (ch >= '0' && ch <= '9')
                    {
                        int v = 0;
                        for (; ch >= '0' && ch <= '9'; ch = Read())
                        {
                            v = 10 * v + ch - '0';
                        }

                        if (ch != ';')
                            throw new CBurlapException("expected ';' at " + (char)ch);

                        return (char)v;
                    }
                    else
                        throw new CBurlapException("expected digit at " + (char)ch);
                }
                else
                {

                    m_sEntityBuild.Length = 0;

                    for (; ch >= 'a' && ch <= 'z'; ch = Read())
                        m_sEntityBuild.Append((char)ch);

                    System.String entity = m_sEntityBuild.ToString();

                    if (ch != ';')
                        throw ExpectedChar("';'", ch);

                    if (entity.Equals("amp"))
                        return '&';
                    else if (entity.Equals("apos"))
                        return '\'';
                    else if (entity.Equals("quot"))
                        return '"';
                    else if (entity.Equals("lt"))
                        return '<';
                    else if (entity.Equals("gt"))
                        return '>';
                    else
                        throw new CBurlapException("unknown XML entity &" + entity + "; at `" + (char)ch + "'");
                }
            }
            else if (ch < 0x80)
                return (char)ch;
            else if ((ch & 0xe0) == 0xc0)
            {
                int ch1 = Read();
                int v = ((ch & 0x1f) << 6) + (ch1 & 0x3f);

                return (char)v;
            }
            else if ((ch & 0xf0) == 0xe0)
            {
                int ch1 = Read();
                int ch2 = Read();
                int v = ((ch & 0x0f) << 12) + ((ch1 & 0x3f) << 6) + (ch2 & 0x3f);

                return (char)v;
            }
            else
                throw new CBurlapException("bad utf-8 encoding");
        }


        protected internal virtual CBurlapException ExpectedChar(String expect, int ch)
        {
            if (ch < 0)
                return new CBurlapException("expected " + expect + " at end of file");
            else
                return new CBurlapException("expected " + expect + " at " + (char)ch);
        }

        protected internal virtual CBurlapException ExpectedTag(System.String expect, int tag)
        {
            return new CBurlapException("expected " + expect + " at " + TagName(tag));            
        }

        protected internal static String TagName(int tag)
        {
            switch (tag)
            {

                case TAG_NULL:
                    return "<null>";

                case TAG_NULL_END:
                    return "</null>";


                case TAG_BOOLEAN:
                    return "<boolean>";

                case TAG_BOOLEAN_END:
                    return "</boolean>";


                case TAG_INT:
                    return "<int>";

                case TAG_INT_END:
                    return "</int>";


                case TAG_LONG:
                    return "<long>";

                case TAG_LONG_END:
                    return "</long>";


                case TAG_DOUBLE:
                    return "<double>";

                case TAG_DOUBLE_END:
                    return "</double>";


                case TAG_STRING:
                    return "<string>";

                case TAG_STRING_END:
                    return "</string>";


                case TAG_XML:
                    return "<xml>";

                case TAG_XML_END:
                    return "</xml>";


                case TAG_BASE64:
                    return "<base64>";

                case TAG_BASE64_END:
                    return "</base64>";


                case TAG_MAP:
                    return "<map>";

                case TAG_MAP_END:
                    return "</map>";


                case TAG_LIST:
                    return "<list>";

                case TAG_LIST_END:
                    return "</list>";


                case TAG_TYPE:
                    return "<type>";

                case TAG_TYPE_END:
                    return "</type>";


                case TAG_LENGTH:
                    return "<length>";

                case TAG_LENGTH_END:
                    return "</length>";


                case TAG_REF:
                    return "<ref>";

                case TAG_REF_END:
                    return "</ref>";


                case TAG_REMOTE:
                    return "<remote>";

                case TAG_REMOTE_END:
                    return "</remote>";


                case TAG_CALL:
                    return "<burlap:call>";

                case TAG_CALL_END:
                    return "</burlap:call>";


                case TAG_REPLY:
                    return "<burlap:reply>";

                case TAG_REPLY_END:
                    return "</burlap:reply>";


                case TAG_HEADER:
                    return "<header>";

                case TAG_HEADER_END:
                    return "</header>";


                case TAG_FAULT:
                    return "<fault>";

                case TAG_FAULT_END:
                    return "</fault>";


                case -1:
                    return "end of file";


                default:
                    return "unknown " + tag;

            }
        }

        /// <summary> 
        /// Parses a byte array.
        /// </summary>
        protected internal virtual byte[] ParseBytes()
        {
            System.IO.MemoryStream bos = new System.IO.MemoryStream();

            ParseBytes(bos);

            return bos.ToArray();
        }


        /// <summary> 
        /// Parses a byte array.
        /// </summary>
        protected internal virtual System.IO.MemoryStream ParseBytes(System.IO.MemoryStream bos)
        {
            int ch;
            for (ch = SkipWhitespace(); ch >= 0 && ch != '<'; ch = SkipWhitespace())
            {
                int b1 = ch;
                int b2 = Read();
                int b3 = Read();
                int b4 = Read();

                if (b4 != '=')
                {
                    int chunk = ((m_base64Decode[b1] << 18) + (m_base64Decode[b2] << 12) + (m_base64Decode[b3] << 6) + (m_base64Decode[b4]));

                    bos.WriteByte((System.Byte)(chunk >> 16));
                    bos.WriteByte((System.Byte)(chunk >> 8));
                    bos.WriteByte((System.Byte)chunk);
                }
                else if (b3 != '=')
                {
                    int chunk = ((m_base64Decode[b1] << 10) + (m_base64Decode[b2] << 4) + (m_base64Decode[b3] >> 2));

                    bos.WriteByte((System.Byte)(chunk >> 8));
                    bos.WriteByte((System.Byte)chunk);
                }
                else
                {
                    int chunk = ((m_base64Decode[b1] << 2) + (m_base64Decode[b2] >> 4));

                    bos.WriteByte((System.Byte)chunk);
                }
            }

            if (ch == '<')
                m_intPeek = ch;

            return bos;
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
            m_srInput = srInput;
            m_intPeek = -1;
            m_intPeekTag = -1;
            m_arrRefs = null;
            if (base.m_serializerFactory == null)
            {
                base.m_serializerFactory = new CSerializerFactory();
            }
        }

        /// <summary>
        /// Starts reading the call
        /// <pre>
        /// <burlap:call>
        /// <method>method</method>
        /// </pre>
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
        /// Reads the method
        /// <pre>
        /// <method>method</method>
        /// </pre>
        /// </summary>
        public String ReadMethod()
        {
            ExpectTag(TAG_METHOD);
            m_strMethod = ParseString();
            ExpectTag(TAG_METHOD_END);
            return m_strMethod;
        }



        /// <summary>
        /// Starts reading the call
        /// <p>A successful completion will have a single value:
        /// <pre>
        /// <burlap:call>
        /// </pre>
        /// </summary>
        public int ReadCall()
        {
            ExpectTag(TAG_CALL);

            int major = 1;
            int minor = 0;

            return (major << 16) + minor;
        }

        /// <summary>
        /// Completes reading the call
        /// A successful completion will have a single value:
        /// <pre> </burlap:call> </pre>
        /// </summary>
        /// <exception cref="CHessianException"/>
        public override void CompleteCall()
        {
            ExpectTag(TAG_CALL_END);
        }

        public void ExpectTag(int expectTag)
        {
            int tag = ParseTag();          
            if (tag != expectTag)
              throw new CBurlapException("expected " + m_tagMap[expectTag] + " at " + m_tagMap[tag]);
        }



        /// <summary>
        /// Reads a reply as an object.
        /// </summary>
        /// <param name="expectedType">the expected class if the protocol doesn't supply it.</param>
        /// <returns>Reply as an object</returns>
        public override object ReadReply(Type expectedType)
        {
            ExpectTag(TAG_REPLY);
            int tag = ParseTag();
            if (tag == TAG_FAULT)
            {
                //TODO: Better Exception handling
                 //throw PrepareFault();
                throw new CBurlapException("Exception in ReadReply");
            }
            else
            {
                m_intPeekTag = tag;
                object objResult = ReadObject(expectedType);

                ExpectTag(TAG_REPLY_END);

                return objResult;
            }
        }


        /// <summary>
        /// Starts reading the reply
        /// A successful completion will have a single value:
        /// <burlap:reply><value>
        /// </summary>		
        public override void StartReply()
        {
            ExpectTag(TAG_REPLY);
            int tag = ParseTag();

            if (tag == TAG_FAULT)
            {
                //TODO:
                //throw prepareFault();
            }
            else
                m_intPeekTag = tag;
        }

        /// <summary>
        /// Completes reading the call
        /// A successful completion will have a single value:
        /// </burlap:reply>
        /// </summary>
        /// <exception cref="CHessianException"/>
        public override void CompleteReply()
        {
            ExpectTag(TAG_REPLY_END);
        }


        /// <summary>
        /// Reads a header, returning null if there are no headers.
        /// <header>value</header>
        /// </summary>
        public String ReadHeader()
        {
            int tag = ParseTag();
            if (tag == TAG_HEADER)
            {
                m_sBuild.Length = 0;
                String value = ParseString(m_sBuild).ToString();
                ExpectTag(TAG_HEADER_END);
                return value;
            }

            m_intPeekTag = tag;
            return null;
        }

        /// <summary>
        ///Reads a null
        /// <null></null>
        /// </summary>
        public void ReadNull()
        {
            int tag = ParseTag();

            switch (tag)
            {
                case TAG_NULL:
                    ExpectTag(TAG_NULL_END);
                    return;
                //TODO:
                //default:
                //    throw ExpectedTag("null", tag);
            }
        }

        /// <summary>
        /// Reads a boolean value
        /// <code>
        /// <boolean>0</boolean>
        /// <boolean>1</boolean>
        /// </code>
        /// </summary>
        /// <exception cref="CBurlapException"/>
        /// <returns>Boolean value</returns>
        public override bool ReadBoolean()
        {
            int intTag = ParseTag();
            bool value;

            switch (intTag)
            {
                case TAG_NULL:
                    value = false;
                    ExpectTag(TAG_NULL_END);
                    return value;

                case TAG_BOOLEAN:
                    value = ParseInt() != 0;
                    ExpectTag(TAG_BOOLEAN_END);
                    return value;

                case TAG_INT:
                    value = ParseInt() != 0;
                    ExpectTag(TAG_INT_END);
                    return value;

                case TAG_LONG:
                    value = ParseLong() != 0;
                    ExpectTag(TAG_LONG_END);
                    return value;

                case TAG_DOUBLE:
                    value = ParseDouble() != 0;
                    ExpectTag(TAG_DOUBLE_END);
                    return value;
                default:
                    throw new CBurlapException("expected an boolean but recieved " + intTag);
            }
        }

        /// <summary>
        /// Reads a byte from the stream.
        /// </summary>
        /// <int>value</int>
        /// <returns>Byte value as int</returns>
        public virtual int ReadByte()
        {
            return (sbyte)ReadInt();
        }

        /// <summary>
        /// Reads a short.
        /// </summary>
        /// <int>value</int>
        /// <returns>Byte value as int</returns>
        public virtual int ReadShort()
        {
            return (short)ReadInt();
        }

        /// <summary>
        /// Reads an integer
        /// <code>
        /// <int>value</int>
        /// </code>
        /// </summary>
        /// <exception cref="CBurlapException"/>
        /// <returns>integer value</returns>
        public override int ReadInt()
        {
            int intTag = ParseTag();
            int value;

            switch (intTag)
            {
                case TAG_NULL:
                    value = 0;
                    ExpectTag(TAG_NULL_END);
                    return value;

                case TAG_BOOLEAN:
                    value = ParseInt();
                    ExpectTag(TAG_BOOLEAN_END);
                    return value;

                case TAG_INT:
                    value = ParseInt();
                    ExpectTag(TAG_INT_END);
                    return value;

                case TAG_LONG:
                    value = (int)ParseLong();
                    ExpectTag(TAG_LONG_END);
                    return value;

                case TAG_DOUBLE:
                    value = (int)ParseDouble();
                    ExpectTag(TAG_DOUBLE_END);
                    return value;

               
            default:
                throw ExpectedTag("int", intTag);
               
            }
        }


        /// <summary>
        /// Reads a long
        /// <code>
        /// <long>value</long>
        /// </code>
        /// </summary>
        /// <exception cref="CBurlapException"/>
        /// <returns>long value</returns>
        public override long ReadLong()
        {
            int tag = ParseTag();

            long value;

            switch (tag)
            {
                case TAG_NULL:
                    value = 0;
                    ExpectTag(TAG_NULL_END);
                    return value;

                case TAG_BOOLEAN:
                    value = ParseInt();
                    ExpectTag(TAG_BOOLEAN_END);
                    return value;

                case TAG_INT:
                    value = ParseInt();
                    ExpectTag(TAG_INT_END);
                    return value;

                case TAG_LONG:
                    value = ParseLong();
                    ExpectTag(TAG_LONG_END);
                    return value;

                case TAG_DOUBLE:
                    value = (long)ParseDouble();
                    ExpectTag(TAG_DOUBLE_END);
                    return value;

                default:
                    throw ExpectedTag("long", tag);
            }
        }

        /// <summary> Reads a float
        /// <code>
        /// <double>value</double>
        /// </code>
        /// </summary>
        public virtual float ReadFloat()
        {
            return (float)ReadDouble();
        }

        /// <summary>
        /// Reads a double.
        /// <code>
        /// <double>value</double>
        /// </code>
        /// </summary>
        /// <exception cref="CHessianException"/>
        /// <returns>Double value</returns>
        public override double ReadDouble()
        {
            int tag = ParseTag();

            double value;

            switch (tag)
            {
                case TAG_NULL:
                  value = 0;
                  ExpectTag(TAG_NULL_END);
                  return value;
                  
                case TAG_BOOLEAN:
                  value = ParseInt();
                  ExpectTag(TAG_BOOLEAN_END);
                  return value;
                  
                case TAG_INT:
                    value = ParseInt();
                    ExpectTag(TAG_INT_END);
                  return value;
                  
                case TAG_LONG:
                  value = ParseLong();
                  ExpectTag(TAG_LONG_END);
                  return value;
                  
                case TAG_DOUBLE:
                  value = ParseDouble();
                  ExpectTag(TAG_DOUBLE_END);
                  return value;
                default:
                    throw ExpectedTag("double", tag);
            }
        }

        /// <summary> 
        /// Reads a date.
        /// <code>
        /// <date>ISO-8609 date</date>
        /// </code>
        /// <exception cref="CHessianException"/>
        /// </summary>
        public override long ReadUTCDate()
        {
            /* Java:
            int tag = ParseTag();

            if (tag != TAG_DATE)
                throw error("expected date");

            if (_utcCalendar == null)
                _utcCalendar = Calendar.getInstance(TimeZone.getTimeZone("UTC"));

            long value = parseDate(_utcCalendar);

            expectTag(TAG_DATE_END);

            return value;

             */ 


            /* 
            int tag = Read();

            if (tag != PROT_DATE_TYPE)
                throw new CHessianException("expected " + PROT_DATE_TYPE + " for Date at " + (char)tag);

            return this.ParseLong();
             */

            return 0;
        }

         /// <summary> 
        /// Reads a date.
        /// <code>
        /// <date>ISO-8609 date</date>
        /// </code>
        /// <exception cref="CBurlapException"/>
        /// </summary>
        public long ReadLocalDate()
        {
            /* java:
             int tag = parseTag();

    if (tag != TAG_DATE)
      throw error("expected date");

    if (_localCalendar == null)
      _localCalendar = Calendar.getInstance();

    long value = parseDate(_localCalendar);

    expectTag(TAG_DATE_END);

    return value;
             */
            return 0;
        }

        /// <summary>
        /// Reads a string
        /// <code>
        /// <string>value&</string>
        /// </code>
        /// </summary>
        /// <returns></returns>
        public override string ReadString()
        {
            int tag = ParseTag();

            String value;

            switch (tag)
            {
                case TAG_NULL:
                    ExpectTag(TAG_NULL_END);
                    return null;

                case TAG_STRING:
                    m_sBuild.Length = 0;
                    value = ParseString(m_sBuild).ToString();
                    ExpectTag(TAG_STRING_END);
                    return value;

                case TAG_XML:
                    m_sBuild.Length = 0;
                    value = ParseString(m_sBuild).ToString();
                    ExpectTag(TAG_XML_END);
                    return value;

                default:
                    throw ExpectedTag("string", tag);

            }
        }

        /// <summary> Reads an XML node.
        /// 
        /// <code>
        /// <xml>xml string</xml>
        /// </code>
        /// </summary>
        public XmlNode ReadNode()
        {
            int tag = Read();

            switch (tag)
            {

                case 'N':
                    return null;


                case 'S':
                case 's':
                case 'X':
                case 'x':
                    throw new CBurlapException("can't cope");

                default:
                    throw ExpectedTag("string", tag);

            }
        }

        /// <summary>
        /// Reads the byte array
        /// </summary>
        /// <returns>Byte array</returns>
        public override byte[] ReadBytes()
        {
            int tag = ParseTag();

           
            switch (tag) {
                case TAG_NULL:
                  ExpectTag(TAG_NULL_END);
                  return null;

                case TAG_BASE64:
                  byte []data = ParseBytes();
                  ExpectTag(TAG_BASE64_END);

                  return data;
                  
                default:
                  throw ExpectedTag("bytes", tag);
            }


        }


        /// <summary>
        ///  Reads the length 
        /// <code><length>value</length></code>
        /// </summary>
        /// <returns>Length of a list</returns>
        public override int ReadLength()
        {
            int tag = ParseTag();

            if (tag != TAG_LENGTH)
            {
                m_intPeekTag = tag;
                return -1;
            }

            int value = ParseInt();

            ExpectTag(TAG_LENGTH_END);

            return value;
        }

        /// <summary>
        /// Reads a fault.
        /// </summary>
        /// <exception cref="CHessianException"/>
        /// <returns>HashMap with fault details</returns>
        private Hashtable ReadFault()
        {
            Hashtable htFault = new Hashtable();

            int intCode = ParseTag();
            for (; intCode > 0 && intCode != TAG_FAULT_END; intCode = ParseTag())
            {
                m_intPeekTag = intCode;

                object objKey = ReadObject();
                object objValue = ReadObject();

                if (objKey != null && objValue != null)
                    htFault.Add(objKey, objValue);
            }

            if (intCode != PROT_REPLY_END)
                throw ExpectedTag("fault", intCode);

            return htFault;
        }


        /// <summary>
        /// Reads an arbitrary object from the input stream.
        /// </summary>
        /// <param name="expectedType">the expected class if the protocol doesn't supply it.</param>
        /// <returns>Read object</returns>
        /// <exception cref="CBurlapException"/>
        public override object ReadObject(Type expectedType)
        {
            object objResult = null;
            if (expectedType == null || expectedType.Equals(typeof(object)))
            {
                objResult = ReadObject();
            }
            else
            {
                int intTag = ParseTag();
                
                    switch (intTag)
                    {
                            case TAG_NULL:
                                ExpectTag(TAG_NULL_END);
                                return null;
                            case TAG_MAP:
                            {
                                string strType = ReadType();
                                AbstractDeserializer deserializer = this.m_serializerFactory.GetObjectDeserializer(strType);

                                if (expectedType != deserializer.GetOwnType() && expectedType.IsAssignableFrom(deserializer.GetOwnType()))
                                    return deserializer.ReadMap(this);

                                deserializer = m_serializerFactory.GetDeserializer(expectedType);


                                return deserializer.ReadMap(this);
                            }
                        case TAG_REF:
                            {
                                int intRefNumber = ParseInt();
                                ExpectTag(TAG_REF_END);
                                return m_arrRefs[intRefNumber];
                            }                       
                            /*
                        case TAG_REMOTE:
                            {
                                string type = ReadType();
                                string url = ReadString();

                                ExpectTag(TAG_REMOTE_END);

                                Object remote = ResolveRemote(type, url);

                                return remote;
                            }
                             */
                        case TAG_LIST:
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

                    m_intPeekTag = intTag;

                    objResult = m_serializerFactory.GetDeserializer(expectedType).ReadObject(this);
                
            }
            return objResult;
        }


        /// <summary>
        /// Reads an arbitrary object from the input stream.
        /// </summary>
        /// <returns>Read object</returns>
        /// <exception cref="CHessianException"/>
        public override object ReadObject()
        {
            int intTag = ParseTag();

            switch (intTag)
            {
                case TAG_NULL:
                    ExpectTag(TAG_NULL_END);
                    return null;
                case TAG_BOOLEAN:
                    {
                        int value = ParseInt();
                        ExpectTag(TAG_BOOLEAN_END);
                        return value != 0;
                    }
                case TAG_INT:
                    {
                        int value = ParseInt();
                        ExpectTag(TAG_INT_END);
                        return (System.Int32)value;
                    }

                case TAG_LONG:
                    {
                        long value = ParseLong();
                        ExpectTag(TAG_LONG_END);
                        return value;
                    }
                case TAG_DOUBLE:
                    {
                        double value = ParseDouble();
                        ExpectTag(TAG_DOUBLE_END);
                        return value;
                    }

                case TAG_DATE:
                    {
                        long value = ParseDate();
                        ExpectTag(TAG_DATE_END);
                        return new System.DateTime(value);
                    }

                case TAG_XML:
                    {
                        return ParseXML();
                    }
                case TAG_STRING:
                    {
                        m_sBuild.Length = 0;
                        String value = ParseString(m_sBuild).ToString();
                        ExpectTag(TAG_STRING_END);
                        return value;
                    }
                case TAG_BASE64:
                    {
                        byte[] data = ParseBytes();
                        ExpectTag(TAG_BASE64_END);
                        return data;
                    }
                case TAG_LIST:
                    {                 
                        string strType = this.ReadType();
                        int intLength = this.ReadLength();
                        return m_serializerFactory.ReadList(this, intLength, strType);
                    }
                case TAG_MAP:
                    {
                        String strType = ReadType();
                        return m_serializerFactory.ReadMap(this, strType);                       
                    }
                 case TAG_REF:
                    {
                      int intRef = ParseInt();
                      ExpectTag(TAG_REF_END);
                      return m_arrRefs[intRef];
                    }                   
                case TAG_REMOTE:
                    {
                     
                        throw new CBurlapException("REMOTE is not implimented");
                        /*
                        //TODO:
                        String strType = ReadType();
                        String strUrl = ReadString();
                        ExpectTag(TAG_REMOTE_END);
                        return ResolveRemote(strType, strUrl);
                         */
                    }              
                default:
                    throw new CHessianException("unknown code:" + TagName(intTag));
            }
        }

        /// <summary>
        /// Reads a reference.
        /// </summary>
        public object ReadRemote()
        {
            throw new CBurlapException("ReadRemote is not implimented");
            /*
            string type = ReadType();
            String url = ReadString();
            return ResolveRemote(type, url);
             */
        }

    
        /// <summary>
        /// Reads a remote object.
        /// </summary>
        public override object ReadRef()
        {
            return m_arrRefs[ParseInt()];
        }

        /// <summary>
        /// Reads the start of a map.
        /// </summary>
        public override int ReadMapStart()
        {
             return ParseTag();
        }

        /// <summary>
        /// Reads the start of a list.
        /// </summary>
        public override int ReadListStart()
        {
            return ParseTag();
        }


        /// <summary>
        /// Returns true if this is the end of a list or a map.
        /// </summary>
        public override bool IsEnd()
        {
            int code = ParseTag();

            m_intPeekTag = code;
            return (code < 0 || code >= 100);
        }


        /// <summary>
        /// Returns true if this is the end of a list or a map.
        /// </summary>
        public override void ReadEnd()
        {
            int code = ParseTag();

            if (code < 100)
                throw new CBurlapException("unknown code:" + (char) code);
        }
       
        /// <summary>
        /// Reads the end of the map
        /// </summary>
        public override void ReadMapEnd()
        {
            ExpectTag(TAG_MAP_END);
        }

        /// <summary>
        /// Reads the end of the map
        /// </summary>
        public override void ReadListEnd()
        {
            ExpectTag(TAG_LIST_END);
        }

        /// <summary>
        /// Adds a list/map reference.
        /// </summary>
        public override int AddRef(Object obj)
        {
            if (m_arrRefs == null)
				m_arrRefs = new ArrayList();
			
			m_arrRefs.Add(obj);

			return m_arrRefs.Count -1;
        }
      
        /// <summary>
        /// Adds a list/map reference.
        /// </summary>
        public void SetRef(int i, Object obj)
        {
            m_arrRefs[i] = obj;
        }


        /// <summary>
        ///   Parses a type from the stream.
        /// <code>
        /// <type>type</type>
        /// </code>
        /// </summary>
        public override String ReadType()
        {
            int intCode = ParseTag();

            if (intCode != TAG_TYPE)
            {
                m_intPeekTag = intCode;
                return "";
            }

            m_sBuild.Length = 0;
            int ch;
            while ((ch = ReadChar()) >= 0)
                m_sBuild.Append((char)ch);
            String type = m_sBuild.ToString();

            ExpectTag(TAG_TYPE_END);

            return type;
        }






        protected internal virtual int ParseTag()
        {
            if (m_intPeekTag >= 0)
            {
                int tag = m_intPeekTag;
                m_intPeekTag = -1;
                return tag;
            }

            int ch = SkipWhitespace();
            int endTagDelta = 0;
            
            if (ch != '<')
                throw ExpectedChar("'<'", ch);
            

            ch = Read();
            if (ch == '/')
            {
                endTagDelta = 100;
                ch = m_srInput.ReadByte();
            }
            
                   
            if (! IsTagChar(ch))
                throw ExpectedChar("tag", ch);


            m_sBuild.Length = 0;
            for (; IsTagChar(ch); ch = Read())
                m_sBuild.Append((char)ch);
         
            if (ch != '>')
                throw ExpectedChar("'>'", ch);


            object value = (object)m_tagMap[m_sBuild.ToString()];
           
        
            if (value == null)
                throw new CBurlapException("Unknown tag <" + m_sBuild.ToString() + ">");

            int intValue = Convert.ToInt32(value);
            return intValue + endTagDelta;
        }

        protected internal virtual int SkipWhitespace()
        {
            int ch = Read();

            for (;
                 ch == ' ' || ch == '\t' || ch == '\n' || ch == '\r';
                 ch = Read())
            {
            }

            return ch;
        }

        protected internal virtual bool IsWhitespace(int ch)
        {
            return ch == ' ' || ch == '\t' || ch == '\n' || ch == '\r';
        }


       
        public override Stream ReadInputStream()
        {
            return null;
        }
        #endregion
        /*
         private static int []base64Decode;
  
           private static HashMap _tagMap;

  private static Field _detailMessageField;
  
  protected SerializerFactory _serializerFactory;
  
  protected ArrayList _refs;
  
  // the underlying input stream
  private InputStream _is;
  // a peek character
  protected int _peek = -1;
  
  // the method for a call
  private String _method;

  private int _peekTag;

  private Throwable _replyFault;

  protected StringBuffer _sbuf = new StringBuffer();
  protected StringBuffer _entityBuffer = new StringBuffer();
  
  protected Calendar _utcCalendar;
  protected Calendar _localCalendar;
         */

    }
}
