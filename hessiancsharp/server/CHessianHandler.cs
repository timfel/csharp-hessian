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
using System.IO;
using System.Web;
using hessiancsharp.io;
using System.Web.SessionState;
using System;

#endregion

namespace hessiancsharp.server
{
	/// <summary>
	/// HessianHandler for request and response
	/// </summary>
	public class CHessianHandler: IHttpHandler, IRequiresSessionState
	{
		#region CLASS_FIELDS

		/// <summary>
		/// Proxy object
		/// </summary>
		protected CHessianSkeleton m_objectSkeleton = null;
        private HttpContext context;
        

		#endregion

        protected HttpContext Context { get { return context; } }         
		public bool IsReusable {get {return true; } }
		
		/// <summary>
		/// Execute a request.
		/// </summary>				
		public void ProcessRequest(HttpContext ctx)
		{
            try
            {
                context = ctx;
                Stream inStream = ctx.Request.InputStream;
                Stream outStream = ctx.Response.OutputStream;

                ctx.Response.BufferOutput = true;
                ctx.Response.ContentType = "text/xml";

                AbstractHessianInput inHessian = new CHessianInput(inStream);
                AbstractHessianOutput outHessian = new CHessianOutput(outStream);

                if (m_objectSkeleton == null)
                {
                    //Vieleicht das Interface als API übergeben???
                    m_objectSkeleton = new CHessianSkeleton(this.GetType(), this);
                }

                m_objectSkeleton.invoke(inHessian, outHessian);

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
