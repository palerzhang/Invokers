using System;

namespace NetworkService.NetworkMessage
{
    public enum NetMsgType : UInt16
    {
        RPC = 0,
    }

    public class NetConst
    {
        public static Int32 ToInt32(NetObjectClasses v)
        {
            return (Int32)v;
        }
    }
    public enum NetObjectClasses : Int32
    {
        // base
        BASE_INT32,
        BASE_UINT32,
        BASE_INT16,
        BASE_UINT16,
        BASE_CHAR,
        BASE_BYTE,
        BASE_BOOL,
        BASE_SINGLE,
        BASE_DOUBLE,
        BASE_STRING,
        // rpc
        RPC_HEADER,
        RPC_ARGS,
        RPC_CALL,
    }

}