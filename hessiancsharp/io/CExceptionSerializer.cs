using System;
using System.Collections;
using System.Reflection;

namespace hessiancsharp.io
{
	public class CExceptionSerializer : CObjectSerializer
	{
		private ArrayList m_serFields = new ArrayList();
		public CExceptionSerializer():base(typeof(Exception))
		{
			m_serFields = GetSerializableFields();
		}

		public static ArrayList GetSerializableFields()
		{
			Type type = typeof(Exception);
			ArrayList serFields = new ArrayList();
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

		public override ArrayList GetSerializableFieldList()
		{
			return m_serFields;
		}

	}
}
