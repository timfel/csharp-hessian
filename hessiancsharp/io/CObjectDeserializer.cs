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
* Last change: 2005-12-16
* By Dimitri Minich
* 2005-12-16: GetDeserializableFields added
* 2006-01-03: BUGFIX Non-existing fields by mw
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
	/// Deserializing an object for known object types.
	/// Analog to the JavaDeserializer - Class from 
	/// the Hessian implementation
	/// </summary>
	public class CObjectDeserializer : AbstractDeserializer
	{
		#region CLASS_FIELDS
		/// <summary>
		/// Object type
		/// </summary>
		private Type m_type;
		/// <summary>
		/// Hashmap with class fields (&lt;field name&gt;&lt;field info instance&gt;)
		/// </summary>
		private Dictionary<Object, Object> m_htFields = new Dictionary<Object, Object>();
		#endregion
		#region CONSTRUCTORS
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="type">Type of the objects, that have to be
		/// deserialized</param>
		public CObjectDeserializer(Type type)
		{
			this.m_type = type;
			for (; type!=null; type = type.BaseType) 
			{
				FieldInfo [] fields = type.GetFields(
					BindingFlags.Public|
					BindingFlags.Instance|
					BindingFlags.NonPublic|
					BindingFlags.GetField |
					BindingFlags.DeclaredOnly);
				if (fields!=null) 
				{
					for (int i = 0; i<fields.Length; i++)
					{
						this.m_htFields.Add(fields[i].Name, fields[i]);
					}
				}
			}
		}
		#endregion
		#region PUBLIC_METHODS

		public override Type GetOwnType() {
			return m_type;
		}
		/// <summary>
		/// Reads object as map
		/// </summary>
		/// <param name="abstractHessianInput">HessianInput to read from</param>
		/// <returns>Read object or null</returns>
		public override object ReadObject(AbstractHessianInput abstractHessianInput)
		{
			return this.ReadMap( abstractHessianInput );
		}

        /// <summary>
        /// Reads map
        /// </summary>
        /// <param name="abstractHessianInput">HessianInput to read from</param>
        /// <returns>Read object or null</returns>
        public override object ReadMap(AbstractHessianInput abstractHessianInput)
        {
            #if COMPACT_FRAMEWORK
            object result = Activator.CreateInstance(this.m_type);				
            #else
            object result = Activator.CreateInstance(this.m_type.Assembly.FullName, this.m_type.FullName).Unwrap();
            //			object result = Activator.CreateInstance(this.m_type);
            //			object result = null;
            #endif


            return ReadMap(abstractHessianInput, result);
   
        }


		/// <summary>
		/// Reads map
		/// </summary>
		/// <param name="abstractHessianInput">HessianInput to read from</param>
		/// <returns>Read object or null</returns>
        public object ReadMap(AbstractHessianInput abstractHessianInput, Object result)
        {

            
            int refer = abstractHessianInput.AddRef(result);

			while (! abstractHessianInput.IsEnd()) 
			{
				object objKey = abstractHessianInput.ReadObject();
                IDictionary deserFields = GetDeserializableFields();
				FieldInfo field = null;
                field = (FieldInfo)deserFields[objKey];
                

                if (field != null)
                {
                    object objFieldValue = abstractHessianInput.ReadObject(field.FieldType);
                    field.SetValue(result, objFieldValue);
                }
                else
                {
                    // mw BUGFIX!!!
                    object ignoreme = abstractHessianInput.ReadObject();
                }
                
            }
			abstractHessianInput.ReadEnd();
			return result;
		}

        public virtual IDictionary GetDeserializableFields()
        {
            return m_htFields;
        }

		#endregion
	}
}
