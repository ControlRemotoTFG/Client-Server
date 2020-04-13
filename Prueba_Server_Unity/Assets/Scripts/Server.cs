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
using System.Net.NetworkInformation;


public class Server : MonoBehaviour
{

    string IP;
    public System.Int32 Port;
    UDPSocket s;
    public QR qr;
    public PlayerController player;
    public Camera camera;
    // Use this for initialization
    WaitForEndOfFrame frameEnd = new WaitForEndOfFrame();

    public System.Int32 getPort()
    {
        return Port;
    }

    void LateUpdate()
    {
        if (s != null && s.checkSending())
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            camera.Render();
            Texture2D texture = new Texture2D(camera.targetTexture.width, camera.targetTexture.height, TextureFormat.RGB24, false);
            //Read the pixels in the Rect starting at 0,0 and ending at the screen's width and height
            RenderTexture.active = camera.targetTexture;
            texture.ReadPixels(new Rect(0, 0, camera.targetTexture.width, camera.targetTexture.height), 0, 0, false);
            //texture.Compress(false);
            byte[] Bytes2Send = texture.EncodeToPNG();
            //byte[] Bytes2Send = texture.GetRawTextureData(); 
            s.setTexture2D(ref Bytes2Send);
            Destroy(texture);

            watch.Stop();
        }
    }
    public void IniciarServer()
    {          
        s = new UDPSocket();
        s.init(Port,qr);
        s.AddListener(player);
    }

    public void endQRShow()
    {
        qr.endQRShow();
    }

    public static string GetIP()
    {
        string output = "";

        foreach (NetworkInterface item in NetworkInterface.GetAllNetworkInterfaces())
        {
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
            NetworkInterfaceType _type1 = NetworkInterfaceType.Wireless80211;
            NetworkInterfaceType _type2 = NetworkInterfaceType.Ethernet;

            if ((item.NetworkInterfaceType == _type1 || item.NetworkInterfaceType == _type2) && item.OperationalStatus == OperationalStatus.Up)
#endif 
            {
                foreach (UnicastIPAddressInformation ip in item.GetIPProperties().UnicastAddresses)
                {
                    if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                    {
                        output = ip.Address.ToString();
                    }
                }
            }
        }
        return output;
    }

    public void CerrarServer()
    {
        s.StopRunning();
        Application.Quit();

    }
   
}



namespace Server_CSharp
{
    public interface InputMovileInterface
    {
        bool RecieveTouch(int x, int y, ref bool vibrate);

        bool EndOfConection();

        bool ScreenSize(int width, int height);
    }

    public class UDPSocket
    {
        Thread receiveThread;
        Thread sendThread;
        int puerto;
        UdpClient client;
        byte[] data;
        bool continua = true;
        bool sending = true;
        bool send = false;
        bool vibrate = false;
        QR qr;
        IPEndPoint anyIP;
        bool conectado = false;
        byte[] byteImg = new byte[200];
        List<InputMovileInterface> listeners;
        // init
        public void init(int port, QR qr)
        {
            this.qr = qr;
            puerto = port;
            listeners = new List<InputMovileInterface>();
            receiveThread = new Thread(
                new ThreadStart(ReceiveData));
            receiveThread.IsBackground = true;
            receiveThread.Start();
            sendThread = new Thread(
                new ThreadStart(SendData));
            sendThread.IsBackground = true;
            sendThread.Start();
        }

        public void AddListener(InputMovileInterface i)
        {
            listeners.Add(i);
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
        public bool conected()
        {
            return conectado;
        }
        // send thread
        private void SendData() {

            System.Net.Sockets.UdpClient cliente = new System.Net.Sockets.UdpClient();

            
            
            while (!conectado) ;
            cliente.Connect(anyIP.Address,puerto);
            qr.endQRShow();//end the QR
            while (sending)
            {
                if (vibrate)
                {
                    byte[] vibrateMessage = new byte[4];
                    vibrateMessage[0] = 0;
                    vibrateMessage[1] = 0;
                    vibrate = false;
                    cliente.Send(vibrateMessage, vibrateMessage.Length);
                }
                if (send)
                {
                    send = false;
                    cliente.Send(byteImg, byteImg.Length); //este mensaje tiene de latencia 5ms aprox cuando hace ping
                }
                
            }
            byte[] sendData = new byte[1];
            //fin de comunicación
            sendData[0] = 1;
            cliente.Send(sendData, sendData.Length);
            Debug.Log("terminó");
        } 
        // receive thread
        private void ReceiveData()
        {
            
            client = new UdpClient(puerto);
            IPAddress a = GetAddress();
            anyIP = new IPEndPoint(a, puerto);

            Debug.Log(""+ puerto +"_________"+ anyIP.Address);

            data = client.Receive(ref anyIP);//recieve screen size

            // Get the size for the listener
            int pos0 = data[0];
            int pos1 = (data[1] << 4);
            int pos2 = (data[2] << 8);
            int pos3 = (data[3] << 16);

            int pos4 = data[4];
            int pos5 = (data[5] << 4);
            int pos6 = (data[6] << 8);
            int pos7 = (data[7] << 16);

            int x = pos0 + pos1 + pos2 + pos3;
            int y = pos4 + pos5 + pos6 + pos7;
            foreach (InputMovileInterface i in listeners)
            {
                if (i.ScreenSize(x, y))//if the event is consumed we stop passing that event
                    break;
            }


            int f = 0;
            conectado = true;//activamos mandar img
            while (continua)
            {

                try
                {
                    //TODO: Desbloquear este receive o algo para no bloquear la aplicacion en el caso de que queramos salir y no se conecte nadie.
                    Debug.Log("Waiting...");
                    data = client.Receive(ref anyIP); //bloqueante
                    f++;
                    if (f % 3 == 0)
                        vibrate = true;
                    Debug.Log("Waiting Finish");
                    if(data.Length > 1)
                    {
                        //Get the position where the user clicked
                        pos0 = data[0];
                        pos1 = (data[1] << 4);
                        pos2 = (data[2] << 8);
                        pos3 = (data[3] << 16);

                        pos4 = data[4];
                        pos5 = (data[5] << 4);
                        pos6 = (data[6] << 8);
                        pos7 = (data[7] << 16);

                        x = pos0 + pos1 + pos2 + pos3;
                        y = pos4 + pos5 + pos6 + pos7;
                        foreach (InputMovileInterface i in listeners)
                        {
                            bool v = false;
                            if (i.RecieveTouch(x, y, ref v))//Pass the event, if its consumed we stop passing the event
                            {
                                vibrate = v;
                                break;
                            }
                        }
                    }
                    else if(data[0] == 2)
                    {
                        Debug.Log("Terminando las conexiones");
                        continua = false;
                        sending = false;
                    }

                }
                catch (Exception err)
                {
                    Debug.Log("Finalizada conexion manera abrupta");
                    break;
                }
            }
            foreach (InputMovileInterface i in listeners)
            {
                if (i.EndOfConection())
                    break;
            }
        }

        private IPAddress GetAddress()
        {

            foreach (NetworkInterface item in NetworkInterface.GetAllNetworkInterfaces())
            {
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
                NetworkInterfaceType _type1 = NetworkInterfaceType.Wireless80211;
                NetworkInterfaceType _type2 = NetworkInterfaceType.Ethernet;

                if ((item.NetworkInterfaceType == _type1 || item.NetworkInterfaceType == _type2) && item.OperationalStatus == OperationalStatus.Up)
#endif
                {
                    foreach (UnicastIPAddressInformation ip in item.GetIPProperties().UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                        {
                            return ip.Address;
                        }
                    }
                }
            }
            return null;
        }
    }
}

