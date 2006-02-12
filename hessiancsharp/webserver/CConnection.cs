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
using System.Net.Sockets;
using System.Text;
using hessiancsharp.io;
using hessiancsharp.server;
#endregion
namespace hessiancsharp.webserver 
{
	/// <summary>
	/// This class handles socketconnections request and respons.
	/// </summary>
	public class CConnection {
		#region CLASS_FIELDS
		private Socket m_socket = null;
		private Type m_apiType = null;
		private Object m_Service;
		private Stream m_stream;
		private string m_serviceUrl;
		private const string serverID = "HessianServer V0.1";
		private static byte[] OK = StrToByteArray(" 200 OK\r\n");
		//For reading Headerlines
		private byte[] m_buffer;
		#endregion

		
		
		#region CONSTRUCTORS
		/// <summary>
		/// Constructor. Creates a web server at the specified port number for the specified Service.
		/// </summary>
		/// <param name="socket_">The socket for client and server</param>
		/// <param name="serviceUrl_">The serviceUrl "/test/myservice.hessian"</param>
		/// <param name="type_">The Type of Serviceobject</param>
		/// <param name="service_">Servicobject</param>	
		public CConnection(Socket socket_, string serviceUrl_, Type type_, Object service_) {
			m_serviceUrl = serviceUrl_;
			m_socket = socket_;
			m_apiType = type_;
			m_Service = service_;
		}
		#endregion

		#region PUBLIC_METHODS
		/// <summary>
		/// The Request will be processed here.
		/// The main function in this class
		/// </summary>
		public void ProcessRequest() 
		{
			//Get references to sockets input & output streams			
			m_stream = new NetworkStream(m_socket, FileAccess.ReadWrite, true);

			MemoryStream memoryStream = new MemoryStream();
			//Reads the Header of HTTP Message
			try {
				ReadHeaders();
			} catch (NotSupportedException e) {
				SendError(500, e.StackTrace);
				m_socket.Close();
				m_stream.Close();
				return;
			}
			AbstractHessianInput inHessian = new CHessianInput(m_stream);
			AbstractHessianOutput tempOutHessian = new CHessianOutput(memoryStream);

			/// Proxy object			
			CHessianSkeleton objectSkeleton = null;
			try {
				objectSkeleton = new CHessianSkeleton(m_apiType, m_Service);
				objectSkeleton.invoke(inHessian, tempOutHessian);
				WriteResponse(memoryStream.GetBuffer());
			} catch (Exception e) {
				SendError(500, e.StackTrace);
			} finally {
				m_socket.Close();
				m_stream.Close();
			}
		}
		#endregion

		/// <summary>
		/// Responsible for writing response
		/// </summary>
		private void WriteResponse(byte[] content_) 
		{
			SendOk(m_stream);
			m_stream.Write(content_, 0, content_.Length);
		}

		/// <summary>
		/// Converts string in a byte Array, encoding is ASCII
		/// </summary>
		protected static byte[] StrToByteArray(string str) {
			ASCIIEncoding encoding = new ASCIIEncoding();
			return encoding.GetBytes(str);
		}

		/// <summary>
		/// return an error to the web browser
		/// </summary>
		/// <param name="errno"></param>
		/// <param name="errString"></param>
		protected void SendError(int errno, string errString) {
			SendString(string.Format("HTTP/1.1 {0} {1}\r\n", errno, errString));
			SendString(string.Format("Date:{0}\r\n", DateTime.Now));
			SendString(string.Format("Server:{0}\r\n", serverID));
			SendString("Content-Type: text/html; charset=utf-8\r\n");
			SendString("Connection: close\r\n");
		}

		/// <summary>
		/// It puts all lines for ok-Header in m_stream
		/// </summary>
		protected void SendOk(Stream ns_) {
			SendString("HTTP/1.1 200 OK\r\n");
			SendString(string.Format("Date:{0}\r\n", DateTime.Now));
			SendString(string.Format("Server:{0}\r\n", serverID));
			SendString("Content-Type: text/html; charset=utf-8\r\n\r\n");
		}

		/// <summary>
		/// It puts the string in m_stream
		/// </summary>
		protected virtual void SendString(String msg_) {
			byte[] buff;
			buff = Encoding.ASCII.GetBytes(msg_);

			try {
				m_stream.Write(buff, 0, buff.Length);
			} catch (Exception) {
				//TODO: Logging
				//log(LogKind.Error, "sendString Exception:{0} ", e);
			}
		}

		/// <summary>
		/// The whole header will be read here
		/// </summary>
		private void ReadHeaders() {
			String line = null;
			do {
				line = ReadHeaderLine();

				if (line != null) {
					String lineLower = line.ToLower();
					if (lineLower.StartsWith("post")) {
						string command = CommandArg(lineLower);
						string serviceUrl = GetServiceUrl(command);
						//Check the URL, it should be the same url like serviceUrl
						if (!CheckServiceUrl(m_serviceUrl, serviceUrl)) {
							throw new NotSupportedException("This ServiceUrl is not supported");
						}

					}
					/*
                    if (lineLower.StartsWith("content-length:"))
                    {
                       
                    }
                    if (lineLower.StartsWith("connection:"))
                    {
                       
                    }
                    if (lineLower.StartsWith("authorization: basic "))
                    {
                      
                    }*/

				}

			} while (line != null && line.Length != 0);
		}

		/// <summary>
		/// It checks the service url and url in the header
		/// </summary>
		private bool CheckServiceUrl(string sholdUrl_, string isUrl_) {
			bool result = false;
			StringBuilder sb = new StringBuilder();
			sb.Append('/');
			sb.Append(sholdUrl_);
			string shouldUrl2 = sb.ToString();

			if (isUrl_.Equals(sholdUrl_) || isUrl_.Equals(shouldUrl2)) {
				return true;
			}
			return result;
		}

		/// <summary>
		/// Finds the command behind Post or Get
		/// </summary>
		protected virtual string CommandArg(string command) {
			int idx = 0;
			//skip blanks before command
			while ((idx < command.Length) && (Char.IsWhiteSpace(command, idx))) {
				idx++;
			}
			if (idx >= command.Length) {
				return string.Empty;
			}

			//skip command
			while ((idx < command.Length) && (!Char.IsWhiteSpace(command, idx))) {
				idx++;
			}
			if (idx >= command.Length) {
				return string.Empty;
			}

			return command.Substring(idx).Trim();
		}

		/// <summary>
		/// Finds the serviceUrl from command
		/// </summary>
		protected virtual string GetServiceUrl(string argument) {
			StringBuilder sb = new StringBuilder();
			CharEnumerator argChar = argument.GetEnumerator();
			while (argChar.MoveNext()) {
				char c = argChar.Current;
				if (c == '?') {
					break;
				}
				if (Char.IsWhiteSpace(c)) {
					break;
				}
				sb.Append(c);
			}
			return sb.ToString();
		}

		/// <summary>
		/// Reads a line of Header
		/// </summary>
		private String ReadHeaderLine() {
			if (m_buffer == null) {
				m_buffer = new byte[2048];
			}
			int next;
			int count = 0;
			for (;; ) {
				next = m_stream.ReadByte();
				if (next < 0 || next == '\n') {
					break;
				}
				if (next != '\r') {
					m_buffer[count++] = (byte) next;
				}
				if (count >= m_buffer.Length) {
					throw new IOException("HTTP Header too long");
				}
			}
			return Encoding.UTF8.GetString(m_buffer, 0, count);

		}


	}
}