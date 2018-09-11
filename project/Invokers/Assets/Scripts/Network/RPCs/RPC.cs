using System;
using System.Collections.Generic;
using System.Reflection;

namespace NetworkService.NetworkMessage
{
    public class RPCHeader : INetObject
    {
        public override Int32 ClassID
        {
            get { return NetConst.ToInt32(NetObjectClasses.RPC_HEADER); }
        }
        public override Dictionary<Int32, Type> ClassIDToType
        {
            get { return CollectClassIDToType(typeof(RPCHeader)); }
        }
        protected override FieldInfo[] FieldsInfo
        {
            get { return CollectFieldsInfo(typeof(RPCHeader)); }
        }

        public RPCHeader() { }

        [MarshalOrder(0)]
        public NetMsgType msg_type = NetMsgType.RPC;
    }

    public class RPCArgs : INetObject
    {
        public override Int32 ClassID
        {
            get { return NetConst.ToInt32(NetObjectClasses.RPC_ARGS); }
        }
        public override Dictionary<Int32, Type> ClassIDToType
        {
            get { return CollectClassIDToType(typeof(RPCArgs)); }
        }
        protected override FieldInfo[] FieldsInfo
        {
            get { return CollectFieldsInfo(typeof(RPCArgs)); }
        }

        public RPCArgs() { }

        [MarshalOrder(0)]
        public List<INetObject> argvs = new List<INetObject>();
    }

    public class RPCBody : INetObject
    {
        public override Int32 ClassID
        {
            get { return NetConst.ToInt32(NetObjectClasses.RPC_CALL); }
        }
        public override Dictionary<Int32, Type> ClassIDToType
        {
            get { return CollectClassIDToType(typeof(RPCBody)); }
        }
        protected override FieldInfo[] FieldsInfo
        {
            get { return CollectFieldsInfo(typeof(RPCBody)); }
        }

        public RPCBody() { }

        [MarshalOrder(0)]
        public RPCHeader header = new RPCHeader();

        [MarshalOrder(1)]
        public string method = "";

        [MarshalOrder(2)]
        public RPCArgs args = new RPCArgs();
    }
}
