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
* Last change: 2006-05-02
* By Dimitri Minich	
* Initial creation.
******************************************************************************************************
*/
#region NAMESPACES
using System;
using System.Text;
using System.Net;
using System.IO;
using hessiancsharp.server;
using hessiancsharp.io;
#endregion

namespace hessiancsharp.webserver 
{
    public delegate void delReceiveWebRequest(HttpListenerContext Context);



    /// <summary>
    /// Wrapper class for the HTTPListener to allow easier access to the
    /// server, for start and stop management and event routing of the actual
    /// inbound requests.
    /// </summary>

    public class CHessianWebServer : CWebServer
    {


        #region CLASS_FIELDS

        /// <summary>
        /// Proxy object
        /// </summary>
        protected CHessianSkeleton m_objectSkeleton = null;
        private HttpListenerContext context;


        #endregion

        protected HttpListenerContext Context { get { return context; } }
        public bool IsReusable { get { return true; } }

        protected override void ProcessRequest(System.Net.HttpListenerContext ctx)
        {
            try
            {
                context = ctx;
                Stream inStream = ctx.Request.InputStream;
                MemoryStream outStream = new MemoryStream();

                //ctx.Response.BufferOutput = true;
                ctx.Response.ContentType = "text/xml";

                AbstractHessianInput inHessian = new CHessianInput(inStream);
                AbstractHessianOutput outHessian = new CHessianOutput(outStream);

                if (m_objectSkeleton == null)
                {
                    //Vieleicht das Interface als API übergeben???
                    m_objectSkeleton = new CHessianSkeleton(this.GetType(), this);
                }

                m_objectSkeleton.invoke(inHessian, outHessian);
                byte[] arrData = outStream.ToArray();
                int intLength = arrData.Length;
                //Set length
                ctx.Response.ContentLength64 = intLength;

                //Write stream
                ctx.Response.OutputStream.Write(arrData, 0, intLength);
                return;
            }
            catch (Exception ex)
            {
                ctx.Response.StatusCode = 500;  // "Internal server error"
                ctx.Response.StatusDescription = ex.Message;
            }
        }

      

    }
}