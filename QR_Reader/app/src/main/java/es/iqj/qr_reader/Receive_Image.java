package es.iqj.qr_reader;

import java.io.IOException;
import java.net.DatagramPacket;
import java.net.DatagramSocket;
import java.net.InetAddress;
import java.net.ServerSocket;
import java.net.Socket;
import java.net.SocketException;

public class Receive_Image extends Thread {

    DatagramSocket socket;
    InetAddress address;
    byte num;
    DatagramPacket packet;
    String dstAddress;
    int port;
    private boolean running;

    public Receive_Image(String addr, int port){
        dstAddress = addr;
        this.port = port;
    }

    public void setRunning(boolean running){
        this.running = running;
    }
    @Override
    public void run(){
        running = true;
        DatagramSocket serversocket=null;
        try {
             serversocket = new DatagramSocket(port);
             System.out.println("Holiwi__" + serversocket.getLocalSocketAddress() + "_____" + serversocket.getLocalPort());
             System.out.println(port);
        }
        catch (IOException e)
        {
            e.printStackTrace();
        }
        while(running) {
            try {
                byte[] message = new byte[10];
                DatagramPacket receivePacket = new DatagramPacket(message, message.length);
                serversocket.receive(receivePacket);

                System.out.println("VIVA EL VINO");
                if(message[0] == 5)
                    System.out.println("Bien");
                else if(message[0] == 7)
                    System.out.println("Mal");
                else if(message[0] == 1) {
                    System.out.println("Terminando");

                    running = false;
                }


            } catch (IOException e) {
                e.printStackTrace();
            }
        }
    }

}
