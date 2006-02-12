using System;
using System.Collections;
using System.Reflection;

namespace hessiancsharp.io
{
	/// <summary>
	/// Summary description for CExceptionDeserializer.
	/// </summary>
	public class CExceptionDeserializer : CObjectDeserializer
	{
		private IDictionary m_deserFields = new Hashtable();
		private Type m_type = null;
		public CExceptionDeserializer(Type type):base(type)
		{
			ArrayList fieldList = CExceptionSerializer.GetSerializableFields();
			foreach (FieldInfo fieldInfo in fieldList)
			{
				m_deserFields[fieldInfo.Name] = fieldInfo;
			}
			m_type = type;
		}

		public override IDictionary GetDeserializableFields()
		{
			return m_deserFields;
		}

		public override object ReadMap(AbstractHessianInput abstractHessianInput)
		{
			Hashtable fieldValueMap = new Hashtable();
			string _message = null;
			Exception _innerException = null;
			while (! abstractHessianInput.IsEnd()) 
			{
				object objKey = abstractHessianInput.ReadObject();
				if(objKey != null)
				{
					IDictionary deserFields = GetDeserializableFields();
					FieldInfo field = (FieldInfo) deserFields[objKey];
					if(objKey.ToString() == "_message")
					{
						_message = abstractHessianInput.ReadObject(field.FieldType) as string;
					}
					else if(objKey.ToString() == "_innerException")
					{
						_innerException = abstractHessianInput.ReadObject(field.FieldType) as Exception;
					}
					else
					{	
						object objFieldValue = abstractHessianInput.ReadObject(field.FieldType );
						fieldValueMap.Add(field, objFieldValue);
						//field.SetValue(result, objFieldValue);
					}	
				}
				
			}
			abstractHessianInput.ReadEnd();

			object result =  null;
			try
			{
#if COMPACT_FRAMEWORK
            	//CF TODO: tbd
#else
				try
				{
					result = Activator.CreateInstance(this.m_type, new object[2]{_message, _innerException});	
				}
				catch(Exception)
				{
					try
					{
						result = Activator.CreateInstance(this.m_type, new object[1]{_innerException});		
					}
					catch(Exception)
					{
						try
						{
							result = Activator.CreateInstance(this.m_type, new object[1]{_message});			
						}
						catch(Exception)
						{
							result = Activator.CreateInstance(this.m_type);			
						}
					}
                }
#endif

            }
			catch(Exception)
			{
				result = new Exception(_message, _innerException);
			}
			foreach (DictionaryEntry entry in fieldValueMap)
			{
				FieldInfo fieldInfo = (FieldInfo) entry.Key;
				object value = entry.Value;
				try {fieldInfo.SetValue(result, value);} catch(Exception){}
			}
			return result;
		}
	}
}
