using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Anonymous.Network;

public class Servertest : MonoBehaviour
{

	// Use this for initialization
	void Start ()
    {
        NetServer server = new NetServer();
        server.StartServer();
	}
	
	// Update is called once per frame
	void Update ()
    {
        Debug.Log("Main thread tick");
	}
}
