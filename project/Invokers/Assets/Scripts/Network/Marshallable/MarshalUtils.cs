using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetworkService.NetworkMessage
{
    /// <summary>
    /// This class implements the marshalling/unmarshalling methods of some primitive types.
    /// </summary>
    public static class MarshalUtils
    {
        public delegate byte[] MarshalDelegate(object obj);
        public delegate object UnmarshalDelegate(byte[] bytes, ref int offset);
        /// <summary>
        /// Map a primitive type to its marshalling method.
        /// </summary>
        public static Dictionary<Type, MarshalDelegate> typeToMarshalMethod = new Dictionary<Type, MarshalDelegate>
        {
            { typeof(Int32), new MarshalDelegate(MarshalInt32) },
            { typeof(UInt32), new MarshalDelegate(MarshalUInt32) },
            { typeof(Int16), new MarshalDelegate(MarshalInt16) },
            { typeof(UInt16), new MarshalDelegate(MarshalUInt16) },
            { typeof(char), new MarshalDelegate(MarshalChar) },
            { typeof(byte), new MarshalDelegate(MarshalByte) },
            { typeof(bool), new MarshalDelegate(MarshalBool) },
            { typeof(Single), new MarshalDelegate(MarshalSingle) },
            { typeof(Double), new MarshalDelegate(MarshalDouble) },
            { typeof(string), new MarshalDelegate(MarshalString) }
        };
        /// <summary>
        /// Map a primitive type to its unmarshalling method.
        /// </summary>
        public static Dictionary<Type, UnmarshalDelegate> typeToUnmarshalMethod = new Dictionary<Type, UnmarshalDelegate>
        {
            { typeof(Int32), new UnmarshalDelegate(UnmarshalInt32) },
            { typeof(UInt32), new UnmarshalDelegate(UnmarshalUInt32) },
            { typeof(Int16), new UnmarshalDelegate(UnmarshalInt16) },
            { typeof(UInt16), new UnmarshalDelegate(UnmarshalUInt16) },
            { typeof(char), new UnmarshalDelegate(UnmarshalChar) },
            { typeof(byte), new UnmarshalDelegate(UnmarshalByte) },
            { typeof(bool), new UnmarshalDelegate(UnmarshalBool) },
            { typeof(Single), new UnmarshalDelegate(UnmarshalSingle) },
            { typeof(Double), new UnmarshalDelegate(UnmarshalDouble) },
            { typeof(string), new UnmarshalDelegate(UnmarshalString) }
        };

        /// <summary>
        /// Marshal an Int32 object.
        /// </summary>
        /// <param name="obj">An object of type Int32.</param>
        /// <returns>Marshalled bytes.</returns>
        public static byte[] MarshalInt32(object obj)
        {
            byte[] objBytes = BitConverter.GetBytes((Int32)obj);
            Array.Reverse(objBytes);
            return objBytes;
        }

        /// <summary>
        /// Marshal an UInt32 object.
        /// </summary>
        /// <param name="obj">An object of type UInt32.</param>
        /// <returns>Marshalled bytes.</returns>
        public static byte[] MarshalUInt32(object obj)
        {
            byte[] objBytes = BitConverter.GetBytes((UInt32)obj);
            Array.Reverse(objBytes);
            return objBytes;
        }

        /// <summary>
        /// Marshal an Int16 object.
        /// </summary>
        /// <param name="obj">An object of type Int16.</param>
        /// <returns>Marshalled bytes.</returns>
        public static byte[] MarshalInt16(object obj)
        {
            byte[] objBytes = BitConverter.GetBytes((Int16)obj);
            Array.Reverse(objBytes);
            return objBytes;
        }

        /// <summary>
        /// Marshal an UInt16 object.
        /// </summary>
        /// <param name="obj">An object of type UInt16.</param>
        /// <returns>Marshalled bytes.</returns>
        public static byte[] MarshalUInt16(object obj)
        {
            byte[] objBytes = BitConverter.GetBytes((UInt16)obj);
            Array.Reverse(objBytes);
            return objBytes;
        }

        /// <summary>
        /// Marshal a char object.
        /// </summary>
        /// <param name="obj">An object of type char.</param>
        /// <returns>Marshalled bytes.</returns>
        public static byte[] MarshalChar(object obj)
        {
            // Note that char is unicode in C# and could have two bytes.
            byte[] objBytes = BitConverter.GetBytes((char)obj);
            Array.Reverse(objBytes);
            return objBytes;
        }

        /// <summary>
        /// Marshal a byte object.
        /// </summary>
        /// <param name="obj">An object of type byte.</param>
        /// <returns>Marshalled bytes.</returns>
        public static byte[] MarshalByte(object obj)
        {
            // Note that BitConverter.GetBytes(byte) will treat byte as short.
            byte[] objBytes = new byte[] { (byte)obj };
            return objBytes;
        }

        /// <summary>
        /// Marshal a bool object.
        /// </summary>
        /// <param name="obj">An object of type bool.</param>
        /// <returns>Marshalled bytes.</returns>
        public static byte[] MarshalBool(object obj)
        {
            byte[] objBytes = BitConverter.GetBytes((bool)obj);
            return objBytes;
        }

        /// <summary>
        /// Marshal a Single object.
        /// </summary>
        /// <param name="obj">An object of type Single.</param>
        /// <returns>Marshalled bytes.</returns>
        public static byte[] MarshalSingle(object obj)
        {
            byte[] objBytes = BitConverter.GetBytes((Single)obj);
            Array.Reverse(objBytes);
            return objBytes;
        }

        /// <summary>
        /// Marshal a Double object.
        /// </summary>
        /// <param name="obj">An object of type Double.</param>
        /// <returns>Marshalled bytes.</returns>
        public static byte[] MarshalDouble(object obj)
        {
            byte[] objBytes = BitConverter.GetBytes((Double)obj);
            // In C#, Double is always 8 bytes.
            Array.Reverse(objBytes, 0, 4);
            Array.Reverse(objBytes, 4, 4);
            return objBytes;
        }

        /// <summary>
        /// Marshal a string object.
        /// </summary>
        /// <param name="obj">An object of type string.</param>
        /// <returns>Marshalled bytes.</returns>
        public static byte[] MarshalString(object obj)
        {
            if (obj == null)
            {
                throw new MarshallingFailException("Marshalling before the string is properly initialized.");
            }
            byte[] strBytes = Encoding.UTF8.GetBytes((string)obj);
            byte[] objBytes = BitConverter.GetBytes(strBytes.Length).Reverse().Concat(strBytes).ToArray();
            return objBytes;
        }

        /// <summary>
        /// Unmarshal an Int32 object from bytes.
        /// </summary>
        /// <param name="bytes">Bytes to unmarshal from.</param>
        /// <param name="offset">Starting offset in the bytes.</param>
        /// <returns>An object of type Int32.</returns>
        public static object UnmarshalInt32(byte[] bytes, ref int offset)
        {
            CheckLength(bytes, offset, sizeof(Int32));
            byte[] objBytes = bytes.Skip(offset).Take(sizeof(Int32)).ToArray();
            Array.Reverse(objBytes);
            Int32 obj = BitConverter.ToInt32(objBytes, 0);
            offset += sizeof(Int32);
            return obj;
        }

        /// <summary>
        /// Unmarshal an UInt32 object from bytes.
        /// </summary>
        /// <param name="bytes">Bytes to unmarshal from.</param>
        /// <param name="offset">Starting offset in the bytes.</param>
        /// <returns>An object of type UInt32.</returns>
        public static object UnmarshalUInt32(byte[] bytes, ref int offset)
        {
            CheckLength(bytes, offset, sizeof(UInt32));
            byte[] objBytes = bytes.Skip(offset).Take(sizeof(UInt32)).ToArray();
            Array.Reverse(objBytes);
            UInt32 obj = BitConverter.ToUInt32(objBytes, 0);
            offset += sizeof(UInt32);
            return obj;
        }

        /// <summary>
        /// Unmarshal an Int16 object from bytes.
        /// </summary>
        /// <param name="bytes">Bytes to unmarshal from.</param>
        /// <param name="offset">Starting offset in the bytes.</param>
        /// <returns>An object of type Int16.</returns>
        public static object UnmarshalInt16(byte[] bytes, ref int offset)
        {
            CheckLength(bytes, offset, sizeof(Int16));
            byte[] objBytes = bytes.Skip(offset).Take(sizeof(Int16)).ToArray();
            Array.Reverse(objBytes);
            Int16 obj = BitConverter.ToInt16(objBytes, 0);
            offset += sizeof(Int16);
            return obj;
        }

        /// <summary>
        /// Unmarshal an UInt16 object from bytes.
        /// </summary>
        /// <param name="bytes">Bytes to unmarshal from.</param>
        /// <param name="offset">Starting offset in the bytes.</param>
        /// <returns>An object of type UInt16.</returns>
        public static object UnmarshalUInt16(byte[] bytes, ref int offset)
        {
            CheckLength(bytes, offset, sizeof(UInt16));
            byte[] objBytes = bytes.Skip(offset).Take(sizeof(UInt16)).ToArray();
            Array.Reverse(objBytes);
            UInt16 obj = BitConverter.ToUInt16(objBytes, 0);
            offset += sizeof(UInt16);
            return obj;
        }

        /// <summary>
        /// Unmarshal a char object from bytes.
        /// </summary>
        /// <param name="bytes">Bytes to unmarshal from.</param>
        /// <param name="offset">Starting offset in the bytes.</param>
        /// <returns>An object of type char.</returns>
        public static object UnmarshalChar(byte[] bytes, ref int offset)
        {
            CheckLength(bytes, offset, sizeof(char));
            byte[] objBytes = bytes.Skip(offset).Take(sizeof(char)).ToArray();
            Array.Reverse(objBytes);
            char obj = BitConverter.ToChar(objBytes, 0);
            offset += sizeof(char);
            return obj;
        }

        /// <summary>
        /// Unmarshal a byte object from bytes.
        /// </summary>
        /// <param name="bytes">Bytes to unmarshal from.</param>
        /// <param name="offset">Starting offset in the bytes.</param>
        /// <returns>An object of type byte.</returns>
        public static object UnmarshalByte(byte[] bytes, ref int offset)
        {
            CheckLength(bytes, offset, sizeof(byte));
            byte obj = bytes[offset];
            offset += sizeof(byte);
            return obj;
        }

        /// <summary>
        /// Unmarshal a bool object from bytes.
        /// </summary>
        /// <param name="bytes">Bytes to unmarshal from.</param>
        /// <param name="offset">Starting offset in the bytes.</param>
        /// <returns>An object of type bool.</returns>
        public static object UnmarshalBool(byte[] bytes, ref int offset)
        {
            CheckLength(bytes, offset, sizeof(bool));
            bool obj = BitConverter.ToBoolean(bytes, offset);
            offset += sizeof(bool);
            return obj;
        }

        /// <summary>
        /// Unmarshal a Single object from bytes.
        /// </summary>
        /// <param name="bytes">Bytes to unmarshal from.</param>
        /// <param name="offset">Starting offset in the bytes.</param>
        /// <returns>An object of type Single.</returns>
        public static object UnmarshalSingle(byte[] bytes, ref int offset)
        {
            CheckLength(bytes, offset, sizeof(Single));
            byte[] objBytes = bytes.Skip(offset).Take(sizeof(Single)).ToArray();
            Array.Reverse(objBytes);
            Single obj = BitConverter.ToSingle(objBytes, 0);
            offset += sizeof(Single);
            return obj;
        }

        /// <summary>
        /// Unmarshal a Double object from bytes.
        /// </summary>
        /// <param name="bytes">Bytes to unmarshal from.</param>
        /// <param name="offset">Starting offset in the bytes.</param>
        /// <returns>An object of type Double.</returns>
        public static object UnmarshalDouble(byte[] bytes, ref int offset)
        {
            CheckLength(bytes, offset, sizeof(Double));
            byte[] objBytes = bytes.Skip(offset).Take(sizeof(Double)).ToArray();
            // In C#, Double is always 8 bytes.
            Array.Reverse(objBytes, 0, 4);
            Array.Reverse(objBytes, 4, 4);
            Double obj = BitConverter.ToDouble(objBytes, 0);
            offset += sizeof(Double);
            return obj;
        }

        /// <summary>
        /// Unmarshal a string object from bytes.
        /// </summary>
        /// <param name="bytes">Bytes to unmarshal from.</param>
        /// <param name="offset">Starting offset in the bytes.</param>
        /// <returns>An object of type string.</returns>
        public static object UnmarshalString(byte[] bytes, ref int offset)
        {
            CheckLength(bytes, offset, sizeof(Int32));
            byte[] lengthBytes = bytes.Skip(offset).Take(sizeof(Int32)).ToArray();
            Array.Reverse(lengthBytes);
            Int32 length = BitConverter.ToInt32(lengthBytes, 0);
            offset += sizeof(Int32);
            CheckLength(bytes, offset, length);
            string obj = Encoding.UTF8.GetString(bytes.Skip(offset).Take(length).ToArray());
            offset += length;
            return obj;
        }

        /// <summary>
        /// Check whether the number of available bytes is larger than or equal to the specified length.
        /// </summary>
        /// <param name="bytes">Bytes.</param>
        /// <param name="offset">Starting offset in the bytes.</param>
        /// <param name="length">Length to compare to.</param>
        public static void CheckLength(byte[] bytes, int offset, Int32 length)
        {
            int neededLength = bytes.Length - offset;
            if (neededLength < length)
            {
                throw new UnmarshallingLengthException(neededLength);
            }
        }
    }
}
