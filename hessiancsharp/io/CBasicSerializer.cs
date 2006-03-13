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
* Last change: 2006-01-03
* By Dimitri Minich
* 2005-12-16: SBYTE_ARRAY added. (Dimitri Minich)
* 2006-01-03: BUGFIX Date by mw
******************************************************************************************************
*/

#region NAMESPACES
using System;
#endregion

namespace hessiancsharp.io
{
	/// <summary>
	/// Serializing an object for known object types
	/// </summary>
	public class CBasicSerializer : AbstractSerializer
	{
		#region CLASS_FIELDS

		/// <summary>
		/// Type code of this Serializer Instance
		/// </summary>
		private int m_intCode;

		#endregion
		#region CONSTRUCTORS
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="intCode">Code that identifies this
		/// instance as Serializer</param>
		public CBasicSerializer(int intCode)
		{
			this.m_intCode = intCode;
		}
		#endregion
		#region PUBLIC_METHODS
		/// <summary>
		/// Writes primitive objects and arrayy of primitive objects
		/// </summary>
		/// <param name="obj">Object to write</param>
		/// <param name="abstractHessianOutput">HessianOutput - Instance</param>
		/// <exception cref="CHessianException"/>
		public override void WriteObject(object obj, AbstractHessianOutput abstractHessianOutput)
		{
			switch (m_intCode)
			{
				case BOOLEAN:
					abstractHessianOutput.WriteBoolean(((bool) obj));
					break;
				case BYTE:
				case SBYTE:
				case SHORT:
				case INTEGER:
					abstractHessianOutput.WriteInt(Convert.ToInt32(obj));
					break;
				case LONG:
					abstractHessianOutput.WriteLong((long) obj);
					break;
				case FLOAT:
					abstractHessianOutput.WriteDouble(Convert.ToDouble(obj));
					break;
				case DOUBLE:
					abstractHessianOutput.WriteDouble((double) obj);
					break;
				case CHARACTER:
					abstractHessianOutput.WriteInt((char) obj);
					break;
				case STRING:
					abstractHessianOutput.WriteString((string) obj);
					break;
				case DATE:					
                    DateTime dt = (DateTime)obj;
                    const long timeShift = 62135596800000;
                    long javaTime = dt.Ticks / 10000 - timeShift;
                    abstractHessianOutput.WriteUTCDate(javaTime);
                    break; 

				case INTEGER_ARRAY:
				{
					if (abstractHessianOutput.AddRef(obj))
						return;

					int[] arrData = (int[]) obj;
					abstractHessianOutput.WriteListBegin(arrData.Length, "[int");
					for (int i = 0; i < arrData.Length; i++)
						abstractHessianOutput.WriteInt(arrData[i]);
					abstractHessianOutput.WriteListEnd();
					break;
				}

				case STRING_ARRAY:
				{
					if (abstractHessianOutput.AddRef(obj))
						return;

					String[] arrData = (String[]) obj;
					abstractHessianOutput.WriteListBegin(arrData.Length, "[string");
					for (int i = 0; i < arrData.Length; i++)
					{
						abstractHessianOutput.WriteString(arrData[i]);
					}
					abstractHessianOutput.WriteListEnd();
					break;
				}
				case BOOLEAN_ARRAY:
				{
					if (abstractHessianOutput.AddRef(obj))
						return;

					bool[] arrData = (bool[]) obj;
					abstractHessianOutput.WriteListBegin(arrData.Length, "[boolean");
					for (int i = 0; i < arrData.Length; i++)
						abstractHessianOutput.WriteBoolean(arrData[i]);
					abstractHessianOutput.WriteListEnd();
					break;
				}

				case BYTE_ARRAY:
				{
					byte[] arrData = (byte[]) obj;
					abstractHessianOutput.WriteBytes(arrData, 0, arrData.Length);
					break;
				}

                case SBYTE_ARRAY:
                {
                    if (abstractHessianOutput.AddRef(obj))
                        return;

                    sbyte[] arrData = (sbyte[])obj;
                    abstractHessianOutput.WriteListBegin(arrData.Length, "[sbyte");
                    for (int i = 0; i < arrData.Length; i++)
                        abstractHessianOutput.WriteInt(arrData[i]);
                    abstractHessianOutput.WriteListEnd();
                    break;
                }

				case SHORT_ARRAY:
				{
					if (abstractHessianOutput.AddRef(obj))
						return;

					short[] arrData = (short[]) obj;
					abstractHessianOutput.WriteListBegin(arrData.Length, "[short");
					for (int i = 0; i < arrData.Length; i++)
						abstractHessianOutput.WriteInt(arrData[i]);
					abstractHessianOutput.WriteListEnd();
					break;
				}


				case LONG_ARRAY:
				{
					if (abstractHessianOutput.AddRef(obj))
						return;

					long[] arrData = (long[]) obj;
					abstractHessianOutput.WriteListBegin(arrData.Length, "[long");
					for (int i = 0; i < arrData.Length; i++)
						abstractHessianOutput.WriteLong(arrData[i]);
					abstractHessianOutput.WriteListEnd();
					break;
				}

				case FLOAT_ARRAY:
				{
					if (abstractHessianOutput.AddRef(obj))
						return;

					float[] arrData = (float[]) obj;
					abstractHessianOutput.WriteListBegin(arrData.Length, "[float");
					for (int i = 0; i < arrData.Length; i++)
						abstractHessianOutput.WriteDouble(arrData[i]);
					abstractHessianOutput.WriteListEnd();
					break;
				}

				case DOUBLE_ARRAY:
				{
					if (abstractHessianOutput.AddRef(obj))
						return;

					double[] arrData = (double[]) obj;
					abstractHessianOutput.WriteListBegin(arrData.Length, "[double");
					for (int i = 0; i < arrData.Length; i++)
						abstractHessianOutput.WriteDouble(arrData[i]);
					abstractHessianOutput.WriteListEnd();
					break;
				}


				case CHARACTER_ARRAY:
				{
					char[] arrData = (char[]) obj;
					abstractHessianOutput.WriteString(arrData, 0, arrData.Length);
					break;
				}

				case OBJECT_ARRAY:
				{
					if (abstractHessianOutput.AddRef(obj))
						return;

					Object[] arrData = (Object[]) obj;
					abstractHessianOutput.WriteListBegin(arrData.Length, "[object");
					for (int i = 0; i < arrData.Length; i++)
					{
						abstractHessianOutput.WriteObject(arrData[i]);
					}
					abstractHessianOutput.WriteListEnd();
					break;
				}

				default:
					throw new CHessianException(m_intCode + " " + obj.GetType().ToString());
			}
		#endregion
		}

	}
}