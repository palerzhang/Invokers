using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace NetworkService.NetworkMessage
{
    /// <summary>
    /// The BaseMarshallable class is a Marshallable subclass that 
    /// supports storing subclass objects at the fields that are declared 
    /// with type as a base class.
    /// To enable a receiver to unmarshal correctly, a class ID must be 
    /// prepended to the marshalled bytes of the marshallable; also, the
    /// receiver needs a map from class ID to Marshallable class type.
    /// Due to these requirements and some programming restrictions, the
    /// following must be borne in mind when subclassing BaseMarshallable.
    /// 0) Everything that needs to be taken care of when subclassing Marshallable 
    /// also needs to be taken care of when subclasssing BaseMarshallable. 
    /// 1) The subclass must not be abstract. Or more exactly, if a field
    /// declared as base class needs to store a subclass object, then the 
    /// declared type must not be an abstract class.
    /// 2) The subclass must implement the ClassIDToType property.
    /// 3) The subclass must implement the ClassID property.
    /// Clearly, a non-abstract class must have all abstract properties implemented.
    /// 4) If a subclass implements ClassIDToType property using CollectClassIDToType 
    /// and it has subclasses, those subclasses must reside in the same namespace 
    /// as that of the subclass that implements ClassIDToType. In addition, those 
    /// subclasses must implement ClassID to enable getting a valid ClassID through it.
    /// These requirements may look ugly. In fact, some of them originate from the 
    /// fundamental problem that a method cannot be marked as both virtual and static.
    /// The properties ClassID and ClassIDToType must be implemented carefully 
    /// to reduce the overhead. Please refer to BaseMarshallableTest for 
    /// an example.
    /// </summary>
    public abstract class BaseMarshallable : Marshallable
    {
        public abstract Int32 ClassID { get; }
        public abstract Dictionary<Int32, Type> ClassIDToType { get; }

        public BaseMarshallable() : base()
        {
        }

        /// <summary>
        /// Use reflection to collect the caller's type and all subclasses of the 
        /// caller's type, but a class will be included only if it has declared 
        /// (implemented) ClassID on its own (rather than by inheriting), and then
        /// make a dictionary that maps the ClassID of a collected type to the type.
        /// This method could be used in the static constructor of a subclass
        /// of BaseMarshallable to initialize the underlining static dictionary
        /// of ClassIDToType.
        /// Note that this method only look at the subclasses that reside in the
        /// same namespace as that of the caller's and its subnamespaces.
        /// </summary>
        /// <returns>A dictionary that maps class ID to types.</returns>
        protected static Dictionary<Int32, Type> CollectClassIDToType(Type callerType)
        {
            /*
            System.Diagnostics.StackFrame frame = new System.Diagnostics.StackFrame(1);
            System.Reflection.MethodBase callerMethodBase = frame.GetMethod();
            Type callerType = callerMethodBase.DeclaringType;
            */
            List<Type> classesList = CommonUtils.InitUtils.GetSubclassesInNameSpace(
                callerType, callerType.Namespace);
            classesList.Add(callerType); // Include the caller itself.
            Dictionary<Int32, Type> classDict = new Dictionary<int, Type>();
            foreach (Type classType in classesList)
            {
                object classTypeObj = System.Activator.CreateInstance(classType);
                /*
                PropertyInfo classIDPropertyInfo = classType.GetProperty("ClassID",
                    BindingFlags.Public |
                    BindingFlags.Instance |
                    BindingFlags.DeclaredOnly);*/
                PropertyInfo classIDPropertyInfo = classType.GetProperty("ClassID");
                if (classIDPropertyInfo == null)
                {
                    // Ignore the classes with IDs that are declared in their base classes.
                    continue;
                }
                try
                {
                    Int32 classID = (Int32)classIDPropertyInfo.GetValue(classTypeObj, new object[] { });
                    if (classDict.ContainsKey(classID))
                    {
                        if (classDict[classID].IsSubclassOf(classType))
                        {
                            classDict[classID] = classType;
                        }
                        else
                        {
                            // NOTE: could be sibling classes.
                        }
                    }
                    else
                    {
                        classDict.Add(classID, classType);
                    }
                }
                catch (TargetInvocationException)
                {
                    // Sometimes it could be NotImplemented if a class subclasses a subclass of BaseMarshallable,
                    // and serve as declared class type.
                    // Do nothing
                }
            }
            return classDict;
        }
    }
}
