using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    byte[] data;

    void Start()
    {
        data = new byte[8];
        data[0] = data[1] = data[2] = data[3] = data[4] = data[5] = data[6] = data[7] = 0 ;
    }
    public void SetByteData(int indice, byte valor)
    {
        data[indice] = valor;

    }
	 void Update () {
        transform.Translate(0, 0, 0);
        if (data != null)
        {
            if (data[0] == 1)
                transform.Translate(0, 0.1f, 0);
            if (data[1] == 1)
                transform.Translate(0, -0.1f, 0);
            if (data[2] == 1)
                transform.Translate(-0.1f, 0, 0);
            if (data[3] == 1)
                transform.Translate(0.1f, 0, 0);
            if (data[4] == 1)
                transform.Translate(0, 0.1f, 0);
            if (data[5] == 1)
                transform.Translate(0, -0.1f, 0);
            if (data[6] == 1)
                transform.Translate(-0.1f, 0, 0);
            if (data[7] == 1)
                transform.Translate(0.1f, 0, 0);
        }
    }
}
