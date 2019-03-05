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
        int puertoMovil;
        UdpClient client;
        UdpClient movil;
        byte[] data;
        byte[] sendData;
        PlayerController player;
        bool continua = true;
        bool sending = true;
        bool send = true;
        // init
        public void init(String ip, int port, PlayerController p)
        {
            Debug.Log("UDPSend.init()");
            player = p;
            puerto = port;
            receiveThread = new Thread(
                new ThreadStart(ReceiveData));
            sendThread = new Thread(
                new ThreadStart(SendData));
            receiveThread.IsBackground = true;
            receiveThread.Start();
            sendThread.IsBackground = true;
            sendThread.Start();
        }

        public void StopRunning()
        {
            continua = false;
        }

        public void StopSending()
        {
            sending = false;
        }

        public void Send()
        {
            send = true;
        }

        // send thread
        private void SendData() {
            movil = new UdpClient(2004);
            sendData = new byte[10];
            int cont = 0;
            while (sending)
            {
                send = true;
                if (send)
                {
                    if (cont < 10)
                        sendData[0] = 5;
                    else if (cont >= 10 && cont < 16)
                        sendData[0] = 7;
                    else
                        sending = false;
                    movil.Send(sendData,10);
                }
                cont++;
            }
            //fin de comunicación
            sendData[0] = 1;
            movil.Send(sendData, 10);
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
                    
                    //TODO: Desbloquear este receive o algo para no bloquear la aplicacion en el caso de que queramos salir y no se conecte nadie.
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

