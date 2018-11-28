using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public Server server;
    byte[] data;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        transform.Translate(0, 0, 0);
        data = server.MandoState();

        if (data != null)
        {
            if (data[0] == 1)
                transform.Translate(0, 1, 0);
            if (data[1] == 1)
                transform.Translate(0, -1, 0);
            if (data[2] == 1)
                transform.Translate(-1, 0, 0);
            if (data[3] == 1)
                transform.Translate(1, 0, 0);
            if (data[4] == 1)
                transform.Translate(0, 5, 0);
            if (data[5] == 1)
                transform.Translate(0, -5, 0);
            if (data[6] == 1)
                transform.Translate(-5, 0, 0);
            if (data[7] == 1)
                transform.Translate(5, 0, 0);
        }
    }
}
