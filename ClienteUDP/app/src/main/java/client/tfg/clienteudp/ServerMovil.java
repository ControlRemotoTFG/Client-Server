package client.tfg.clienteudp;

import java.io.IOException;
import java.net.DatagramPacket;
import java.net.DatagramSocket;
import java.net.InetAddress;
import java.net.SocketException;
import java.net.UnknownHostException;

public class ServerMovil extends Thread {

    byte num;
    DatagramPacket packet;
    String dstAddress;
    int port = 2004;
    private boolean running;
    MainActivity.UdpClientHandler handler;

    DatagramSocket socket;
    InetAddress address;
    public void setRunning(boolean running){
        this.running = running;
    }
    @Override
    public void run(){
        running = true;
        byte[] message = new byte[50];

        try {
            packet = new DatagramPacket(message, message.length);
            socket = new DatagramSocket(port);
        }catch (SocketException e) {
            e.printStackTrace();
        }

        while(running) {
            try {

            socket.receive(packet);
            
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
