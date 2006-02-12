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
using System;
using System.Reflection;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;

namespace hessiancsharp.client
{
	/// <summary>
	/// Proxy that works with .NET - Remote proxy framework
	/// </summary>
	public class CHessianProxyStandardImpl :  RealProxy, IRemotingTypeInfo, IHessianProxyStandard
	{
		#region CLASS_FIELDS
		/// <summary>
		/// Interface type, that has to be proxied 
		/// </summary>
		private Type m_proxyType = null;
		// <summary>
		/// Instance to communicate with the Hessian - server
		/// </summary>
		private CHessianMethodCaller m_methodCaller = null;
		#endregion

		#region CONSTRUCTORS
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="proxyType">Interface type that has to be proxied</param>
		/// <param name="hessianProxyFactory">HessianProxyFactory - Instance</param>
		/// <param name="uri">Server-Proxy uri</param>
		public CHessianProxyStandardImpl(Type proxyType,CHessianProxyFactory hessianProxyFactory, Uri uri) : base(typeof(IHessianProxyStandard))
		{
			this.m_proxyType = proxyType;	
			this.m_methodCaller = new CHessianMethodCaller(hessianProxyFactory,uri);
		}

        public CHessianProxyStandardImpl(Type proxyType, CHessianProxyFactory hessianProxyFactory, Uri uri, string username, string password)
            : base(typeof(IHessianProxyStandard))
        {
            this.m_proxyType = proxyType;
            this.m_methodCaller = new CHessianMethodCaller(hessianProxyFactory, uri, username, password);
        }

		#endregion

		/// <summary>
		/// This reflective method for invoking methods. Overriden from RealProxy.
		/// Handles the object invocation. This method wrapps an instance call to the hessian 
		/// requests, sends it to the hessian service and translates the reply of this call to the C# - data type
		/// <see cref="System.Runtime.Remoting.Proxies.RealProxy"/>
		/// </summary>
		/// <param name="msg"></param>
		/// <returns></returns>
		public override IMessage Invoke(IMessage msg)
		{
            // Convert to a MethodCallMessage
            IMethodCallMessage methodMessage = new MethodCallMessageWrapper((IMethodCallMessage)msg);
            MethodInfo methodInfo = this.GetMethodInfoForMethodBase(methodMessage);
            Type[] argumentTypes = CHessianMethodCaller.GetArgTypes(methodMessage.Args);
            object objReturnValue = null;
            if (methodInfo != null)
            {
                if (methodInfo.Name.Equals("Equals") && argumentTypes != null &&
                    argumentTypes.Length == 1 && argumentTypes[0].IsAssignableFrom((typeof(Object))))
                {
                    Object value = methodMessage.Args[0];
                    if (value == null)
                    {
                        objReturnValue = false;
                    }
                    else if (value.GetType().Equals(typeof(CHessianProxy))
                        || value.GetType().IsAssignableFrom(typeof(CHessianProxy)))
                    {
                        objReturnValue = this.m_methodCaller.URI.Equals(((CHessianProxy)value).URI);
                    }
                    else
                    {
                        objReturnValue = false;
                    }
                }
                else if (methodInfo.Name.Equals("GetHashCode") && argumentTypes.Length == 0)
                {
                    objReturnValue = this.m_methodCaller.URI.GetHashCode();
                }
                else if (methodInfo.Name.Equals("GetHessianURL"))
                {
                    objReturnValue = this.m_methodCaller.URI.ToString();
                }
                else if (methodInfo.Name.Equals("ToString") && argumentTypes.Length == 0)
                {
                    objReturnValue = "[HessianProxy " + this.m_methodCaller.URI + "]";
                }
                else if (methodInfo.Name.Equals("GetType") && argumentTypes.Length == 0)
                {
                    objReturnValue = this.m_proxyType;
                }
                else
                {
                    objReturnValue = this.m_methodCaller.DoHessianMethodCall(methodMessage.Args, methodInfo);
                }
            }
            else
            {
                if (methodMessage.MethodName.Equals("GetType") && (methodMessage.ArgCount == 0))
                {
                    objReturnValue = this.m_proxyType;
                }
            }
            // Create the return message (ReturnMessage)
            return new ReturnMessage(objReturnValue, methodMessage.Args, methodMessage.ArgCount, methodMessage.LogicalCallContext, methodMessage);


		}
		/// <summary>
		/// Checks whether the proxy representing the specified object 
		/// type can be cast to the type represented by the IRemotingTypeInfo interface
		/// </summary>
		/// <param name="fromType">Cast - Type</param>
		/// <param name="obj">Proxy object</param>
		/// <returns>True if the cast type equals or is assingable from the interface type,
		/// wich was used for proxy initialization
		/// </returns>
		public bool CanCastTo(Type fromType, object obj)
		{
			return fromType.Equals(this.m_proxyType) || fromType.IsAssignableFrom(this.m_proxyType);
		}

		/// <summary>
		/// Gets the name of the interface type, 
		/// that has to be proxied 
		/// </summary>
		public string TypeName
		{
			get { return m_proxyType.Name; }
			set {  }
		}
		/// <summary>
		/// Gets method info instance, according to the given method base
		/// </summary>
		/// <param name="methodMessage">Method message, that describes the method call</param>
		/// <returns>MethodInfo - Instance</returns>
		private MethodInfo GetMethodInfoForMethodBase(IMethodCallMessage methodMessage)
		{
			return this.m_proxyType.GetMethod(methodMessage.MethodName, CHessianMethodCaller.GetArgTypes(methodMessage.Args));
		}
	}
}
