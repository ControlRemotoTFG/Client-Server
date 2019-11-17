using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System;

/// <summary>
/// Class for managing sending images and reciving input from a phone through a Internet Connection
/// </summary>
public class LibreryDevelopment
{
    // Thread Managment
    Thread receiveThread;
    Thread sendThread;
    byte[] data;
    byte[] sendData;

    // .Net variables for conection
    int puerto;
    UdpClient client;
    IPEndPoint anyIP;
    byte[] address = new byte[4];

    // Control Variable
    volatile bool reciving = true; // reciving thread is active
    volatile bool sending = true; // sending thread is active
    volatile bool send = false; // control varible for sending the new img
    volatile bool conectedd = false; // control for when the active conection has started

    // Image texture allocator
    byte[] byteImg = new byte[200]; 


    public void setTexture2D(ref byte[] arrayImg)
    {
        byteImg = arrayImg;
        send = true;
    }
    /// <summary>
    /// Stops the thread that recive the input from the phone
    /// WARNING : Stoping this thread will stop all the other threads that are already working
    /// </summary>
    public void StopReciving()
    {
        reciving = false;
    }

    /// <summary>
    /// Stops sending the Image to the phone
    /// </summary>
    public void StopSending()
    {
        sending = false;
    }
    /// <summary>
    /// Active to send the new image to the phone
    /// </summary>
    public void Send()
    {
        send = true;
    }

    /// <summary>
    /// Checks if the sending thread is active
    /// </summary>
    /// <returns>Return if the thread is active</returns>
    public bool checkSending()
    {
        return sending;
    }
    /// <summary>
    /// Checks if the connection has started
    /// </summary>
    /// <returns>Return the state of the conection</returns>
    public bool conected()
    {
        return conectedd;
    }

    /// <summary>
    /// Initialice the server threads and variable.
    /// Use before everything else to avoid crashes.
    /// </summary>
    /// <param name="port">Port number for the conection</param>
    /// <param name="address">Addres of the local machine. The [0] is the first left number of the ip until the [4] that is the right number of the ip</param>

    public void init(int port, byte[] address)
    {
        this.address = address;
        puerto = port;
        receiveThread = new Thread(
            new ThreadStart(ReceiveData));
        receiveThread.IsBackground = true;
        sendThread = new Thread(
            new ThreadStart(SendData));
        sendThread.IsBackground = true;
    }

    /// <summary>
    /// Starts the main thread that recives the input from the phone
    /// </summary>
    /// <returns>Returns if the start has been succesfully started</returns>
    public bool StartReciving()
    {
        if (receiveThread != null) {
            reciving = true;
            receiveThread.Start();
            return true;
        }
        return false;
    }

    /// <summary>
    /// Starts the thread that sends the image to the phone
    /// WARNING: Requires the Main Thread(Reciving Thread) to be active
    /// </summary>
    /// <returns>Returns if the start has been succesfully started</returns>
    public bool StartSending()
    {
        if (sendThread != null)
        {
            sending = true;
            sendThread.Start();
            return true;
        }
        return false;
    }

    /// <summary>
    /// Starts both threads(Reciving Thread and Sending Thread).
    /// </summary>
    /// <returns>Returns if both starts have been succesfully started</returns>
    public bool StartRecivingAndSending()
    {
        if (receiveThread != null && sendThread != null)
        {
            sending = reciving = true;
            receiveThread.Start();
            sendThread.Start();
            return true;
        }
        return false;
    }

    /// <summary>
    /// Method that sending thread does
    /// </summary>
    private void SendData()
    {

        System.Net.Sockets.UdpClient cliente = new System.Net.Sockets.UdpClient();

        sendData = new byte[10];

        while (!conectedd);// wait until they conect is active
        cliente.Connect(anyIP.Address, puerto);

        while (sending)
        {
            if (send)
            {
                send = false;
                cliente.Send(byteImg, byteImg.Length); //este mensaje tiene de latencia 5ms aprox cuando hace ping
            }

        }
        //fin de comunicación
        sendData[0] = 1;
        cliente.Send(sendData, sendData.Length);
    }


    /// <summary>
    /// Method that main thread does
    /// </summary>
    private void ReceiveData()
    {

        client = new UdpClient(puerto);
        IPAddress a = new IPAddress(address);
        anyIP = new IPEndPoint(a, puerto);


        while (reciving)
        {

            try
            {
                //TODO: Desbloquear este receive o algo para no bloquear la aplicacion en el caso de que queramos salir y no se conecte nadie.
                data = client.Receive(ref anyIP); //bloqueante

                conectedd = true;//activamos mandar img
                if (data[0] == 2)
                {
                    reciving = false;
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
                break;
            }
        }
    }
}
