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
using System.Collections; using System.Collections.Generic;
using System.Reflection;
#endregion

namespace hessiancsharp.io
{
	/// <summary>
	/// Serializing an object for known object types.
	/// Analog to the JavaSerializer - Class from 
	/// the Hessian implementation
	/// </summary>
	public class CObjectSerializer : AbstractSerializer
	{
		#region CLASS_FIELDS
		/// <summary>
		/// Fields of the objectType
		/// </summary>
		private List<Object> m_alFields = new List<Object>();
		#endregion
		#region CONSTRUCTORS
		/// <summary>
		/// Construktor.
		/// </summary>
		/// <param name="type">Type of the objects, that have to be
		/// serialized</param>
		public CObjectSerializer(Type type)
		{
			for (; type!=null; type = type.BaseType) 
			{
				FieldInfo [] fields = type.GetFields(BindingFlags.Public|
					BindingFlags.Instance|
					BindingFlags.NonPublic|
					BindingFlags.GetField |
					BindingFlags.DeclaredOnly);
				if (fields!=null) 
				{
					for (int i = 0; i<fields.Length; i++)
					{
                        if ((fields[i].Attributes & FieldAttributes.NotSerialized) == 0)
						    if (!this.m_alFields.Contains(fields[i])) 
						    {
							    this.m_alFields.Add(fields[i]);
						    }
					}
				}
				
			}
		}
		#endregion
		#region PUBLIC_METHODS
		
        /// <summary>
        /// Serialiaztion of objects
        /// </summary>
        /// <param name="obj">Object to serialize</param>
        /// <param name="abstractHessianOutput">HessianOutput - Instance</param>
        public override void WriteObject(object obj, AbstractHessianOutput abstractHessianOutput)
        {
            if (abstractHessianOutput.AddRef(obj))
                return;
            Type type = obj.GetType();
            abstractHessianOutput.WriteMapBegin(type.FullName);
            List<Object> serFields = GetSerializableFieldList();
            for (int i = 0; i < serFields.Count; i++)
            {
                FieldInfo field = (FieldInfo)serFields[i];
                abstractHessianOutput.WriteString(field.Name);
                abstractHessianOutput.WriteObject(field.GetValue(obj));
            }
            abstractHessianOutput.WriteMapEnd();
        }

        public virtual List<Object> GetSerializableFieldList()
        {
            return m_alFields;
        }


		#endregion
	}
}
