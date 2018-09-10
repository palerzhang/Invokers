using System.Collections.Generic;
using System;

namespace Anonymous
{
    namespace Network
    {
        using INetObject = NetworkService.NetworkMessage.BaseMarshallable;

        public class NetObjectFactory
        {
            private static Dictionary<NetObjectClasses, Type> NetClasses;
            private static NetObjectFactory instance;
            public static NetObjectFactory Instance
            {
                get { return instance; }
            }

            private NetObjectFactory() { }
            static NetObjectFactory()
            {
                // TODO: 
                // prepare dictionary
            }

            INetObject CreateObject(NetObjectClasses classId)
            {
                if (NetClasses.ContainsKey(classId))
                {
                    return (INetObject)(NetClasses[classId].Assembly.CreateInstance(NetClasses[classId].FullName));
                }
                return null;
            }
        }
    } // end network
} // end anonymous

