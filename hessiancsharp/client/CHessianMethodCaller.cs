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
	public class CHessianMethodCaller : AbstractCHessianMethodCaller
	{
        public const bool USE_GZIP_COMPRESSION = true;
        protected WebProxy m_proxy = null; // null = system default

		public CHessianMethodCaller(CHessianProxyFactory hessianProxyFactory, Uri uri) : base (hessianProxyFactory, uri) {}
        public CHessianMethodCaller(CHessianProxyFactory hessianProxyFactory, Uri uri, string username, string password) : base (hessianProxyFactory, uri, username, password) {}        
        
        public CHessianMethodCaller(CHessianProxyFactory hessianProxyFactory, Uri uri, string username, string password, WebProxy proxy)
        {
            this.m_CHessianProxyFactory = hessianProxyFactory;
            this.m_uriHessianServiceUri = uri;
            this.m_credentials = new System.Net.NetworkCredential(username, password);
            this.m_proxy = proxy;
        }

		/// <summary>
		/// This method wrapps an instance call to the hessian 
		/// requests, sends it to the hessian service and translates the reply of this call to the C# - data type
		/// </summary>
		/// <param name="methodInfo">The method to call</param>
		/// <param name="arrMethodArgs">The arguments to the method call</param>
		/// <returns>Invocation result</returns>
		public object DoHessianMethodCall(object[] arrMethodArgs, MethodInfo methodInfo)
		{
            Stream sInStream = null, sOutStream = null;
            try
            {
                int totalBytesRead;
                DateTime start = DateTime.Now;

                byte[] request = GetRequestBytes(arrMethodArgs, methodInfo);

                object result;
                try
                {
                    WebRequest webRequest = SendRequest(request, out sOutStream);
                    result = ReadReply(webRequest, methodInfo, out sInStream, out totalBytesRead);
                }
                catch (Exception e)
                {
                    /*
                    SocketException se = e.InnerException as SocketException;
                    WebException we = e as WebException;
                    if ((se != null && se.SocketErrorCode == SocketError.ConnectionAborted)
                        || (we != null && we.Status == WebExceptionStatus.KeepAliveFailure))
                     */
                    
                    if (!(e is CHessianException))
                    {
                        try
                        {
                            // retry once (Keep-Alive connection closed?)
                            WebRequest webRequest = SendRequest(request, out sOutStream);
                            result = ReadReply(webRequest, methodInfo, out sInStream, out totalBytesRead);
                        }
                        catch (Exception)
                        {
                            // retry again (last time)
                            WebRequest webRequest = SendRequest(request, out sOutStream);
                            result = ReadReply(webRequest, methodInfo, out sInStream, out totalBytesRead);
                        }
                    }
                    else
                        throw e; // rethrow
                }

                CHessianLog.AddLogEntry(methodInfo.Name, start, DateTime.Now, totalBytesRead, request.Length);
                return result;
            }
            catch (CHessianException he)
            {
                if (he.FaultWrapper)
                    // wrapper for a received exception
                    throw he.InnerException;
                else
                    throw he; // rethrow
            }
            finally
            {
                if (sInStream != null)
                    sInStream.Close();
                if (sOutStream != null)
                    sOutStream.Close();
            }
		}

        /// <summary>
        /// Reads and decodes the reply.
        /// </summary>
        /// <param name="webRequest"></param>
        /// <returns></returns>
        protected object ReadReply(WebRequest webRequest, MethodInfo methodInfo, out Stream sInStream, out int totalBytesRead)
        {
            HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse();
            if (webResponse.StatusCode != HttpStatusCode.OK)
                ReadAndThrowHttpFault(webResponse);
            sInStream = webResponse.GetResponseStream();

            if (webResponse.ContentEncoding.ToLower().Contains("gzip"))
                sInStream = new GZipStream(sInStream, CompressionMode.Decompress);
            else if (webResponse.ContentEncoding.ToLower().Contains("deflate"))
                sInStream = new DeflateStream(sInStream, CompressionMode.Decompress);

            BufferedStream bStream = new BufferedStream(sInStream, 4096);
            AbstractHessianInput hessianInput = this.GetHessianInput(bStream);

            object result = hessianInput.ReadReply(methodInfo.ReturnType);
            totalBytesRead = ((CHessianInput)hessianInput).GetTotalBytesRead();
            return result;
        }

        /// <summary>
        /// Sends the web request.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        protected WebRequest SendRequest(byte[] request, out Stream sOutStream)
        {
            WebRequest webRequest = PrepareWebRequest(request.Length);
            sOutStream = webRequest.GetRequestStream();
            sOutStream.Write(request, 0, request.Length);
            sOutStream.Flush();
            return webRequest;
        }

        /// <summary>
        /// Prepares a WebRequest object for communication
        /// with the Hessian server.
        /// </summary>
        /// <returns></returns>
        override protected WebRequest PrepareWebRequest(long contentLength)
        {
            WebRequest webRequest = base.PrepareWebRequest(contentLength);

            //webRequest.Headers
            HttpWebRequest req = webRequest as HttpWebRequest;

            // mw: gzip compression
            if (USE_GZIP_COMPRESSION)
                req.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip, deflate");

            //Preserve cookies to allow for session affinity between remote server and client
            if (HttpContext.Current != null && HttpContext.Current.Session != null)
            {
                if (HttpContext.Current.Session["SessionCookie"] == null)
                    HttpContext.Current.Session.Add("SessionCookie", new CookieContainer());
                req.CookieContainer = (CookieContainer)HttpContext.Current.Session["SessionCookie"];
                AddCustomHeadersToRequest(webRequest, HttpContext.Current.Session);
            }

            webRequest.Proxy = m_proxy;
            if (contentLength > -1)
                webRequest.ContentLength = contentLength;

            return webRequest;
        }

		/// <summary>
		/// Creates the URI connection.
		/// </summary>
		/// <param name="uri">Uri for connection</param>
		/// <returns>Request instance</returns>
		override protected WebRequest OpenConnection(Uri uri) 
		{
            WebRequest request = base.OpenConnection(uri);
            if (this.m_credentials != null)
                request.PreAuthenticate = true;
            return request;
		}

        protected void AddCustomHeadersToRequest(WebRequest request, HttpSessionState session)
        {
            if (session[CUSTOM_HEADER_KEY] != null)
            {
                IDictionary headers = session[CUSTOM_HEADER_KEY] as IDictionary;
                foreach (DictionaryEntry entry in headers)
                {
                    request.Headers.Add("X-" + entry.Key, entry.Value.ToString());
                }
            }
        }
    }
}
