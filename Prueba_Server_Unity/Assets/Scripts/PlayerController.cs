﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour, Server_CSharp.InputMovileInterface {
    /*Rect up = new Rect(201, 52, 171, 114);
    Rect down = new Rect(201, 288, 171, 114);
    Rect left = new Rect(55, 175, 171, 114);
    Rect right = new Rect(363, 166, 171, 114);  X Y ANCHO ALTO

    Rect A = new Rect(480, 348, 358, 260);
    Rect B = new Rect(611, 689, 347, 239);
    Rect select = new Rect(13, 818, 446, 74);
    Rect start = new Rect(40, 626, 401, 93);*/

    int ScreenW;
    int ScreenH;


    public void RecieveTouch(int x, int y)
    {
        Debug.Log("Moving to " + x + ","+ y);
    }

    public void EndOfConection()
    {
        Debug.Log("F");
    }

    public void ScreenSize(int width, int height)
    {
        Debug.Log("Screen " + width + "," + height);
        ScreenW = width;
        ScreenH = height;
    }
}
