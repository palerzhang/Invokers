using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NetworkService.NetworkMessage;

public class Servertest : MonoBehaviour
{
    private NetServer server;

	// Use this for initialization
	void Start ()
    {
        server = new NetServer();
        server.StartServer();

        RPCBody rpc = new RPCBody();
        rpc.method = "test rpc call";
        rpc.args.argvs.Add(new NetInt32(10));
        rpc.args.argvs.Add(new NetString("test argv"));
        rpc.args.argvs.Add(new NetBool(false));
        rpc.args.argvs.Add(new NetChar('C'));

        byte[] bs = rpc.Marshal();

        RPCBody rpc2 = new RPCBody();
        int offset = 0;
        rpc2.Unmarshal(bs, ref offset);
        int x = 0;
    }
	
	// Update is called once per frame
	void Update ()
    {
        Debug.Log("Main thread tick");
	}

    private void OnDestroy()
    {
        server.Shutdown();
    }
}
