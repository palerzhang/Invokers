using System;
using System.Collections.Generic;
using System.Reflection;

namespace NetworkService.NetworkMessage
{
    public abstract class INetObject : NetworkService.NetworkMessage.BaseMarshallable
    {
        // Nothing
    }

    // base net objects to wrap base type
    public class NetInt32 : INetObject
    {
        public override Int32 ClassID
        {
            get { return NetConst.ToInt32(NetObjectClasses.BASE_INT32); }
        }
        public override Dictionary<Int32, Type> ClassIDToType
        {
            get { return CollectClassIDToType(typeof(NetInt32)); }
        }
        protected override FieldInfo[] FieldsInfo
        {
            get { return CollectFieldsInfo(typeof(NetInt32)); }
        }

        public NetInt32(Int32 val) { value = val; }

        [MarshalOrder(0)]
        Int32 value;
    }

    public class NetUInt32 : INetObject
    {
        public override Int32 ClassID
        {
            get { return NetConst.ToInt32(NetObjectClasses.BASE_UINT32); }
        }
        public override Dictionary<Int32, Type> ClassIDToType
        {
            get { return CollectClassIDToType(typeof(NetUInt32)); }
        }
        protected override FieldInfo[] FieldsInfo
        {
            get { return CollectFieldsInfo(typeof(NetUInt32)); }
        }

        public NetUInt32(UInt32 val) { value = val; }

        [MarshalOrder(0)]
        UInt32 value;
    }

    public class NetInt16 : INetObject
    {
        public override Int32 ClassID
        {
            get { return NetConst.ToInt32(NetObjectClasses.BASE_INT16); }
        }
        public override Dictionary<Int32, Type> ClassIDToType
        {
            get { return CollectClassIDToType(typeof(NetInt16)); }
        }
        protected override FieldInfo[] FieldsInfo
        {
            get { return CollectFieldsInfo(typeof(NetInt16)); }
        }

        public NetInt16(Int16 val) { value = val; }

        [MarshalOrder(0)]
        Int16 value;
    }

    public class NetUInt16 : INetObject
    {
        public override Int32 ClassID
        {
            get { return NetConst.ToInt32(NetObjectClasses.BASE_UINT16); }
        }
        public override Dictionary<Int32, Type> ClassIDToType
        {
            get { return CollectClassIDToType(typeof(NetUInt16)); }
        }
        protected override FieldInfo[] FieldsInfo
        {
            get { return CollectFieldsInfo(typeof(NetUInt16)); }
        }

        public NetUInt16(UInt16 val) { value = val; }

        [MarshalOrder(0)]
        UInt16 value;
    }

    public class NetChar : INetObject
    {
        public override Int32 ClassID
        {
            get { return NetConst.ToInt32(NetObjectClasses.BASE_CHAR); }
        }
        public override Dictionary<Int32, Type> ClassIDToType
        {
            get { return CollectClassIDToType(typeof(NetChar)); }
        }
        protected override FieldInfo[] FieldsInfo
        {
            get { return CollectFieldsInfo(typeof(NetChar)); }
        }

        public NetChar(char val) { value = val; }

        [MarshalOrder(0)]
        char value;
    }

    public class NetByte : INetObject
    {
        public override Int32 ClassID
        {
            get { return NetConst.ToInt32(NetObjectClasses.BASE_BYTE); }
        }
        public override Dictionary<Int32, Type> ClassIDToType
        {
            get { return CollectClassIDToType(typeof(NetByte)); }
        }
        protected override FieldInfo[] FieldsInfo
        {
            get { return CollectFieldsInfo(typeof(NetByte)); }
        }

        public NetByte(byte val) { value = val; }

        [MarshalOrder(0)]
        byte value;
    }

    public class NetBool : INetObject
    {
        public override Int32 ClassID
        {
            get { return NetConst.ToInt32(NetObjectClasses.BASE_BOOL); }
        }
        public override Dictionary<Int32, Type> ClassIDToType
        {
            get { return CollectClassIDToType(typeof(NetBool)); }
        }
        protected override FieldInfo[] FieldsInfo
        {
            get { return CollectFieldsInfo(typeof(NetBool)); }
        }

        public NetBool(bool val) { value = val; }

        [MarshalOrder(0)]
        bool value;
    }

    public class NetSingle : INetObject
    {
        public override Int32 ClassID
        {
            get { return NetConst.ToInt32(NetObjectClasses.BASE_SINGLE); }
        }
        public override Dictionary<Int32, Type> ClassIDToType
        {
            get { return CollectClassIDToType(typeof(NetSingle)); }
        }
        protected override FieldInfo[] FieldsInfo
        {
            get { return CollectFieldsInfo(typeof(NetSingle)); }
        }

        public NetSingle(Single val) { value = val; }

        [MarshalOrder(0)]
        Single value;
    }

    public class NetDouble : INetObject
    {
        public override Int32 ClassID
        {
            get { return NetConst.ToInt32(NetObjectClasses.BASE_DOUBLE); }
        }
        public override Dictionary<Int32, Type> ClassIDToType
        {
            get { return CollectClassIDToType(typeof(NetDouble)); }
        }
        protected override FieldInfo[] FieldsInfo
        {
            get { return CollectFieldsInfo(typeof(NetDouble)); }
        }

        public NetDouble(Double val) { value = val; }

        [MarshalOrder(0)]
        Double value;
    }

    public class NetString : INetObject
    {
        public override Int32 ClassID
        {
            get { return NetConst.ToInt32(NetObjectClasses.BASE_STRING); }
        }
        public override Dictionary<Int32, Type> ClassIDToType
        {
            get { return CollectClassIDToType(typeof(NetString)); }
        }
        protected override FieldInfo[] FieldsInfo
        {
            get { return CollectFieldsInfo(typeof(NetString)); }
        }

        public NetString(string val) { value = val; }

        [MarshalOrder(0)]
        string value;
    }
}
