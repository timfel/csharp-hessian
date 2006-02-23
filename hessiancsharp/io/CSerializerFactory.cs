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
* Last change: 2005-08-14
* 2005-08-14 Licence added (By Andre Voltmann)
* 2005-08-04: SBYTE added (Dimitri Minich)
* 2005-12-16: CExceptionDeserializer and  CExceptionSerializera dded (Dimitri Minich)
* 2006-01-03: Support for nullable types (Matthias Wuttke)
* 2006-02-23: Support for Generic lists (Matthias Wuttke)
******************************************************************************************************
*/

#region NAMESPACES
using System;
using System.Collections;
using System.IO;
using System.Reflection;

#endregion
namespace hessiancsharp.io
{
	/// <summary>
	/// Factory for helper-classes for Serialiazation and Deserialization
	/// throw HessianOutput and HessianInput  
	/// </summary>
	public class CSerializerFactory
	{
		#region CLASS_FIELDS
		/// <summary>
		/// Map with deserializers
		/// </summary>
		private static Hashtable m_htDeserializerMap = null;

		/// <summary>
		/// Map with serializers
		/// </summary>
		private static Hashtable m_htSerializerMap = null;

		/// <summary>
		/// Map with type names
		/// </summary>
		private static Hashtable m_htTypeMap = null;

		/// <summary>
		/// Cache for serializer
		/// </summary>
		private static Hashtable m_htCachedSerializerMap = null;

		/// <summary>
		/// Cache for deserializer
		/// </summary>
		private static Hashtable m_htCachedDeserializerMap = null;		

		#endregion
		
		#if COMPACT_FRAMEWORK
		/// <summary>
		/// List of all files in current directory with extension ".exe" and ".dll"
		/// </summary>
		private IList m_assamblyFiles = null; 		
		#endif
		


		#region STATIC_CONSTRUCTORS
		/// <summary>
		/// Static initalization
		/// </summary>
		static CSerializerFactory()
		{
			m_htDeserializerMap = new Hashtable();
			m_htSerializerMap = new Hashtable();
			m_htTypeMap = new Hashtable();
			addBasic(typeof (char), "char", CSerializationConstants.CHARACTER);
			addBasic(typeof (byte), "byte", CSerializationConstants.BYTE);
			addBasic(typeof (sbyte), "sbyte", CSerializationConstants.SBYTE);
			addBasic(typeof (short), "short", CSerializationConstants.SHORT);
			addBasic(typeof (int), "int", CSerializationConstants.INTEGER);
			addBasic(typeof (double), "double", CSerializationConstants.DOUBLE);
			addBasic(typeof (string), "string", CSerializationConstants.STRING);
			addBasic(typeof (long), "long", CSerializationConstants.LONG);
			addBasic(typeof (float), "float", CSerializationConstants.FLOAT);
			addBasic(typeof (bool), "bool", CSerializationConstants.BOOLEAN);

			addBasic(typeof (bool[]), "[bool", CSerializationConstants.BOOLEAN_ARRAY);
			addBasic(typeof (byte[]), "[byte", CSerializationConstants.BYTE_ARRAY);
			addBasic(typeof (short[]), "[short", CSerializationConstants.SHORT_ARRAY);
			addBasic(typeof (int[]), "[int", CSerializationConstants.INTEGER_ARRAY);
			addBasic(typeof (long[]), "[long", CSerializationConstants.LONG_ARRAY);
			addBasic(typeof (float[]), "[float", CSerializationConstants.FLOAT_ARRAY);
			addBasic(typeof (double[]), "[double", CSerializationConstants.DOUBLE_ARRAY);
			addBasic(typeof (char[]), "[char", CSerializationConstants.CHARACTER_ARRAY);
			addBasic(typeof (string[]), "[string", CSerializationConstants.STRING_ARRAY);
            addBasic(typeof(sbyte[]), "[sbyte", CSerializationConstants.SBYTE_ARRAY);
			addBasic(typeof(DateTime),"date",CSerializationConstants.DATE);
			//addBasic(typeof(Object[]), "[object", BasicSerializer.OBJECT_ARRAY);
			m_htCachedDeserializerMap = new Hashtable();
			m_htCachedSerializerMap = new Hashtable();
            m_htSerializerMap.Add(typeof(System.Decimal), new CStringValueSerializer());

            m_htDeserializerMap.Add(typeof(System.Decimal), new CDecimalDeserializer());


			
			m_htSerializerMap.Add(typeof (System.IO.FileInfo), new CStringValueSerializer());
			m_htDeserializerMap.Add(typeof (System.IO.FileInfo),
				new CStringValueDeserializer(typeof (System.IO.FileInfo)));
			//m_htSerializerMap.Add(typeof (System.DateTime), new CDateSerializer());
			//m_htDeserializerMap.Add(typeof (System.DateTime), new CDateDeserializer());
		}

		#endregion
		
		#region PRIVATE_METHODS
		/// <summary>
		/// Adds basic serializers to the Hashtables
		/// </summary>
		/// <param name="type">Type of the instances for de/serialization</param>
		/// <param name="strTypeName">Type name of the instances for de/serialization</param>
		/// <param name="intTypeCode">Type code <see cref="CSerializationConstants"/></param>
		private static void addBasic(Type type, string strTypeName, int intTypeCode)
		{
			m_htSerializerMap.Add(type, new CBasicSerializer(intTypeCode));
			AbstractDeserializer abstractDeserializer = new CBasicDeserializer(intTypeCode);
			m_htDeserializerMap.Add(type, abstractDeserializer);
			m_htTypeMap.Add(strTypeName, abstractDeserializer);
		}

		#endregion

		#region PUBLIC_METHODS
		/// <summary>
		/// Gets the serializer-Instance according to given type
		/// </summary>
		/// <param name="type">Type of the objects, that have to be serialized</param>
		/// <returns>Serializer - Instance</returns>
		public AbstractSerializer GetSerializer(Type type)
		{
			AbstractSerializer abstractSerializer = (AbstractSerializer) m_htSerializerMap[type];
			if (abstractSerializer == null)
			{
                // TODO: Serialisieren von Nullbaren Typen und generischen
                // Listen
				if (typeof (IDictionary).IsAssignableFrom(type))
				{
					abstractSerializer = new CMapSerializer();
				}
				else if (typeof (IList).IsAssignableFrom(type))
				{
					abstractSerializer = new CCollectionSerializer();
				}else if (typeof (Stream).IsAssignableFrom(type))
				{
					abstractSerializer = new CInputStreamSerializer();
				}
                else if (typeof(Exception).IsAssignableFrom(type))
                {
                    abstractSerializer = new CExceptionSerializer();
                }	
				else if (type.IsArray)
				{
					abstractSerializer = new CArraySerializer();
				}
				else
				{
					if (m_htCachedSerializerMap[type.FullName] != null)
					{
						abstractSerializer = (AbstractSerializer) m_htCachedSerializerMap[type.FullName];
					}
					else
					{
						abstractSerializer = new CObjectSerializer(type);
						m_htCachedSerializerMap.Add(type.FullName, abstractSerializer);

					}

				}
			}
			return abstractSerializer;
		}

		/// <summary>
		/// Returns according deserializer to the given type
		/// </summary>
		/// <param name="type">Type of the deserializer</param>
		/// <returns>Deserializer instance</returns>
		public AbstractDeserializer GetDeserializer(Type type)
		{
			AbstractDeserializer abstractDeserializer = (AbstractDeserializer) m_htDeserializerMap[type];
			if (abstractDeserializer == null)
			{
				if (typeof (IDictionary).IsAssignableFrom(type))
				{
					abstractDeserializer = new CMapDeserializer(type);
				}
                else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(System.Nullable))
                {
                    // nullbarer Typ
                    Type[] args = type.GetGenericArguments();
                    return GetDeserializer(args[0]);
                }
                else if (type.IsArray)
                {
                    abstractDeserializer = new CArrayDeserializer(GetDeserializer(type.GetElementType()));
                }
                else if (typeof(IList).IsAssignableFrom(type) ||
                    (type.IsGenericType && 
                    typeof(System.Collections.Generic.List<>).IsAssignableFrom(type.GetGenericTypeDefinition())))
                {
                    abstractDeserializer = new CCollectionDeserializer(type);
                }
                else if (typeof(Exception).IsAssignableFrom(type))
                {
                    abstractDeserializer = new CExceptionDeserializer(type);
                }
                else
                {
                    if (m_htCachedDeserializerMap[type.FullName] != null)
                    {
                        abstractDeserializer = (AbstractDeserializer)m_htCachedDeserializerMap[type.FullName];
                    }
                    else
                    {
                        abstractDeserializer = new CObjectDeserializer(type);
                        m_htCachedDeserializerMap.Add(type.FullName, abstractDeserializer);

                    }
                }
			}
			return abstractDeserializer;

		}

		/// <summary>
		/// Returns a deserializer based on a string type.
		/// </summary>
		/// <param name="strType">Type of the object for deserialization</param>
		/// <returns>deserializer based on a string type</returns>
		public AbstractDeserializer GetDeserializer(string strType)
		{
			if (strType == null || strType.Equals(""))
				return null;

			AbstractDeserializer abstractDeserializer = null;

			abstractDeserializer = (AbstractDeserializer) m_htTypeMap[strType];
			if (abstractDeserializer != null)
			{
				return abstractDeserializer;
			}			

			if (strType.StartsWith("["))
			{
				AbstractDeserializer subABSTRACTDeserializer = GetDeserializer(strType.Substring(1));
				abstractDeserializer = new CArrayDeserializer(subABSTRACTDeserializer);
				return abstractDeserializer;
			}
			else 
			{
				#if COMPACT_FRAMEWORK
				// do CF stuff
				if (m_assamblyFiles == null) 
				{
					m_assamblyFiles = AllAssamblyNamesInCurrentDirectory();
				}
				foreach(string ass in m_assamblyFiles) 
				{
					string typeString = strType + ","+ass;
					Type searchType = Type.GetType(typeString);
					if(searchType != null) 
					{
						abstractDeserializer = GetDeserializer(searchType);
						return abstractDeserializer;
					}
				}
				#else
				
				// do other stuff
				try 
				{
					//Diese Typsuche funzt bei Mobileloesung nicht:
					//Es wurde ein andere Suche implementiert
					Assembly[] ass = AppDomain.CurrentDomain.GetAssemblies();
					Type t = null;
					foreach(Assembly a in ass) 
					{
						t = a.GetType(strType);
						if(t != null) 
						{							
							break;	
						}
					}
					if(t != null)
						abstractDeserializer = GetDeserializer(t);
															
				} 
				catch (Exception) 
				{
				}
			#endif
				
			}
			
			/* TODO: Implementieren Type.GetType(type) geht nicht, man muss die Assembly eingeben.
			*/
			//deserializer = getDeserializer(Type.GetType(type));
			return abstractDeserializer;
		}
		
		#if COMPACT_FRAMEWORK
				// do CF stuff
		/// <summary>
		/// Returns a List of files in current directory with extension ".exe" and ".dll".
		/// </summary>		
		/// <returns>List of all files as string </returns>
		private IList AllAssamblyNamesInCurrentDirectory() 
		{
			ArrayList result = new ArrayList();			

			string currentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetModules()[0].FullyQualifiedName);

			try
			{        
				//Create A new Instance from DirectoryInfo, to Get information
				//About The current directory strDirectory
				DirectoryInfo CurDir = new  DirectoryInfo(currentDirectory);
            

				//Array Holds Files and thier information
				FileInfo[] FilesArray;            
				/*
				  * Call GetFiles() returns and array of Files inside
				  * the Current Directory strDirectory
				*/

				FilesArray = CurDir.GetFiles();

				//Get every FileInfo inside the current Directory
				foreach (FileInfo curfileInfo in FilesArray)
				{					
					/*
					  *Check for File Extension   
					*/

					if(curfileInfo.Extension == ".exe")
					{
						string name = curfileInfo.Name.Substring(0,curfileInfo.Name.IndexOf(".exe"));
						
						result.Add(name);
						//
					}
					else if(curfileInfo.Extension == ".dll")
					{
						string name = curfileInfo.Name.Substring(0,curfileInfo.Name.IndexOf(".dll"));
						
						result.Add(name);
					}					
				}				
			}
			catch(UnauthorizedAccessException)
			{
				//TODO:"Access Denied!"
				
			}
			return result;
		}

		#endif


		/// <summary>
		/// Reads the object as a map. (Hashtable)
		/// </summary>
		/// <param name="abstractHessianInput">HessianInput instance to read from</param>
		/// <param name="strType">Type of the map (can be null)</param>
		/// <returns>Object read from stream</returns>
		public Object ReadMap(AbstractHessianInput abstractHessianInput, string strType)
		{
			AbstractDeserializer abstractDeserializer = GetDeserializer(strType);

			if (abstractDeserializer == null)
			{
				abstractDeserializer = new CMapDeserializer(typeof (Hashtable));
			}
			return abstractDeserializer.ReadMap(abstractHessianInput);
		}

		/// <summary>
		/// Returns the Deserializer - instance that reads object as a map
		/// </summary>
		/// <param name="strType">Object - Type</param>
		/// <returns>Deserializer object</returns>
		public AbstractDeserializer GetObjectDeserializer(string strType)

		{
			AbstractDeserializer abstractDeserializer = GetDeserializer(strType);
			if (abstractDeserializer != null)
				return abstractDeserializer;
			else
			{
				return new CMapDeserializer(typeof (Hashtable));
			}
		}

		/// <summary>
		/// Reads the array.
		/// </summary>
		/// <param name="abstractHessianInput">HessianInput</param>
		/// <param name="intLength">Length of data</param>
		/// <param name="strType">Type of the array objects</param>
		/// <returns>Array data</returns>
		public Object ReadList(AbstractHessianInput abstractHessianInput, int intLength, string strType)
		{
			AbstractDeserializer abstractDeserializer = GetDeserializer(strType);

			if (abstractDeserializer != null)
				return abstractDeserializer.ReadList(abstractHessianInput, intLength);
			else
				return new CCollectionDeserializer(typeof (ArrayList)).ReadList(
					abstractHessianInput,
					intLength);
		}
		#endregion
	}
}