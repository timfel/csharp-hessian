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
using System.IO;
using System.Net;
using System.Reflection;
#endregion

namespace burlapcsharp.client
{
    class CBurlapProxy
    {
        #region CLASS_FIELDS
		/// <summary>
		/// Instance to communicate with the Burlap - server
		/// </summary>
		private CBurlapMethodCaller m_methodCaller = null;
		#endregion

		#region PROPERTIES
		/// <summary> 
		/// Returns the connection uri to the Burlap service.
		/// </summary>
		public virtual Uri URI 
		{
			get { return m_methodCaller!=null?m_methodCaller.URI:null; }

		}
		#endregion

		#region CONSTRUCTORS
		/// <summary>
		/// Constructor
		/// </summary>
        /// <param name="burlapProxyFactory">CBurlapProxyFactory - Instance</param>
		/// <param name="uri">Server-Proxy uri</param>
		internal CBurlapProxy(CBurlapProxyFactory burlapProxyFactory, Uri uri) 
		{
			this.m_methodCaller = new CBurlapMethodCaller(burlapProxyFactory,uri);
		}
        #endregion

        

		/// <summary>
		/// Handles the object invocation. This method wrapps an instance call to the hessian 
		/// requests, sends it to the hessian service and translates the reply of this call to the C# - data type
		/// </summary>
		/// <param name="objProxy">The proxy object to invoke</param>
		/// <param name="methodInfo">The method to call</param>
		/// <param name="arrMethodArgs">The arguments to the method call</param>
		/// <returns>Invocation result</returns>
		public object Invoke(object objProxy, MethodInfo methodInfo, object[] arrMethodArgs) 
		{
			return this.m_methodCaller.DoBurlapMethodCall(arrMethodArgs, methodInfo );
		}
    }

}
