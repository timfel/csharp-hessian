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
using hessiancsharp.io;
using System.Net;
#endregion

namespace hessiancsharp.client 
{
	/// <summary>
	/// Factory for Proxy - creation.
	/// </summary>
	public class CHessianProxyFactory {
		#region CLASS_FIELDS
		/// <summary>
		/// flag, that allows or not the overloaded methods (using mangling)
		/// </summary>
		private bool m_blnIsOverloadEnabled = false;

        private string m_password;
        private string m_username;
        private WebProxy m_webproxy;

        #endregion

        #region Contructors
        public CHessianProxyFactory()
        {
            m_username = null;
            m_password = null;
        }

        public CHessianProxyFactory(string username, string password)
        {
            m_username = username;
            m_password = password;
        }

        public CHessianProxyFactory(string username, string password, WebProxy webproxy)
        {
            m_username = username;
            m_password = password;
            m_webproxy = webproxy;
        }
		#endregion

		#region PROPERTIES
		/// <summary>
		/// Returns of Sets flag, that allows or not the overloaded methods (using mangling)
		/// </summary>
		public bool IsOverloadEnabled
		{
			get { return m_blnIsOverloadEnabled; }
			set { m_blnIsOverloadEnabled = value; }
		}
		#endregion

		#region PUBLIC_METHODS
		

		/// <summary>
		/// Creates a new proxy with the specified URL.  The returned object
		/// is a proxy with the interface specified by api.
		/// <code>
		/// string url = "http://localhost:8080/ejb/hello");
		/// HelloHome hello = (HelloHome) factory.create(HelloHome.class, url);
		/// </code>
		/// </summary>
		/// <param name="type">the interface the proxy class needs to implement</param>
		/// <param name="strUrl">the URL where the client object is located</param>
		/// <returns>a proxy to the object with the specified interface</returns>
		public Object Create(Type type, string strUrl) 
		{
			return CreateHessianStandardProxy(strUrl, type);
		}
	

		/// <summary>
		/// Creates proxy object using .NET - Remote proxy framework
		/// </summary>
		/// <param name="type">the interface the proxy class needs to implement</param>
		/// <param name="strUrl">the URL where the client object is located</param>
		/// <returns>a proxy to the object with the specified interface</returns>
		private object CreateHessianStandardProxy(string strUrl, Type type)
		{
            

			#if COMPACT_FRAMEWORK
			// do CF stuff
			throw new CHessianException("not supported in compact version");		
			#else
            return new CHessianProxyStandardImpl(type, this, new Uri(strUrl), m_username, m_password, m_webproxy).GetTransparentProxy();
			#endif			
		}

		#endregion
	}
}