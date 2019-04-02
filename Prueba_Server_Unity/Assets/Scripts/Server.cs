using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Server_CSharp;
using System.Threading;
using UnityEngine.UI;


public class Server : MonoBehaviour
{

    string IP;
    public System.Int32 Port;
    UDPSocket s;
    public PlayerController player;
    public Camera camera;
    // Use this for initialization
    WaitForEndOfFrame frameEnd = new WaitForEndOfFrame();
    public string getIP()
    {
        return IP;
    }
    public System.Int32 getPort()
    {
        return Port;
    }
    void LateUpdate()
    {
        if (s != null && s.checkSending())
        {
            camera.Render();
            Texture2D texture = new Texture2D(camera.targetTexture.width, camera.targetTexture.height, TextureFormat.RGB24, false);
            //Read the pixels in the Rect starting at 0,0 and ending at the screen's width and height
            RenderTexture.active = camera.targetTexture;
            texture.ReadPixels(new Rect(0, 0, camera.targetTexture.width, camera.targetTexture.height), 0, 0, false);
            texture.Compress(false);
            byte[] Bytes2Send = texture.GetRawTextureData(); 
            s.setTexture2D(ref Bytes2Send);
        }
    }
    public void IniciarServer()
    {          
        s = new UDPSocket();
        Debug.Log("New hecho.");
        IP = "192.168.1.33";
        s.init(IP, Port, player);
    }

    private string GetIp()
    {
        IPHostEntry host;
        string localIP = "?";
        host = Dns.GetHostEntry(Dns.GetHostName());

        foreach(IPAddress ip in host.AddressList)
        {

            if (ip.AddressFamily.ToString() == "InterNetwork")
            {
                localIP = ip.ToString();
            }
        }
        return localIP;
    }

    public void CerrarServer()
    {
        s.StopRunning();
        Application.Quit();

    }
   
}



namespace Server_CSharp
{
    public class UDPSocket
    {
        Thread receiveThread;
        Thread sendThread;
        int puerto;
        UdpClient client;
        byte[] data;
        byte[] sendData;
        PlayerController player;
        bool continua = true;
        bool sending = true;
        bool send = false;
        IPEndPoint anyIP;
        bool conectado = false;
        byte[] byteImg = new byte[200];
        // init
        public void init(String ip, int port, PlayerController p)
        {
            Debug.Log("UDPSend.init()");
            player = p;
            puerto = port;
            receiveThread = new Thread(
                new ThreadStart(ReceiveData));
            receiveThread.IsBackground = true;
            receiveThread.Start();
            sendThread = new Thread(
                new ThreadStart(SendData));
            sendThread.IsBackground = true;
            sendThread.Start();
        }
        public void setTexture2D(ref byte[] arrayImg)
        {
            byteImg = arrayImg;
            send = true;
        }
        public void StopRunning()
        {
            continua = false;
        }

        public void StopSending()
        {
            sending = false;
        }
        public void ActivateSending()
        {
            sending = true;
        }
        public void Send()
        {
            send = true;
        }
        public bool checkSending()
        {
            return sending;
        }
        // send thread
        private void SendData() {

            System.Net.Sockets.UdpClient cliente = new System.Net.Sockets.UdpClient();

            sendData = new byte[10];
            
            while (!conectado) ;
            cliente.Connect(anyIP.Address,puerto );
            while (sending)
            {
                if (send)
                {
                    send = false;
                    cliente.Send(byteImg, byteImg.Length);
                    Debug.Log("Se mandó.");
                }
                
            }
            //fin de comunicación
            sendData[0] = 1;
            cliente.Send(sendData, sendData.Length);
            Debug.Log("terminó");
        } 
        // receive thread
        private void ReceiveData()
        {

            client = new UdpClient(puerto);
            byte[] address = new byte[4];
            address[0] = 192;
            address[1] = 162;
            address[2] = 1;
            address[3] = 34;
            IPAddress a = new IPAddress(address);
            anyIP = new IPEndPoint(a, puerto);

            Debug.Log(""+ puerto +"_________"+ anyIP.Address);
           
            while (continua)
            {

                try
                {
                   

                    //TODO: Desbloquear este receive o algo para no bloquear la aplicacion en el caso de que queramos salir y no se conecte nadie.
                    Debug.Log("Waiting...");
                    data = client.Receive(ref anyIP); //bloqueante

                    Debug.Log("" + puerto + "_________" + anyIP.Address);
                    conectado = true;//activamos mandar img
                    if (data[0] == 2)
                    {
                        Debug.Log("Terminando las conexiones");
                        continua = false;
                        sending = false;
                    }
                    else
                    {
                        if (data[0] == 1)
                        {
                            player.SetByteData(0, 1);
                            
                        }
                        else
                            player.SetByteData(0, 0);
                        if (data[1] == 1)
                            player.SetByteData(1, 1);
                        else
                            player.SetByteData(1, 0);
                        if (data[2] == 1)
                            player.SetByteData(2, 1);
                        else
                            player.SetByteData(2, 0);
                        if (data[3] == 1)
                            player.SetByteData(3, 1);
                        else
                            player.SetByteData(3, 0);
                        if (data[4] == 1)
                            player.SetByteData(4, 1);
                        else
                            player.SetByteData(4, 0);
                        if (data[5] == 1)
                            player.SetByteData(5, 1);
                        else
                            player.SetByteData(5, 0);
                        if (data[6] == 1)
                            player.SetByteData(6, 1);
                        else
                            player.SetByteData(6, 0);
                        if (data[7] == 1)
                            player.SetByteData(7, 1);
                        else
                            player.SetByteData(7, 0);
                    }
                }
                catch (Exception err)
                {
                    Debug.Log("Finalizada conexion manera abrupta");
                    break;
                }
            }
        } 
    }
}

