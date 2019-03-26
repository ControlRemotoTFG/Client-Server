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
        IP = "192.168.1.47";
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
        bool sending = false;
        bool send = true;
        IPEndPoint anyIP;
        bool conectado = false;
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
        // send thread
        private void SendData() {

            System.Net.Sockets.UdpClient cliente = new System.Net.Sockets.UdpClient();

            sendData = new byte[10];
            int cont = 9;
            while (!conectado) ;
            cliente.Connect(anyIP.Address,puerto );
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

                     //s.SendTo(sendData, sendData.Length, SocketFlags.None, anyIP);
                     

                    cliente.Send(sendData,sendData.Length);
                    Debug.Log("Se mandó.");
                    
                }
                cont--;
            }
            //fin de comunicación
            sendData[0] = 1;
            cliente.Send(sendData, sendData.Length);
            Debug.Log("termionó");
        } 
        // receive thread
        private void ReceiveData()
        {

            client = new UdpClient(puerto);
            anyIP = new IPEndPoint(IPAddress.Any, puerto);
           
            while (continua)
            {

                try
                {
                   

                    //TODO: Desbloquear este receive o algo para no bloquear la aplicacion en el caso de que queramos salir y no se conecte nadie.
                    Debug.Log("Waiting...");
                    data = client.Receive(ref anyIP); //bloqueante


                    conectado = true;//activamos mandar img
                    if (data[0] == 2)
                    {
                        continua = false;
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

