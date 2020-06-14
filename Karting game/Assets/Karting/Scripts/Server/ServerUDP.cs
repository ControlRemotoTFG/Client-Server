using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.Net;
using System;
using System.Text;
namespace Server_CSharp
{
    public interface InputMovileInterface
    {
        bool RecieveTouch(int x, int y, int typeOfPress, ref bool vibrate);

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
        IPEndPoint anyIP;
        bool conectado = false;
        byte[] byteImg = new byte[200];
        int vibrationTime;
        List<InputMovileInterface> listeners;
        TrackerInfo trackerInfo;
        // init
        public void init(int port, TrackerInfo trackerInfo, int milisecondsForVibration = 500)
        {
            puerto = port;
            this.trackerInfo = trackerInfo;
            vibrationTime = milisecondsForVibration;
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
        public void StopSending()
        {
            sending = false;
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
        private void SendData()
        {

            System.Net.Sockets.UdpClient cliente = new System.Net.Sockets.UdpClient();



            while (!conectado) ;
            cliente.Connect(anyIP.Address, puerto);
            byte[] timeToVibrate = new byte[4];
            timeToVibrate[0] = (byte)(vibrationTime & 0x000000FF);
            timeToVibrate[1] = (byte)((vibrationTime >> 4) & 0x000000FF);
            timeToVibrate[2] = (byte)((vibrationTime >> 8) & 0x000000FF);
            timeToVibrate[3] = (byte)((vibrationTime >> 16) & 0x000000FF);
            cliente.Send(timeToVibrate, timeToVibrate.Length);

            //miramos latencia con un ping
            Ping pingSender = new Ping();
            PingReply reply = pingSender.Send(anyIP.Address);
            if (reply.Status == IPStatus.Success)
            {
                trackerInfo.AddLatencyOfNetwork((int)(reply.RoundtripTime/2));
                Console.Out.WriteLine("Completed " + reply.RoundtripTime / 2);
            }
            pingSender.Dispose();


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
        }
        // receive thread
        private void ReceiveData()
        {

            client = new UdpClient(puerto);
            IPAddress a = GetAddress();
            anyIP = new IPEndPoint(a, puerto);

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


            conectado = true;//activamos mandar img
            while (continua)
            {

                try
                {
                    //TODO: Desbloquear este receive o algo para no bloquear la aplicacion en el caso de que queramos salir y no se conecte nadie.
                    data = client.Receive(ref anyIP); //bloqueante
                    if (data.Length > 1 && data.Length < 15)
                    {
                        //Get the position where the user clicked
                        int type = data[0];
                        pos0 = data[1];
                        pos1 = (data[2] << 4);
                        pos2 = (data[3] << 8);
                        pos3 = (data[4] << 16);

                        pos4 = data[5];
                        pos5 = (data[6] << 4);
                        pos6 = (data[7] << 8);
                        pos7 = (data[8] << 16);

                        x = pos0 + pos1 + pos2 + pos3;
                        y = pos4 + pos5 + pos6 + pos7;
                        foreach (InputMovileInterface i in listeners)
                        {
                            bool v = false;
                            if (i.RecieveTouch(x, y, type, ref v))//Pass the event, if its consumed we stop passing the event
                            {
                                vibrate = v;
                                break;
                            }
                        }
                    }
                    else if(data.Length >= 15)
                    {
                        int[] timePerImage = new int[data.Length/4];

                        for(int i = 0; i < timePerImage.Length; i++)
                        {
                            int posAct = i * 4;
                            pos0 = data[posAct];
                            pos1 = (data[posAct + 1] << 4);
                            pos2 = (data[posAct + 2] << 8);
                            pos3 = (data[posAct + 3] << 16);
                            timePerImage[i] = pos0 + pos1 + pos2 + pos3;

                            trackerInfo.AddTimePerImageAndroid(timePerImage);
                        }
                    }
                    else if (data[0] == 2)
                    {
                        continua = false;
                        sending = false;
                    }

                }
                catch (Exception err)
                {
                    break;
                }
            }
            foreach (InputMovileInterface i in listeners)
            {
                if (i.EndOfConection())
                    break;
            }

            client.Close();
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
