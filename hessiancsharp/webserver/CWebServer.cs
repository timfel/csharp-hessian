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
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Threading;
#endregion

namespace hessiancsharp.webserver 
{
	/// <summary>
	/// An embeded tiny webserver.
	/// 
	/// CWebServer web = new CWebServer(5667,"/hallo/test/math.hessian",typeof(CMath));
	/// web.Paranoid = true;
	///	web.AcceptClient("[\\d\\s]");	
	/// web.Run();
	/// or
	/// IMath myMath = new Math();
	/// CWebServer web = new CWebServer(5667,"/hallo/test/math.hessian",typeof(IMath),myMath);
	/// web.Paranoid = true;
	/// web.AcceptClient("127.0.0.1");
	/// web.Run();
	/// </summary>
	public class CWebServer {
		#region CLASS_FIELDS

		private int m_port;
		private Type m_apiType = null;
		private Object m_Service;
		private Socket m_serverSock;
		private string m_serviceUrl;
		private IList m_AcceptIPAddressList = new ArrayList();
		private IList m_DenyIPAddressList = new ArrayList();
		private bool m_paranoid_IP = true;
		private bool m_ready = false;
		private Thread runningThread = null;
		
		#endregion

		#region CONSTRUCTORS

		/// <summary>
		/// Constructor. Creates a web server at the specified port number for the specified Service.
		/// </summary>
		/// <param name="port_">Portnumber to them the server listening</param>
		/// <param name="serviceUrl_">The serviceUrl "/test/myservice.hessian"</param>
		/// <param name="apiType_">The Type of Serviceobject, may not be an interface</param>	
		/// <exception cref="ArgumentException"/>	
		public CWebServer(int port_, string serviceUrl_, Type apiType_) 
		{
			m_serviceUrl = serviceUrl_;
			m_port = port_;
			m_apiType = apiType_;
			if(apiType_.IsInterface || apiType_.IsAbstract) {
				throw new ArgumentException("apiType_ should be a class!");	
			} else {
				m_Service = Activator.CreateInstance(apiType_);		
			}							
		}

		/// <summary>
		/// Constructor. Creates a web server at the specified port number for the specified Service.
		/// </summary>
		/// <param name="port_">Portnumber to them the server listening</param>
		/// <param name="serviceUrl_">The serviceUrl "/test/myservice.hessian"</param>
		/// <param name="apiType_">The Type of Serviceobject</param>
		/// <param name="service_">Servicobject</param>		
		public CWebServer(int port_, string serviceUrl_, Type apiType_, Object service_) 
		{
			m_serviceUrl = serviceUrl_;
			m_port = port_;
			m_apiType = apiType_;
			m_Service = service_;
		}
		#endregion

		#region PROPORTIES
		/// <summary>
		/// The portnumber
		/// </summary>
		public int Port 
		{
			get { return m_port; }
		}
		/// <summary>
		/// The Service Object
		/// </summary>
		public Object Service 
		{
			get { return m_Service; }
		}

		/// <summary>
		/// The Api Type 
		/// </summary>
		public Type TypeApi 
		{
			get { return m_apiType; }
		}

		/// <summary>
		/// informational, is the web server running
		/// </summary>
		public bool Running 
		{
			get { return m_ready; }
		}

		/// <summary>
		/// The Service Url "/test/myservice.hessian"
		/// </summary>
		public string ServiceUrl 
		{
			get { return m_serviceUrl; }
		}
		
		/// <summary>
		/// Switch client filtering on/off.
		/// see AcceptClient(string)
		/// see DenyClient(string)
		/// </summary>
		public bool Paranoid 
		{
			get { return m_paranoid_IP; }
			set { m_paranoid_IP = value; }
		}
		#endregion

		#region PUBLIC_METHODS
		
		/// <summary>
		/// listen to port and get HTTP calls
		/// </summary>
		public void Run() 
		{
			if (runningThread != null) 
			{
				runningThread.Abort();
				m_serverSock.Close();
				m_ready = false;
			}
			runningThread = new Thread(new ThreadStart(RunConnectionThread));
			runningThread.IsBackground = true;
			runningThread.Start();
		}

		/// <summary>
		/// stop web server
		/// </summary>
		public void Stop() 
		{
			if (runningThread != null) 
			{
				try 
				{
					runningThread.Abort();
				} 
				finally 
				{
					runningThread = null;
					m_serverSock.Close();
					m_serverSock = null;
					m_ready = false;
				}
			}
		}

		/// <summary>
		///Add an IP address to the list of accepted clients. The parameter can
		///contain '*' as wildcard character, e.g. "192.168.*.*", just a regular expression. 
		///You must call Paranoid = true in order for this to have any effect.
		/// </summary>
		public void AcceptClient(string regexpr_) 
		{			
			if (!m_AcceptIPAddressList.Contains(regexpr_)) 
			{
				m_AcceptIPAddressList.Add(regexpr_);
			}			
		}

		/// <summary>
		///Add an IP address to the list of denied clients. The parameter can
		///contain '*' as wildcard character, e.g. "192.168.*.*", just a regular expression. 
		///You must call Paranoid = true in order for this to have any effect.
		/// </summary>
		public void DenyClient(string regexpr_) 
		{		
			if (!m_DenyIPAddressList.Contains(regexpr_)) 
			{
				m_DenyIPAddressList.Add(regexpr_);
			}		
		}
		#endregion		

		/// <summary>
		/// the threadstart of the web server. called by Run()
		/// </summary>
		protected void RunConnectionThread() {
			//Establish the listen socket			
			m_serverSock = new Socket(AddressFamily.InterNetwork,
			                          SocketType.Stream, ProtocolType.Tcp);

			IPEndPoint ipe = new IPEndPoint(IPAddress.Any, m_port);

			m_serverSock.Bind(ipe);
			m_ready = true;

			while (true) {
				Socket sock = null;
				try {
					sock = AcceptConnection();
				} catch (NotSupportedException) {
					//TODO: Log deny ip
					sock.Close();
					sock = null;
				}
				if (sock != null) {
					CConnection conn = new CConnection(sock, m_serviceUrl, m_apiType, m_Service);
					conn.ProcessRequest();
				}
			}
		}
	
		/// <summary>
		/// Checks incoming connections to see if they should be allowed.
		/// If not in paranoid mode, always returns true.
		/// </summary>
		/// <param name="s">The socket to inspect</param>
		/// <returns>Whether the connection should be allowed</returns>
		protected bool AllowConnection(Socket s)
		{
			if (!Paranoid)
			{
				return true;
			}

			string addresAsStr = ((IPEndPoint) s.RemoteEndPoint).Address.ToString();

			int l = m_DenyIPAddressList.Count;			
			for (int i = 0; i < l; i++)
			{
				Regex ipPattern=new Regex(Convert.ToString(m_DenyIPAddressList[i]));
				if(ipPattern.IsMatch(addresAsStr)) {
					return false;
				}								
			}
			l = m_AcceptIPAddressList.Count;
			for (int i = 0; i < l; i++)
			{
				string reg = Convert.ToString(m_AcceptIPAddressList[i]);
				Regex ipPattern=new Regex(reg);
				if(ipPattern.IsMatch(addresAsStr)) 
				{
					return true;
				}					
			}
			return false;
		}

		/// <summary>
		/// accept a connection and return a Socket
		/// </summary>
		/// <returns></returns>
		private Socket AcceptConnection() 
		{
			m_serverSock.Listen(1);
			Socket s = m_serverSock.Accept();
			IPEndPoint ep = (IPEndPoint) s.RemoteEndPoint;
			IPAddress remoteIp = ep.Address;
			if(!AllowConnection(s)) 
			{
				throw new NotSupportedException("The client with this ip is not supported: "
					+ remoteIp.ToString());
			}			
			return s;
		}
	}
}