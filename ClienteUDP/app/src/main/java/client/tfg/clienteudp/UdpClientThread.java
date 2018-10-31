package client.tfg.clienteudp;
import android.os.Message;

import java.io.IOException;
import java.net.DatagramPacket;
import java.net.DatagramSocket;
import java.net.InetAddress;
import java.net.SocketException;
import java.net.UnknownHostException;
import static java.lang.System.out;
public class UdpClientThread extends Thread{

    //x, y: ints to send to server.
    int _x;
    int _y;


    String dstAddress;
    int dstPort;
    private boolean running;
    MainActivity.UdpClientHandler handler;

    DatagramSocket socket;
    InetAddress address;

    public UdpClientThread(String addr, int port, MainActivity.UdpClientHandler handler , int x,int y) {
        super();
        dstAddress = addr;
        dstPort = port;
        this.handler = handler;


        _x=x;
        _y=y;

    }

    public void setRunning(boolean running){
        this.running = running;
    }

    private void sendState(String state){
        handler.sendMessage(
                Message.obtain(handler,
                        MainActivity.UdpClientHandler.UPDATE_STATE, state));
    }

    @Override
    public void run() {
        sendState("connecting...");

        running = true;

        try {
            socket = new DatagramSocket();
            address = InetAddress.getByName(dstAddress);

            // send request
            byte[] buf = new byte[256];

            //coding 2 ints in big endian
            buf [0] = (byte)((_x >> 24) & 0xff);
            buf [1] = (byte)((_x >> 16) & 0xff);
            buf [2] = (byte)((_x >> 8) & 0xff);
            buf [3] = (byte)((_x>> 0) & 0xff);


            buf [4] = (byte)((_y >> 24) & 0xff);
            buf [5] = (byte)((_y  >> 16) & 0xff);
            buf [6] = (byte)((_y >> 8) & 0xff);
            buf [7] = (byte)((_y>> 0) & 0xff);

            DatagramPacket packet =
                    new DatagramPacket(buf, buf.length, address, dstPort);
            socket.send(packet);

            sendState("connected");

            //client wait to response for the message

            /*// get response
            packet = new DatagramPacket(buf, buf.length);


            socket.receive(packet);
            String line = new String(packet.getData(), 0, packet.getLength());

            handler.sendMessage(
                    Message.obtain(handler, MainActivity.UdpClientHandler.UPDATE_MSG, line));
            */
        } catch (SocketException e) {
            e.printStackTrace();
        } catch (UnknownHostException e) {
            e.printStackTrace();
        } catch (IOException e) {
            e.printStackTrace();
        } finally {
            if(socket != null){
                socket.close();
                handler.sendEmptyMessage(MainActivity.UdpClientHandler.UPDATE_END);
            }
        }

    }
}

