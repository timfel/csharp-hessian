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
* 2005-12-26 initial class definition by Dimitri Minich.
* ....
******************************************************************************************************
*/

#region NAMESPACES
using System;
using System.Collections;
using System.IO;
using hessiancsharp.io;
#endregion

namespace burlapcsharp.io
{
    class CBurlapOutput : AbstractBurlapOutput
    {

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

        /*Java:
         
         * // the output stream
  protected OutputStream os;
  // map of references
  private IdentityHashMap _refs;

  private Date date;
  private Calendar utcCalendar;
  private Calendar localCalendar;
         * 
         * */

        #region CONSTRUCTORS
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="srOutput">Output stream</param>
        public CBurlapOutput(Stream srOutput) 
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
        public virtual void Call(string strMethod, object[] args)
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


        /// <summary> Starts the method call.  Clients would use <code>startCall</code>
        /// instead of <code>call</code> if they wanted finer control over
        /// writing the arguments, or needed to write headers.
        /// 
        /// <code><pre>
        /// <burlap:call>
        /// <method>method-name</method>
        /// </pre></code>
        /// 
        /// </summary>
        /// <param name="method">the method name to call.
        /// </param>
        public override void StartCall(String method)
        {
            Print("<burlap:call><method>");
            Print(method);
            Print("</method>");
        }

        /// <summary> Starts the method call.  Clients would use <code>startCall</code>
        /// instead of <code>call</code> if they wanted finer control over
        /// writing the arguments, or needed to write headers.
        /// 
        /// <code><pre>
        /// <method>method-name</method>
        /// </pre></code>
        /// 
        /// </summary>
        /// <param name="method">the method name to call.
        /// </param>
        public void StartCall()
        {
            Print("<burlap:call>");
        }

        /// <summary> Writes the method for a call.
        /// 
        /// <code><pre>
        /// <method>value</method>
        /// </pre></code>
        /// 
        /// </summary>
        /// <param name="method">the method name to call.
        /// </param>
        public virtual void WriteMethod(String method)
        {
            Print("<method>");
            Print(method);
            Print("</method>");
        }

        /// <summary> Completes.
        /// 
        /// <code><pre>
        /// </burlap:call>
        /// </pre></code>
        /// </summary>
        public override void CompleteCall()
        {
            Print("</burlap:call>");
        }


        /// <summary> Starts the reply
        /// 
        /// <p>A successful completion will have a single value:
        /// 
        /// </summary>
        public override void StartReply()
        {
            Print("<burlap:reply>");
        }

        /// <summary> Completes reading the reply
        /// 
        /// <p>A successful completion will have a single value:
        /// 
        /// <pre>
        /// </burlap:reply>
        /// </pre>
        /// </summary>
        public override void CompleteReply()
        {
            Print("</burlap:reply>");
        }

        /// <summary> Writes a header name.  The header value must immediately follow.
        /// 
        /// <code><pre>
        /// <header>foo</header><int>value<;/int>
        /// </pre></code>
        /// </summary>
        public virtual void WriteHeader(String name)
        {
            Print("<header>");
            PrintString(name);
            Print("</header>");
        }

        /// <summary> Writes a fault.  The fault will be written
        /// as a descriptive string followed by an object:
        /// 
        /// <code><pre>
        /// <fault>
        /// <string>code
        /// <string>the fault code
        /// 
        /// <string>message
        /// <string>the fault mesage
        /// 
        /// <string>detail
        /// <map>t\x00\xnnjavax.ejb.FinderException
        /// ...
        /// </map>
        /// </fault>
        /// </pre></code>
        /// 
        /// </summary>
        /// <param name="code">the fault code, a three digit
        /// </param>
        public override void WriteFault(System.String code, System.String message, System.Object detail)
        {
            Print("<fault>");
            WriteString("code");
            WriteString(code);

            WriteString("message");
            WriteString(message);

            if (detail != null)
            {
                WriteString("detail");
                WriteObject(detail);
            }
            Print("</fault>");
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

        /// <summary> Writes the list header to the stream.  List writers will call
        /// <code>writeListBegin</code> followed by the list contents and then
        /// call <code>writeListEnd</code>.
        /// 
        /// <code><pre>
        /// <list>
        /// <type>ArrayList</type>
        /// <length>3</length>
        /// <int>1</int>
        /// <int>2</int>
        /// <int>3</int>
        /// </list>
        /// </pre></code>
        /// </summary>
        public override void WriteListBegin(int length, String type)
        {
            Print("<list><type>");
            if (type != null)
                Print(type);
            Print("</type><length>");
            Print(length);
            Print("</length>");
        }

        /// <summary> Writes the tail of the list to the stream.</summary>
        public override void WriteListEnd()
        {
            Print("</list>");
        }


        /// <summary> Writes the map header to the stream.  Map writers will call
        /// <code>writeMapBegin</code> followed by the map contents and then
        /// call <code>writeMapEnd</code>.
        /// 
        /// <code><pre>
        /// <map>
        /// <type>type</type>
        /// (<key> <value>)*
        /// </map>
        /// </pre></code>
        /// </summary>
        public override void WriteMapBegin(System.String type)
        {
            Print("<map><type>");
            if (type != null)
                Print(type);

            Print("</type>");
        }

        /// <summary> 
        /// Writes the tail of the map to the stream.
        /// </summary>
        public override void WriteMapEnd()
        {
            Print("</map>");
        }

        /// <summary> Writes a remote object reference to the stream.  The type is the
        /// type of the remote interface.
        /// 
        /// <code><pre>
        /// <remote>
        /// <type>test.account.Account</type>
        /// <string>http://caucho.com/foo;ejbid=bar</string>
        /// </remote>
        /// </pre></code>
        /// </summary>
        public override void WriteRemote(String type, String url)
        {
            Print("<remote><type>");
            Print(type);
            Print("</type><string>");
            Print(url);
            Print("</string></remote>");
        }


        /// <summary> Writes a boolean value to the stream.  The boolean will be written
        /// with the following syntax:
        /// 
        /// <code><pre>
        /// <boolean>0</boolean>
        /// <boolean>1</boolean>
        /// </pre></code>
        /// 
        /// </summary>
        /// <param name="bValue">the boolean value to write.
        /// </param>
        public override void WriteBoolean(bool bValue)
        {
            if (bValue)
                Print("<boolean>1</boolean>");
            else
                Print("<boolean>0</boolean>");
        }

        /// <summary> Writes an integer value to the stream.  The integer will be written
        /// with the following syntax:
        /// 
        /// <code><pre>
        /// <int>int value</int>
        /// </pre></code>
        /// 
        /// </summary>
        /// <param name="value">the integer value to write.
        /// </param>
        public override void WriteInt(int intValue)
        {
            Print("<int>");
            Print(intValue);
            Print("</int>");
        }

        /// <summary> Writes a long value to the stream.  The long will be written
        /// with the following syntax:
        /// 
        /// <code><pre>
        /// <long>int value</long>
        /// </pre></code>
        /// 
        /// </summary>
        /// <param name="value">the long value to write.
        /// </param>
        public override void WriteLong(long lValue)
        {
            Print("<long>");
            Print(lValue);
            Print("</long>");
        }

        /// <summary> Writes a double value to the stream.  The double will be written
        /// with the following syntax:
        /// 
        /// <code><pre>
        /// <double>value</double>
        /// </pre></code>
        /// 
        /// </summary>
        /// <param name="value">the double value to write.
        /// </param>
        public override void WriteDouble(double dValue)
        {
            Print("<double>");
            Print(dValue);
            Print("</double>");
        }

        /// <summary> Writes a date to the stream.
        /// 
        /// <code><pre>
        /// <date>iso8901</date>
        /// </pre></code>
        /// 
        /// </summary>
        /// <param name="time">the date in milliseconds from the epoch in UTC
        /// </param>
        public override void WriteUTCDate(long time)
        {
            //TODO:
            /* 
            Print("<date>");
            if (utcCalendar == null)
            {
                //UPGRADE_ISSUE: Method 'java.util.TimeZone.getTimeZone' was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1000_javautilTimeZonegetTimeZone_javalangString'"
                TimeZone.getTimeZone("UTC");
                utcCalendar = new System.Globalization.GregorianCalendar();
                date = System.DateTime.Now;
            }

            //UPGRADE_TODO: Method 'java.util.Date.setTime' was converted to 'System.DateTime.DateTime' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilDatesetTime_long'"
            date = new System.DateTime(time);
            //UPGRADE_TODO: The differences in the format  of parameters for method 'java.util.Calendar.setTime'  may cause compilation errors.  "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1092'"
            SupportClass.CalendarManager.manager.SetDateTime(utcCalendar, date);

            PrintDate(utcCalendar);
            Print("</date>");
             * */
        }

        /// <summary> Writes a null value to the stream.
        /// The null will be written with the following syntax
        /// 
        /// <code><pre>
        /// <null></null>
        /// </pre></code>
        /// 
        /// </summary>
        /// <param name="value">the string value to write.
        /// </param>
        public override void WriteNull()
        {
            Print("<null></null>");
        }

        /// <summary> Writes a string value to the stream using UTF-8 encoding.
        /// The string will be written with the following syntax:
        /// 
        /// <code><pre>
        /// <string>string-value</string>
        /// </pre></code>
        /// 
        /// If the value is null, it will be written as
        /// 
        /// <code><pre>
        /// <null></null>
        /// </pre></code>
        /// 
        /// </summary>
        /// <param name="value">the string value to write.
        /// </param>
        public override void WriteString(String sValue)
        {
            if (sValue == null)
            {
                Print("<null></null>");
            }
            else
            {
                Print("<string>");
                PrintString(sValue);
                Print("</string>");
            }
        }

        /// <summary> Writes a string value to the stream using UTF-8 encoding.
        /// The string will be written with the following syntax:
        /// 
        /// <code><pre>
        /// S b16 b8 string-value
        /// </pre></code>
        /// 
        /// If the value is null, it will be written as
        /// 
        /// <code><pre>
        /// N
        /// </pre></code>
        /// 
        /// </summary>
        /// <param name="value">the string value to write.
        /// </param>
        public override void WriteString(char[] buffer, int offset, int length)
        {
            if (buffer == null)
            {
                Print("<null></null>");
            }
            else
            {
                Print("<string>");
                PrintString(buffer, offset, length);
                Print("</string>");
            }
        }

        /// <summary> Writes a byte array to the stream.
        /// The array will be written with the following syntax:
        /// 
        /// <code><pre>
        /// <base64>bytes</base64>
        /// </pre></code>
        /// 
        /// If the value is null, it will be written as
        /// 
        /// <code><pre>
        /// <null></null>
        /// </pre></code>
        /// 
        /// </summary>
        /// <param name="value">the string value to write.
        /// </param>
        public override void WriteBytes(byte[] buffer)
        {
            if (buffer == null)
                Print("<null></null>");
            else
                WriteBytes(buffer, 0, buffer.Length);
        }

        /// <summary> Writes a byte array to the stream.
        /// The array will be written with the following syntax:
        /// 
        /// <code><pre>
        /// <base64>bytes</base64>
        /// </pre></code>
        /// 
        /// If the value is null, it will be written as
        /// 
        /// <code><pre>
        /// <null></null>
        /// </pre></code>
        /// 
        /// </summary>
        /// <param name="value">the string value to write.
        /// </param>
        public override void WriteBytes(byte[] buffer, int offset, int length)
        {
            if (buffer == null)
            {
                Print("<null></null>");
            }
            else
            {
                Print("<base64>");

                int i = 0;
                for (; i + 2 < length; i += 3)
                {
                    if (i != 0 && (i & 0x3f) == 0)
                        Print('\n');

                    int v = (((buffer[offset + i] & 0xff) << 16) + ((buffer[offset + i + 1] & 0xff) << 8) + (buffer[offset + i + 2] & 0xff));

                    Print(Encode(v >> 18));
                    Print(Encode(v >> 12));
                    Print(Encode(v >> 6));
                    Print(Encode(v));
                }

                if (i + 1 < length)
                {
                    int v = (((buffer[offset + i] & 0xff) << 8) + (buffer[offset + i + 1] & 0xff));

                    Print(Encode(v >> 10));
                    Print(Encode(v >> 4));
                    Print(Encode(v << 2));
                    Print('=');
                }
                else if (i < length)
                {
                    int v = buffer[offset + i] & 0xff;

                    Print(Encode(v >> 2));
                    Print(Encode(v << 4));
                    Print('=');
                    Print('=');
                }

                Print("</base64>");
            }
        }

        

        /// <summary> Writes a byte buffer to the stream.
        /// 
        /// <code><pre>
        /// b b16 b18 bytes
        /// </pre></code>
        /// </summary>
        public override void WriteByteBufferPart(byte[] buffer, int offset, int length)
        {
            throw new System.NotSupportedException();
        }

        /// <summary> Writes a byte buffer to the stream.
        /// 
        /// <code><pre>
        /// b b16 b18 bytes
        /// </pre></code>
        /// </summary>
        public override void WriteByteBufferEnd(byte[] buffer, int offset, int length)
        {
            throw new System.NotSupportedException();
        }

        /// <summary> Writes a reference.
        /// 
        /// <code><pre>
        /// <ref>int</ref>
        /// </pre></code>
        /// 
        /// </summary>
        /// <param name="value">the integer value to write.
        /// </param>
        public override void WriteRef(int intValue)
        {
            Print("<ref>");
            Print(intValue);
            Print("</ref>");
        }

        /// <summary>
        /// If the object has already been written, just write its ref.     
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
                int t_ref = (int)m_htRefs[objReference];
                int value = t_ref;
                WriteRef(value);
                return true;
            }
            else
            {
                m_htRefs.Add(objReference, m_htRefs.Count);
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
                int value = (int)m_htRefs[objNewReference];
                m_htRefs.Remove(objOldReference);
                m_htRefs.Add(objNewReference, value);
                return true;

            }
            else
            {
                return false;
            }
        }

        /// <summary> Prints a string to the stream, encoded as UTF-8
        /// 
        /// </summary>
        /// <param name="v">the string to print.
        /// </param>
        public virtual void PrintString(String v)
        {
            PrintString(v, 0, v.Length);
        }

        /// <summary> Prints a string to the stream, encoded as UTF-8
		/// 
		/// </summary>
		/// <param name="v">the string to print.
		/// </param>
        public virtual void PrintString(System.String v, int offset, int length)
        {
            for (int i = 0; i < length; i++)
            {
                char ch = v[i + offset];

                if (ch == '<')
                {
                    m_srOutput.WriteByte((System.Byte)'&');
                    m_srOutput.WriteByte((System.Byte)'#');
                    m_srOutput.WriteByte((System.Byte)'6');
                    m_srOutput.WriteByte((System.Byte)'0');
                    m_srOutput.WriteByte((System.Byte)';');
                }
                else if (ch == '&')
                {
                    m_srOutput.WriteByte((System.Byte)'&');
                    m_srOutput.WriteByte((System.Byte)'#');
                    m_srOutput.WriteByte((System.Byte)'3');
                    m_srOutput.WriteByte((System.Byte)'8');
                    m_srOutput.WriteByte((System.Byte)';');
                }
                else if (ch < 0x80)
                    m_srOutput.WriteByte((System.Byte)ch);
                else if (ch < 0x800)
                {
                    m_srOutput.WriteByte((System.Byte)(0xc0 + ((ch >> 6) & 0x1f)));
                    m_srOutput.WriteByte((System.Byte)(0x80 + (ch & 0x3f)));
                }
                else
                {
                    m_srOutput.WriteByte((System.Byte)(0xe0 + ((ch >> 12) & 0xf)));
                    m_srOutput.WriteByte((System.Byte)(0x80 + ((ch >> 6) & 0x3f)));
                    m_srOutput.WriteByte((System.Byte)(0x80 + (ch & 0x3f)));
                }

            }
        }

        /// <summary> Prints a string to the stream, encoded as UTF-8
        /// 
        /// </summary>
        /// <param name="v">the string to print.
        /// </param>
        public virtual void PrintString(char[] v, int offset, int length)
        {
            for (int i = 0; i < length; i++)
            {
                char ch = v[i + offset];

                if (ch < 0x80)
                    m_srOutput.WriteByte((System.Byte)ch);
                else if (ch < 0x800)
                {
                    m_srOutput.WriteByte((System.Byte)(0xc0 + ((ch >> 6) & 0x1f)));
                    m_srOutput.WriteByte((System.Byte)(0x80 + (ch & 0x3f)));
                }
                else
                {
                    m_srOutput.WriteByte((System.Byte)(0xe0 + ((ch >> 12) & 0xf)));
                    m_srOutput.WriteByte((System.Byte)(0x80 + ((ch >> 6) & 0x3f)));
                    m_srOutput.WriteByte((System.Byte)(0x80 + (ch & 0x3f)));
                }
            }
        }

        /// <summary> Prints a date.
        /// 
        /// </summary>
        /// <param name="date">the date to print.
        /// </param>
        public virtual void printDate(DateTime dateTime)
        {
            //Todo: Parameter wurde geändert in DateTime!
            
            int year = dateTime.Year;

            m_srOutput.WriteByte((System.Byte)('0' + (year / 1000 % 10)));
            m_srOutput.WriteByte((System.Byte)('0' + (year / 100 % 10)));
            m_srOutput.WriteByte((System.Byte)('0' + (year / 10 % 10)));
            m_srOutput.WriteByte((System.Byte)('0' + (year % 10)));

            
            int month = dateTime.Month + 1;
            m_srOutput.WriteByte((System.Byte)('0' + (month / 10 % 10)));
            m_srOutput.WriteByte((System.Byte)('0' + (month % 10)));


            int day = dateTime.Day;
            m_srOutput.WriteByte((System.Byte)('0' + (day / 10 % 10)));
            m_srOutput.WriteByte((System.Byte)('0' + (day % 10)));

            m_srOutput.WriteByte((System.Byte)'T');


            int hour = dateTime.Hour;
            m_srOutput.WriteByte((System.Byte)('0' + (hour / 10 % 10)));
            m_srOutput.WriteByte((System.Byte)('0' + (hour % 10)));


            int minute = dateTime.Minute;
            m_srOutput.WriteByte((System.Byte)('0' + (minute / 10 % 10)));
            m_srOutput.WriteByte((System.Byte)('0' + (minute % 10)));


            int second = dateTime.Second;
            m_srOutput.WriteByte((System.Byte)('0' + (second / 10 % 10)));
            m_srOutput.WriteByte((System.Byte)('0' + (second % 10)));


            int ms = dateTime.Millisecond;
            m_srOutput.WriteByte((System.Byte)'.');
            m_srOutput.WriteByte((System.Byte)('0' + (ms / 100 % 10)));
            m_srOutput.WriteByte((System.Byte)('0' + (ms / 10 % 10)));
            m_srOutput.WriteByte((System.Byte)('0' + (ms % 10)));

            m_srOutput.WriteByte((System.Byte)'Z');
        }

        /// <summary> Prints a char to the stream.
        /// 
        /// </summary>
        /// <param name="v">the char to print.
        /// </param>
        protected internal virtual void Print(char v)
        {
            m_srOutput.WriteByte((System.Byte)v);
        }

        /// <summary> Prints an integer to the stream.
        /// 
        /// </summary>
        /// <param name="v">the integer to print.
        /// </param>
        protected internal virtual void Print(int v)
        {
            Print(System.Convert.ToString(v));
        }

        /// <summary> Prints a long to the stream.
        /// 
        /// </summary>
        /// <param name="v">the long to print.
        /// </param>
        protected internal virtual void Print(long v)
        {
            Print(System.Convert.ToString(v));
        }

        /// <summary> Prints a double to the stream.
        /// 
        /// </summary>
        /// <param name="v">the double to print.
        /// </param>
        protected internal virtual void Print(double v)
        {
            Print(System.Convert.ToString(v));
        }

        /// <summary> Prints a string as ascii to the stream.  Used for tags, etc.
        /// that are known to the ascii.
        /// 
        /// </summary>
        /// <param name="str">the ascii string to print.
        /// </param>
        protected internal virtual void Print(String str)
        {
            int len = str.Length;
            for (int i = 0; i < len; i++)
            {
                int ch = str[i];

                m_srOutput.WriteByte((System.Byte)ch);
            }
        }




        #endregion

        /// <summary>
        /// Encodes a digit
        /// </summary>
        private char Encode(int d)
        {
            d &= 0x3f;
            if (d < 26)
                return (char)(d + 'A');
            else if (d < 52)
                return (char)(d + 'a' - 26);
            else if (d < 62)
                return (char)(d + '0' - 52);
            else if (d == 62)
                return '+';
            else
                return '/';
        }
    }
}
