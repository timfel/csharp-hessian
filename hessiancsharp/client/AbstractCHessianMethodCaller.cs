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
* You can find all contact information on http://www.hessiancsharp.com	
******************************************************************************************************
*
*
******************************************************************************************************
* Last change: 
* 2005-08-14 Licence added  (Andre Voltmann)
* 2005-12-16 Session Cookie added (Dimitri Minich)
* 2007-12-08 Proxy support (Matthias Wuttke)
* 2007-12-14 GZIP compression (Matthias Wuttke)
* 2008-01-20 Keep-Alive retry (Matthias Wuttke)
* 
******************************************************************************************************
*/
using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Reflection;
#if !COMPACT_FRAMEWORK
using System.Web;
using System.Web.SessionState;
using System.IO.Compression;
#endif

using hessiancsharp.io;
using System.Text;
using System.Net.Sockets;

namespace hessiancsharp.client
{
	/// <summary>
	/// Zusammenfassung für CHessianMethodCaller.
	/// </summary>
	public abstract class AbstractCHessianMethodCaller
	{
        public const string CUSTOM_HEADER_KEY = "__CUSTOM_HEADERS";
		protected CHessianProxyFactory m_CHessianProxyFactory;
		protected Uri m_uriHessianServiceUri;
        protected NetworkCredential m_credentials = null;

		public virtual Uri URI 
		{
			get { return m_uriHessianServiceUri; }

		}

        public AbstractCHessianMethodCaller() { }

		public AbstractCHessianMethodCaller(CHessianProxyFactory hessianProxyFactory, Uri uri)
		{
			this.m_CHessianProxyFactory = hessianProxyFactory;
			this.m_uriHessianServiceUri = uri;
		}

        public AbstractCHessianMethodCaller(CHessianProxyFactory hessianProxyFactory, Uri uri, string username, string password)
        {
            this.m_CHessianProxyFactory = hessianProxyFactory;
            this.m_uriHessianServiceUri = uri;
            this.m_credentials = new System.Net.NetworkCredential(username, password);
        }

        /// <summary>
        /// Translates the method call to a request byte array.
        /// </summary>
        /// <param name="arrMethodArgs"></param>
        /// <param name="methodInfo"></param>
        /// <returns></returns>
        protected byte[] GetRequestBytes(object[] arrMethodArgs, MethodInfo methodInfo)
        {
            Type[] argumentTypes = GetArgTypes(arrMethodArgs);
            MemoryStream memoryStream = new MemoryStream(4096);
            CHessianOutput cHessianOutput = this.GetHessianOutput(memoryStream);
            string strMethodName = methodInfo.Name;
            if (m_CHessianProxyFactory.IsOverloadEnabled)
            {
                if (arrMethodArgs != null)
                    strMethodName = strMethodName + "__" + arrMethodArgs.Length;
                else
                    strMethodName = strMethodName + "__0";
            }
            cHessianOutput.Call(strMethodName, arrMethodArgs);
            return memoryStream.ToArray();
        }

        /// <summary>
        /// Reads a HTTP fault and throws a CHessianException.
        /// </summary>
        /// <param name="webResponse"></param>
        protected void ReadAndThrowHttpFault(WebResponse webResponse)
        {
            StringBuilder sb = new StringBuilder();
            int chTemp;

            Stream sInStream = webResponse.GetResponseStream();
            if (sInStream != null)
            {
                while ((chTemp = sInStream.ReadByte()) >= 0)
                    sb.Append((char)chTemp);
                sInStream.Close();
            }

            throw new CHessianException(sb.ToString());
        }

        /// <summary>
        /// Prepares a WebRequest object for communication
        /// with the Hessian server.
        /// </summary>
        /// <returns></returns>
        protected virtual WebRequest PrepareWebRequest(long contentLength)
        {
            WebRequest webRequest = OpenConnection(m_uriHessianServiceUri);
            webRequest.ContentType = "text/xml";
            webRequest.Method = "POST";
            return webRequest;
        }

		/// <summary>
		/// Returns array with types of the instance from 
		/// the argument array
		/// </summary>
		/// <param name="arrArgs">Any array</param>
		/// <returns>Array with types of the instance from 
		/// the argument array</returns>
		public static Type[] GetArgTypes(object[] arrArgs) 
		{
			if (null == arrArgs) 
			{
				return new Type[0];
			}

			Type[] result = new Type[arrArgs.Length];
			for (int i = 0; i < result.Length; ++i) 
			{
				if (arrArgs[i] == null) 
				{
					result[i] = null; 
				} 
				else 
				{
					result[i] = arrArgs[i].GetType();
				}
			}

			return result;
		}

		/// <summary>
		/// Creates the URI connection.
		/// </summary>
		/// <param name="uri">Uri for connection</param>
		/// <returns>Request instance</returns>
        protected virtual WebRequest OpenConnection(Uri uri) 
		{
            WebRequest request = WebRequest.Create(uri);
            if (this.m_credentials != null)
                request.Credentials = this.m_credentials;
            return request;
		}

		/// <summary>
		/// Instantiation of the hessian input (not cached) 
		/// </summary>
		/// <param name="stream">Stream for HessianInput-Instantiation</param>
		/// <returns>New HessianInput - Instance</returns>
		protected AbstractHessianInput GetHessianInput(Stream stream) 
		{
			return new CHessianInput(stream);
		}


		/// <summary>
		/// Instantiation of the hessian output (not cached)
		/// </summary>
		/// <param name="stream">Strean for HessianOutput - Instantiation</param>
		/// <returns>New HessianOutput - Instance</returns>
		protected CHessianOutput GetHessianOutput(Stream stream) 
		{
			CHessianOutput cHessianOut = new CHessianOutput(stream);
			return cHessianOut;
		}
    }
}
