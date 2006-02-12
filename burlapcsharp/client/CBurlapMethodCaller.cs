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
******************************************************************************************************
*/

#region NAMESPACES
using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Reflection;
using System.Web;
using System.Web.SessionState;
using burlapcsharp.io;
#endregion

namespace burlapcsharp.client
{
    class CBurlapMethodCaller
    {

        #region CLASS_FIELDS
        /// <summary>
        /// Instance of the proxy factory
        /// </summary>
        private CBurlapProxyFactory m_CBurlapProxyFactory;
        /// <summary>
        /// Uri for connection to the burlap service
        /// </summary>
        private Uri m_uriBurlapServiceUri;

        private NetworkCredential m_credentials = null;

        #endregion
        #region PROPERTIES
        /// <summary> 
        /// Returns the connection uri to the burlap service.
        /// </summary>
        public virtual Uri URI
        {
            get { return m_uriBurlapServiceUri; }

        }
        #endregion

        #region CONSTRUCTORS
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="hessianProxyFactory">BurlapProxyFactory - Instance</param>
		/// <param name="uri">Server-Proxy uri</param>
		public CBurlapMethodCaller(CBurlapProxyFactory burlapProxyFactory, Uri uri)
		{
            this.m_CBurlapProxyFactory = burlapProxyFactory;
            this.m_uriBurlapServiceUri = uri;
		}

        
		#endregion

        #region PUBLIC_METHODS
        /// <summary>
        /// This method wrapps an instance call to the burlap 
        /// requests, sends it to the burlap service and translates the reply of this call to the C# - data type
        /// </summary>
        /// <param name="methodInfo">The method to call</param>
        /// <param name="arrMethodArgs">The arguments to the method call</param>
        /// <returns>Invocation result</returns>

        public object DoBurlapMethodCall(object[] arrMethodArgs, MethodInfo methodInfo)
        {
            Type[] argumentTypes = GetArgTypes(arrMethodArgs);
            Stream sInStream = null;
            Stream sOutStream = null;

            try
            {
                WebRequest webRequest = this.OpenConnection(m_uriBurlapServiceUri);
               

                webRequest.ContentType = "text/xml";
                webRequest.Method = "POST";
             

                MemoryStream memoryStream = new MemoryStream(2048);

                CBurlapOutput cBurlapOutput = this.GetBurlapOutput(memoryStream);
                string strMethodName = methodInfo.Name;
                if (m_CBurlapProxyFactory.IsOverloadEnabled)
                {
                    if (arrMethodArgs != null)
                    {
                        strMethodName = strMethodName + "__" + arrMethodArgs.Length;
                    }
                    else
                    {
                        strMethodName = strMethodName + "__0";
                    }
                }

                cBurlapOutput.Call(strMethodName, arrMethodArgs);

                try
                {
                    webRequest.ContentLength = memoryStream.ToArray().Length;
                    sOutStream = webRequest.GetRequestStream();
                    memoryStream.WriteTo(sOutStream);

                }
                catch (Exception e)
                {
                    throw new CBurlapException("Exception by sending request to the service with URI:\n" +
                        this.URI.ToString() + "\n" + e.Message);
                }


                sOutStream.Flush();
                sOutStream.Close();
                HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse();
                if (webResponse.StatusCode != HttpStatusCode.OK)
                {
                    System.Text.StringBuilder sb = new System.Text.StringBuilder();
                    int chTemp;
                    sInStream = webResponse.GetResponseStream();

                    if (sInStream != null)
                    {
                        while ((chTemp = sInStream.ReadByte()) >= 0)
                            sb.Append((char)chTemp);

                        sInStream.Close();
                    }
                    throw new CBurlapException(sb.ToString());
                }
                sInStream = webResponse.GetResponseStream();

                System.IO.BufferedStream bStream = new BufferedStream(sInStream, 2048);
                AbstractBurlapInput burlapInput = this.GetBurlapInput(bStream);

                return burlapInput.ReadReply(methodInfo.ReturnType);
            }
            catch (Exception e)
            {
                if (e.GetType().Equals(typeof(CBurlapException)))
                {
                    throw e;
                }
                else
                {
                    throw new CBurlapException("Exception by proxy call\n" + e.ToString() + e.Message);
                }

            }
            finally
            {
                if (sInStream != null)
                {
                    sInStream.Close();
                }
                if (sOutStream != null)
                {
                    sOutStream.Close();
                }
            }
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
        #endregion

        #region PRIVATE_METHODS
        /// <summary>
        /// Creates the URI connection.
        /// </summary>
        /// <param name="uri">Uri for connection</param>
        /// <returns>Request instance</returns>
        private WebRequest OpenConnection(Uri uri)
        {
            WebRequest request = WebRequest.Create(uri);
            if (this.m_credentials != null)
            {
                request.Credentials = this.m_credentials;
            }
            return request;
        }

        /// <summary>
        /// Instantiation of the burlap input (not cached) 
        /// </summary>
        /// <param name="stream">Stream for BurlapInput-Instantiation</param>
        /// <returns>New BurlapInput - Instance</returns>
        private AbstractBurlapInput GetBurlapInput(Stream stream)
        {
            return new CBurlapInput(stream);
        }


        /// <summary>
        /// Instantiation of the hessian output (not cached)
        /// </summary>
        /// <param name="stream">Strean for HessianOutput - Instantiation</param>
        /// <returns>New HessianOutput - Instance</returns>
        private CBurlapOutput GetBurlapOutput(Stream stream)
        {
            CBurlapOutput cBurlapOut = new CBurlapOutput(stream);
            return cBurlapOut;
        }
        #endregion
    }
}
