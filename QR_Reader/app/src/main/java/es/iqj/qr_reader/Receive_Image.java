package es.iqj.qr_reader;

import java.io.IOException;
import java.net.DatagramPacket;
import java.net.DatagramSocket;
import java.net.InetAddress;
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
