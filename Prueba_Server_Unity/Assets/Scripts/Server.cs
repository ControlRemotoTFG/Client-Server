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
    // Use this for initialization

    public string getIP()
    {
        return IP;
    }
    public System.Int32 getPort()
    {
        return Port;
    }
    public void IniciarServer()
    {
        s = new UDPSocket();
        Debug.Log("New hecho.");
        IP = GetIp();
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
        PlayerController player;
        bool continua = true;
        
       
        // init
        public void init(String ip, int port,PlayerController p)
        {

            Debug.Log("UDPSend.init()");
            player = p;
            puerto = port;
            receiveThread = new Thread(
                new ThreadStart(ReceiveData));
            receiveThread.IsBackground = true;
            receiveThread.Start();

        }

        public void StopRunning()
        {
            continua = false;
            
        }

        // receive thread
        private void ReceiveData()
        {

            client = new UdpClient(puerto);
            while (continua)
            {

                try
                {
                    IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);
                    
                    data = client.Receive(ref anyIP); //bloqueante

                    if (data[0] == 2)
                    {
                        continua = false;
                    }
                    else
                    {
                        if (data[0] == 1)
                            player.SetByteData(0, 1);
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

