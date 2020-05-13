using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZXing;
using ZXing.QrCode;
public class QR : MonoBehaviour
{
    Texture2D myQR;
    public Server server;
    GameObject button;
    bool conected = false;
    // Start is called before the first frame update
     public void Generate_QR()
    {
        System.Int32 port = server.getPort();
        string ip = Server.GetIP();
        myQR = generateQR(port + ":" + ip);
    }

    public void endQRShow()
    {
        conected = true;
    }
 
    void OnGUI()
    {

       
        if (!conected && GUI.Button(new Rect((Screen.width/2) - 150, (Screen.height/2) - 150, 256, 256), myQR, GUIStyle.none)) { }

    }
private static Color32[] Encode(string textForEncoding, int width, int height)
    {
        var writer = new BarcodeWriter
        {
            Format = BarcodeFormat.QR_CODE,
            Options = new QrCodeEncodingOptions
            {
                Height = height,
                Width = width
            }
        };
        return writer.Write(textForEncoding);
    }

    private Texture2D generateQR(string text)
    {
        var encoded = new Texture2D(256, 256);
        var color32 = Encode(text, encoded.width, encoded.height);
        encoded.SetPixels32(color32);
        encoded.Apply();
        return encoded;
    }


}
