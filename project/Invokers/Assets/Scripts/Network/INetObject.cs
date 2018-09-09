namespace Anonymous
{
    namespace Network
    {
        public abstract class INetObject
        {
            public abstract byte[] Serialize();
            public abstract INetObject Deserialize(byte[] data);
            public static NetObjectClasses GetClassId(byte[] data)
            {
                return NetObjectClasses.test;
            }
        }
    } // end network
} // end anonymous

