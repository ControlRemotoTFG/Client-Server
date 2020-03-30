package es.iqj.qr_reader;

import java.io.IOException;
import java.net.DatagramPacket;
import java.net.DatagramSocket;
import java.net.InetAddress;
import java.net.SocketException;
import java.net.UnknownHostException;

public class UdpClientThread extends Thread{

    boolean ready = false;

    String dstAddress;
    int dstPort;
    private boolean running;
    Controller.UdpClientHandler handler;

    DatagramSocket socket;
    InetAddress address;
    int xPos;
    int yPos;
    int width;
    int height;
    public UdpClientThread(String addr, int port, Controller.UdpClientHandler handler ,int x, int y, int widthScreen, int heightScreen){
        super();
        dstAddress = addr;
        dstPort = port;
        this.handler = handler;
        xPos = x;
        yPos = y;
        width = widthScreen;
        height = heightScreen;
    }

    public void setRunning(boolean running){
        this.running = running;
    }

    public void clicked(int x, int y){
        xPos = x;
        yPos = y;
        ready = true;
    }


    @Override
    public void run() {
        try {
            socket = new DatagramSocket();
        } catch (SocketException e) {
            e.printStackTrace();
        }
        byte[] buf = new byte[8];
        running = true;
        try {
            address = InetAddress.getByName(dstAddress);
        } catch (UnknownHostException e) {
            e.printStackTrace();
        }
        buf[0] = (byte)(width & (0x000000FF));
        buf[1] = (byte)((width & (0x0000FF00)) >> 4);
        buf[2] = (byte)((width & (0x00FF0000)) >> 8);
        buf[3] = (byte)((width & (0xFF000000)) >> 16);

        buf[4] = (byte)(height & (0x000F));
        buf[5] = (byte)((height & (0x00F0)) >> 4);
        buf[6] = (byte)((height & (0x0F00)) >> 8);
        buf[7] = (byte)((height & (0xF000)) >> 16);

        DatagramPacket packet =
                new DatagramPacket(buf, buf.length, address, dstPort);

        try {
            socket.send(packet);
        } catch (IOException e) {
            e.printStackTrace();
        }
        while(running) {
            if(ready) {
                try {
                    ready = false;

                    address = InetAddress.getByName(dstAddress);
                    System.out.println(address);
                    // send request

                    buf[0] = (byte)(xPos & (0x000000FF));
                    buf[1] = (byte)((xPos & (0x0000FF00)) >> 4);
                    buf[2] = (byte)((xPos & (0x00FF0000)) >> 8);
                    buf[3] = (byte)((xPos & (0xFF000000)) >> 16);

                    buf[4] = (byte)(yPos & (0x000F));
                    buf[5] = (byte)((yPos & (0x00F0)) >> 4);
                    buf[6] = (byte)((yPos & (0x0F00)) >> 8);
                    buf[7] = (byte)((yPos & (0xF000)) >> 16);


                    System.out.println(yPos + "  "+ buf[4] + "  " + buf[5]+ "  " + buf[6]+ "  " + buf[7]);

                    socket.send(packet);

                } catch (SocketException e) {
                    e.printStackTrace();
                } catch (UnknownHostException e) {
                    e.printStackTrace();
                } catch (IOException e) {
                    e.printStackTrace();
                }
            }
        }

        byte[] buff = new byte[1];

        buff[0] = 2;

        packet =
                new DatagramPacket(buff, buff.length, address, dstPort);

        try {
            socket.send(packet);
        }
        catch (IOException e){
            e.printStackTrace();
        }
        System.out.print("Mensaje mandado");
        socket.close();
        handler.sendEmptyMessage(Controller.UdpClientHandler.UPDATE_END);
    }
}
