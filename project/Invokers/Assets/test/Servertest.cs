using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;

public class Servertest : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Socket soc = null;
        System.Byte[] bs = { };
        soc.Send(bs);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
