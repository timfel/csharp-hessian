using System;
using System.Collections; using System.Collections.Generic;
using System.Reflection;

namespace hessiancsharp.io
{
	public class CExceptionSerializer : CObjectSerializer
	{
		private List<Object> m_serFields = new List<Object>();
		public CExceptionSerializer():base(typeof(Exception))
		{
			m_serFields = GetSerializableFields();
		}

		public static List<Object> GetSerializableFields()
		{
			Type type = typeof(Exception);
			List<Object> serFields = new List<Object>();
			FieldInfo [] fields = type.GetFields(BindingFlags.Public|
												BindingFlags.Instance|
												BindingFlags.NonPublic|
												BindingFlags.GetField |
												BindingFlags.DeclaredOnly);
			if (fields!=null) 
			{
				for (int i = 0; i<fields.Length; i++)
				{
					if ((!serFields.Contains(fields[i]))   && (fields[i].FieldType != typeof(System.IntPtr)))
					{
						serFields.Add(fields[i]);
					}
				}
			}
			return serFields;
		}

		public override List<Object> GetSerializableFieldList()
		{
			return m_serFields;
		}

	}
}
