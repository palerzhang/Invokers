using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace NetworkService.NetworkMessage
{
    /// <summary>
    /// Compare two fields by their MarshalOrder attribute.
    /// </summary>
    class FieldComparer : IComparer<FieldInfo>
    {
        public int Compare(FieldInfo x, FieldInfo y)
        {
            object[] xAttributes = x.GetCustomAttributes(typeof(MarshalOrderAttribute), true);
            object[] yAttributes = y.GetCustomAttributes(typeof(MarshalOrderAttribute), true);
            if (xAttributes.Length < 1)
            {
                throw new ArgumentException(
                    string.Format("The field {0} has {1} MarshalOrderAttribute.", x.Name, xAttributes.Length));
            }
            if (yAttributes.Length < 1)
            {
                throw new ArgumentException(
                    string.Format("The field {0} has {1} MarshalOrderAttribute.", y.Name, yAttributes.Length));
            }
            MarshalOrderAttribute xAttribute = (MarshalOrderAttribute)xAttributes[0];
            MarshalOrderAttribute yAttribute = (MarshalOrderAttribute)yAttributes[0];
            return xAttribute.Order.CompareTo(yAttribute.Order);
        }
    }

    /// <summary>
    /// This attribute must be prepended to every field that 
    /// needs to be marshalled/unmarshalled in every marshallable.
    /// </summary>
    public class MarshalOrderAttribute: Attribute
    {
        public int Order { get; private set; }

        /// <summary>
        /// The marshalling order determines the order that a field 
        /// appears in the byte stream after marshalling.
        /// </summary>
        /// <param name="order">Marshalling order in the byte stream.</param>
        public MarshalOrderAttribute(int order)
        {
            this.Order = order;
        }
    }

    /// <summary>
    /// If a class inherits from this class, then it can marshal to bytes from its public fields that are marked 
    /// with MarshalOrder attribute tagged by a number indicating the marshalling order;
    /// also, it can unmarshal to those fields from bytes. 
    /// The following must be paid attention to when making a derived class:
    /// 1) The fields that need to be marshalled/unmarshalled must be public.
    /// This restriction could be removed in later versions if necessary.
    /// Besides, note that properties are excluded.
    /// 2) The fields that need to be marshalled/unmarshalled must be marked by MarshalOrder(order) attribute.
    /// The order defines in what order the fields will be marshalled/unmarshalled.
    /// 3) The derived class must have a public constructor that takes no arguments.
    /// This is because the .NET implementation of Mono used by Unity seems to be defective in dealing with 
    /// creating instance via constructors with default parameters using reflection.
    /// 4) A subclass must implement FieldsInfo if it is gonna be used for real marshalling/unmarshalling.
    /// 5) A subclass must initialize FieldsInfo (if implemented) in its static constructor with CollectFieldsInfo.
    /// 6) A field declared as a type that is derived from Marshallable (but not BaseMarshallable) must not be null 
    /// when doing Marshal(). If a field could be null when doing Marshal(), 
    /// please make the type of the field a subclass of BaseMarshallable instead.
    /// 7) In an instance of Marshallable subclass (but not BaseMarshallable), 
    /// the type of a field object must be of exactly the same type
    /// as what the field is declared to be---it could not be an instance of a subclass of the field.
    /// If an instance of a subclass of the class needs to be put at the field to implement it kind of abstract,
    /// please declare the field with type as a subclass of BaseMarshallable. But a few rules must be followed
    /// in order to subclass BaseMarshallable properly. See the detail there.
    /// </summary>
    public abstract class Marshallable
    {
        /// <summary>
        /// FieldInfo[] of all fields included in marshalling/unmarshalling.
        /// </summary>
        protected abstract FieldInfo[] FieldsInfo { get; }
        /// <summary>
        /// This flag configures the endianess used in network communciations throughout the whole game.
        /// </summary>
        private bool isUsingLittleEndian = false;

        static Marshallable()
        {
        }

        public Marshallable()
        {
        }

        /// <summary>
        /// Return the FieldInfo of the fields of callerType that are marked by MarshalOrder(),
        /// including the fields declared in the base classes. Fields from base classes 
        /// precede fields from subclasses.
        /// This method can be used in the static constructor of a Marshallable subclass.
        /// </summary>
        /// <param name="callerType">The type of the caller. It is named callerType because
        /// this method is supposed to be called to initialize FieldsInfo of a Marshallable
        /// subclass at its static constructor, though a caller can certainly use this
        /// method to get the fields info of any classes.
        /// This method can be made more convenient by removing the parameter and letting it
        /// infer the caller's type by System.Diagnostics.StackFrame(1). It did work on 
        /// most platforms, e.g. Windows, MacOS, and Android. However, it does not work
        /// correctly on iOS, for unknown reasons. So, we let the caller pass in its type
        /// manually for the sake of better compatibility.</param>
        /// <returns>Collected fields info of the callerType.</returns>
        protected static FieldInfo[] CollectFieldsInfo(Type callerType)
        {
            FieldInfo[] parentFieldInfo;
            Type callerParentType = callerType.BaseType;
            if (callerParentType == typeof(object))
            {
                // The callerType should be Marshallable itself.
                parentFieldInfo = new FieldInfo[] { };
            }
            else
            {
                if (callerParentType.IsSubclassOf(typeof(Marshallable)) || callerParentType == typeof(Marshallable))
                {
                    parentFieldInfo = CollectFieldsInfo(callerParentType);
                }
                else
                {
                    throw new InvalidMarshallableException(string.Format("Caller type {0} has an invalid parent type: {1}",
                        callerType, callerParentType));
                }
            }
            FieldInfo[] declaredFieldsInfo = callerType.GetFields(
                BindingFlags.Public |
                BindingFlags.Instance |
                BindingFlags.DeclaredOnly);
            FieldInfo[] markedFieldsInfo = FilterAndSortFields(declaredFieldsInfo);

            return parentFieldInfo.Concat(markedFieldsInfo).ToArray();
        }

        /// <summary>
        /// Filter the fields info to preserve only the fields that are marked by MarshalOrder,
        /// and sort the filtered fields.
        /// </summary>
        /// <param name="declaredFieldsInfo">Fields to be filtered and sorted.</param>
        /// <returns>Filtered and sorted fields info.</returns>
        protected static FieldInfo[] FilterAndSortFields(FieldInfo[] declaredFieldsInfo)
        {
            List<FieldInfo> fieldsList = declaredFieldsInfo.ToList();
            List<int> toRemoveIndices = new List<int>();
            for (int i = 0; i < fieldsList.Count; i++)
            {
                object[] fieldAttributes = fieldsList[i].GetCustomAttributes(typeof(MarshalOrderAttribute), true);
                if (fieldAttributes.Length != 1)
                {
                    toRemoveIndices.Add(i);
                    if (fieldAttributes.Length > 1)
                    {
                        // Prohibit the case where there are multiple MarshalOrder attributes.
                        throw new InvalidMarshallableException(string.Format("MarshalOrder marked {0} times.", fieldAttributes.Length));
                    }
                    continue;
                }
            }
            foreach (int index in toRemoveIndices.OrderByDescending(v => v))
            {
                // Fields that have not been marked with MarshalOrder attribute 
                // will not be marshalled from or unmarshalled to.
                fieldsList.RemoveAt(index);
            }
            declaredFieldsInfo = fieldsList.ToArray();
            FieldComparer fieldComparer = new FieldComparer();
            Array.Sort(declaredFieldsInfo, (x, y) => { return fieldComparer.Compare(x, y); });

            // Make sure there are no duplicate attributes.
            int lastOrder = -1;
            for(int i = 0; i < declaredFieldsInfo.Length; i++)
            {
                MarshalOrderAttribute attribute = (MarshalOrderAttribute)declaredFieldsInfo[i]
                    .GetCustomAttributes(typeof(MarshalOrderAttribute), true)[0];
                if (i == 0)
                {
                    lastOrder = attribute.Order;
                }
                else if (attribute.Order == lastOrder)
                {
                    throw new InvalidMarshallableException(string.Format("Field {0} has a conflicting ID {1}.", declaredFieldsInfo[i].Name, lastOrder));
                }
            }
            return fieldsList.ToArray();
        }

        /// <summary>
        /// If the endianness does not match isUsingLittleEndian, then
        /// reverse the bytes starting from index and with the specified length.
        /// </summary>
        /// <param name="bytes">Bytes to be reversed.</param>
        /// <param name="index">Starting index.</param>
        /// <param name="length">Length.</param>
        public void Reverse(byte[] bytes, int index, int length) 
        {
            if (isUsingLittleEndian == BitConverter.IsLittleEndian)
            {
                // do nothing
            }
            else
            {
                Array.Reverse(bytes, index, length);
            }
        }

        /// <summary>
        /// If the endianness does not match isUsingLittleEndian, then reverse the bytes.
        /// </summary>
        /// <param name="bytes">Bytes to be reversed.</param>
        public void Reverse(byte[] bytes) 
        {
            if (isUsingLittleEndian == BitConverter.IsLittleEndian)
            {
                // do nothing
            }
            else
            {
                Array.Reverse(bytes);
            }
        }

        /// <summary>
        /// Marshal this object into bytes.
        /// </summary>
        /// <returns>Marshalled bytes</returns>
        public byte[] Marshal()
        {
            List<byte> bytesList = new List<byte>();
            foreach (FieldInfo fieldInfo in FieldsInfo)
            {
                byte[] fieldBytes;
                object fieldObject = fieldInfo.GetValue(this);
                fieldBytes = MarshalObject(fieldObject, fieldInfo.FieldType);
                bytesList.AddRange(fieldBytes);
            }
            return bytesList.ToArray();
        }

        /// <summary>
        /// Marshal an object into bytes.
        /// </summary>
        /// <param name="obj">Object to be marshalled.</param>
        /// <returns></returns>
        public static byte[] MarshalObject(object obj, Type declaredType)
        {
            byte[] objBytes;
            if (MarshalUtils.typeToMarshalMethod.ContainsKey(declaredType))
            {
                MarshalUtils.MarshalDelegate marshalDelegate = MarshalUtils.typeToMarshalMethod[declaredType];
                objBytes = marshalDelegate(obj);
            }
            else if (declaredType.IsEnum)
            {
                Type underliningType = Enum.GetUnderlyingType(declaredType);
                objBytes = MarshalObject(Convert.ChangeType(obj, underliningType), underliningType);
            }
            else if (declaredType.IsSubclassOf(typeof(BaseMarshallable)))
            {
                if (obj == null)
                {
                    objBytes = MarshalObject(0, typeof(Int32)); // Use ID 0 to indicate null.
                }
                else
                {
                    // Int32 classID = (Int32)declaredType.GetProperty("ClassID").GetValue(obj, new object[] { });
                    Int32 classID = ((BaseMarshallable)obj).ClassID;
                    byte[] classIDBytes = MarshalObject(classID, typeof(Int32));
                    byte[] marshallabledBytes = ((Marshallable)obj).Marshal();
                    objBytes = classIDBytes.Concat(marshallabledBytes).ToArray();
                }
            }
            else if (declaredType.IsSubclassOf(typeof(Marshallable)))
            {
                if (obj == null)
                {
                    throw new MarshallingFailException("Marshalling before the object is properly initialized.");
                }
                if (obj.GetType() != declaredType)
                {
                    throw new MarshallingFailException(
                        "A field declared as Marshallable (but not BaseMarshallable) " +
                        "must be of exactly the same type as declared. Use BaseMarshallable " +
                        "instead if you need to store a subclass object at a field whose type " +
                        "is declared as the base class");
                }
                objBytes = ((Marshallable)obj).Marshal();
            }
            else if (declaredType.IsGenericType && declaredType.GetGenericTypeDefinition() == typeof(List<>))
            {
                PropertyInfo countInfo = declaredType.GetProperty("Count");
                Int32 length = (Int32)countInfo.GetValue(obj, new object[] { });
                IEnumerable<byte> objEnumerable = BitConverter.GetBytes(length).Reverse();
                Type[] listGenericArgument = declaredType.GetGenericArguments();
                if (listGenericArgument.Length != 1)
                {
                    throw new MarshallingFailException(string.Format(
                        "Got incorrect generic arguments when marshalling an object of type {0}", declaredType));
                }
                Type declaredElementType = listGenericArgument[0];
                for (int i = 0; i < length; i++)
                {
                    object element = declaredType.GetProperty("Item").GetValue(obj, new object[] { i });
                    objEnumerable = objEnumerable.Concat(MarshalObject(element, declaredElementType));
                }
                objBytes = objEnumerable.ToArray();
            }
            else if (declaredType.IsGenericType && declaredType.GetGenericTypeDefinition() == typeof(Dictionary<,>))
            {
                PropertyInfo countInfo = declaredType.GetProperty("Count");
                Int32 length = (Int32)countInfo.GetValue(obj, new object[] { });
                IEnumerable<byte> objEnumerable = BitConverter.GetBytes(length).Reverse();
                MethodInfo enumeratorInfo = declaredType.GetMethod("GetEnumerator");
                object enumerator = enumeratorInfo.Invoke(obj, new object[] { });
                PropertyInfo currentPropertyInfo = enumerator.GetType().GetProperty("Current");
                MethodInfo moveNextMethodInfo = enumerator.GetType().GetMethod("MoveNext");
                Type[] dictGenericArgument = declaredType.GetGenericArguments();
                if (dictGenericArgument.Length != 2)
                {
                    throw new MarshallingFailException(string.Format(
                        "Got incorrect generic arguments when marshalling an object of type {0}", declaredType));
                }
                Type keyType = dictGenericArgument[0];
                Type valueType = dictGenericArgument[1];
                while ((bool)moveNextMethodInfo.Invoke(enumerator, new object[] { }))
                {
                    object keyValuePair = currentPropertyInfo.GetValue(enumerator, new object[] { });
                    PropertyInfo keyInfo = keyValuePair.GetType().GetProperty("Key");
                    PropertyInfo valueInfo = keyValuePair.GetType().GetProperty("Value");
                    object key = keyInfo.GetValue(keyValuePair, new object[] { });
                    object value = valueInfo.GetValue(keyValuePair, new object[] { });
                    objEnumerable = objEnumerable.Concat(MarshalObject(key, keyType));
                    objEnumerable = objEnumerable.Concat(MarshalObject(value, valueType));
                }
                objBytes = objEnumerable.ToArray();
            }
            else
            {
                throw new MarshallingFailException(string.Format("Cannot marshal object of type {0}", declaredType));
            }
            return objBytes;
        }

        /// <summary>
        /// Unmarshal bytes to this object.
        /// </summary>
        /// <param name="bytes">Bytes to unmarshal from.</param>
        /// <param name="offset">Starting index at the bytes. 
        /// It will be changed to the new byte index after the unmarshalling consumes some bytes.</param>
        public void Unmarshal(byte[] bytes, ref int offset)
        {
            foreach (FieldInfo fieldInfo in FieldsInfo)
            {
                object fieldObject;
                UnmarshalObject(out fieldObject, fieldInfo.FieldType, bytes, ref offset);
                fieldInfo.SetValue(this, fieldObject);
            }
        }

        /// <summary>
        /// Unmarshal bytes to an object.
        /// </summary>
        /// <param name="obj">The object to unmarshal to.</param>
        /// <param name="declaredType">The type of the object to unmarshal to.
        /// It is needed because obj could be null in general, and thus its type could not be relied on.</param>
        /// <param name="bytes">Bytes to unmarshal from.</param>
        /// <param name="offset">Starting index at the bytes.</param>
        public static void UnmarshalObject(out object obj, Type declaredType, byte[] bytes, ref int offset)
        {
            if (MarshalUtils.typeToUnmarshalMethod.ContainsKey(declaredType))
            {
                MarshalUtils.UnmarshalDelegate unmarshalDelegate = MarshalUtils.typeToUnmarshalMethod[declaredType];
                obj = unmarshalDelegate(bytes, ref offset);
            }
            else if (declaredType.IsEnum)
            {
                Type underliningType = Enum.GetUnderlyingType(declaredType);
                object underliningObj;
                UnmarshalObject(out underliningObj, underliningType, bytes, ref offset);
                obj = Enum.ToObject(declaredType, underliningObj);
            }
            else if (declaredType.IsSubclassOf(typeof(BaseMarshallable)))
            {
                object classIDObj;
                UnmarshalObject(out classIDObj, typeof(Int32), bytes, ref offset);
                Int32 classID = (Int32)classIDObj;
                
                if (classID == 0)
                {
                    obj = null; // Class ID 0 indicates that it is null and does not need further unmarshalling.
                }
                else
                {
                    object declaredTypeObj = System.Activator.CreateInstance(declaredType);
                    Dictionary<Int32, Type> classIDToType = (Dictionary<Int32, Type>)declaredType
                        .GetProperty("ClassIDToType").GetValue(declaredTypeObj, new object[] { });
                    if (!classIDToType.ContainsKey(classID))
                    {
                        throw new UnmarshallingFailException(0, string.Format(
                            "Class ID {0} is invalid for {1}", classID, declaredType));
                    }
                    Type objType = classIDToType[classID];
                    obj = System.Activator.CreateInstance(objType);
                    ((BaseMarshallable)obj).Unmarshal(bytes, ref offset);
                }
            }
            else if (declaredType.IsSubclassOf(typeof(Marshallable)))
            {
                obj = System.Activator.CreateInstance(declaredType);
                ((Marshallable)obj).Unmarshal(bytes, ref offset);
            }
            else if (declaredType.IsGenericType && declaredType.GetGenericTypeDefinition() == typeof(List<>))
            {
                MarshalUtils.CheckLength(bytes, offset, sizeof(Int32));
                byte[] lengthBytes = bytes.Skip(offset).Take(sizeof(Int32)).ToArray();
                offset += sizeof(Int32);
                Array.Reverse(lengthBytes);
                Int32 length = BitConverter.ToInt32(lengthBytes, 0);
                Type[] listGenericArgument = declaredType.GetGenericArguments();
                if (listGenericArgument.Length != 1)
                {
                    throw new UnmarshallingFailException(0, string.Format(
                        "Got incorrect generic arguments when unmarshalling an object of type {0}", declaredType));
                }
                Type elementType = listGenericArgument[0];
                Type listType = typeof(List<>);
                Type genericListType = listType.MakeGenericType(elementType);
                obj = System.Activator.CreateInstance(genericListType);
                MethodInfo listAddMethod = genericListType.GetMethod("Add");
                for (int i = 0; i < length; i++)
                {
                    object element;
                    UnmarshalObject(out element, elementType, bytes, ref offset);
                    listAddMethod.Invoke(obj, new object[] { element });
                }
            }
            else if (declaredType.IsGenericType && declaredType.GetGenericTypeDefinition() == typeof(Dictionary<,>))
            {
                MarshalUtils.CheckLength(bytes, offset, sizeof(Int32));
                byte[] lengthBytes = bytes.Skip(offset).Take(sizeof(Int32)).ToArray();
                offset += sizeof(Int32);
                Array.Reverse(lengthBytes);
                Int32 length = BitConverter.ToInt32(lengthBytes, 0);
                Type[] dictGenericArgument = declaredType.GetGenericArguments();
                if (dictGenericArgument.Length != 2)
                {
                    throw new UnmarshallingFailException(0, string.Format(
                        "Got incorrect generic arguments when unmarshalling an object of type {0}", declaredType));
                }
                Type keyType = dictGenericArgument[0];
                Type valueType = dictGenericArgument[1];
                Type dictType = typeof(Dictionary<,>);
                Type genericDictType = dictType.MakeGenericType(new Type[] { keyType, valueType });
                obj = System.Activator.CreateInstance(genericDictType);
                MethodInfo dictAddMethod = genericDictType.GetMethod("Add");
                for (int i = 0; i < length; i++)
                {
                    object key;
                    object value;
                    UnmarshalObject(out key, keyType, bytes, ref offset);
                    UnmarshalObject(out value, valueType, bytes, ref offset);
                    dictAddMethod.Invoke(obj, new object[] { key, value });
                }
            }
            else
            {
                throw new UnmarshallingFailException(0, string.Format("Cannot unmarshal object of type {0}.", declaredType));
            }
        }
    }

    /// <summary>
    /// This exception suggests that an invalid Marshallable is found.
    /// </summary>
    public class InvalidMarshallableException : Exception
    {
        public InvalidMarshallableException() : base()
        {
        }

        public InvalidMarshallableException(string message) : base(message)
        {
        }

        public InvalidMarshallableException(string message, Exception inner) : base(message, inner)
        {
        }
    }

    /// <summary>
    /// An exception to indicate a failure of marshalling.
    /// </summary>
    public class MarshallingFailException : Exception
    {
        public MarshallingFailException() : base()
        {
        }

        public MarshallingFailException(string message) : base(message)
        {
        }

        public MarshallingFailException(string message, Exception inner) : base(message, inner)
        {
        }
    }

    /// <summary>
    /// An exception to indicate a failure of unmarshalling.
    /// </summary>
    public class UnmarshallingFailException : Exception
    {
        public int consumedLength;

        public UnmarshallingFailException(int consumedLength) : base()
        {
            this.consumedLength = consumedLength;
        }

        public UnmarshallingFailException(int consumedLength, string message) : base(message)
        {
            this.consumedLength = consumedLength;
        }

        public UnmarshallingFailException(int consumedLength, string message, Exception inner) : base(message, inner)
        {
            this.consumedLength = consumedLength;
        }
    }

    /// <summary>
    /// This exception suggests that the received bytes are not sufficient to 
    /// unmarshal a message.
    /// This is a "normal" exception, because the way to deal with it is just
    /// to catch it and wait for more bytes.
    /// </summary>
    public class UnmarshallingLengthException : Exception
    {
        public readonly int expectedLength;
        public UnmarshallingLengthException(int expectedLength) : base()
        {
            this.expectedLength = expectedLength;
        }

        public UnmarshallingLengthException(int expectedLength, string message) : base(message)
        {
            this.expectedLength = expectedLength;
        }

        public UnmarshallingLengthException(int expectedLength, string message, Exception inner) : base(message, inner)
        {
            this.expectedLength = expectedLength;
        }
    }
}
