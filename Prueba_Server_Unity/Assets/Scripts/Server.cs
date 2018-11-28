using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Server_CSharp;
using System.Threading;


public class Server : MonoBehaviour
{

    public String IP;
    public System.Int32 Port;
    UDPSocket s;
    // Use this for initialization
    void Start()
    {

        s = new UDPSocket();
        Debug.Log("New hecho.");
        s.init(IP, Port);
        Debug.Log("Servidor creado en puerto " + Port + " y con la ip " + IP);



        Console.ReadLine();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public byte[] MandoState()
    {
        return s.TakeCommands();
    }
   
}



namespace Server_CSharp
{
    public class UDPSocket
    {
        Thread receiveThread;
        int puerto;
        UdpClient client;
         byte[] data;

        // init
        public void init(String ip, int port)
        {
            Debug.Log("UDPSend.init()");
            puerto = port;
            receiveThread = new Thread(
                new ThreadStart(ReceiveData));
            receiveThread.IsBackground = true;
            receiveThread.Start();



        }

        // receive thread
        private void ReceiveData()
        {

            client = new UdpClient(puerto);
            while (true)
            {

                try
                {
                    IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);
                    data = client.Receive(ref anyIP);

                    if(data[0] == 1)
                        Debug.Log("up");
                     if (data[1] == 1)
                        Debug.Log("down");
                     if (data[2] == 1)
                        Debug.Log("left");
                     if (data[3] == 1)
                        Debug.Log("right");
                     if (data[4] == 1)
                        Debug.Log("A");
                     if (data[5] == 1)
                        Debug.Log("B");
                     if (data[6] == 1)
                        Debug.Log("START");
                     if (data[7] == 1)
                        Debug.Log("SELECT");


                }
                catch (Exception err)
                {
                    break;
                    Debug.Log(err.ToString());
                }
            }
        }

        public byte[] TakeCommands()
        {
            byte[] aux = data;
            data = null;
            return aux;
        }
    }
}