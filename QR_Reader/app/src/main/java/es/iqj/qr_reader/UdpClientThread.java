package es.iqj.qr_reader;
import android.os.Message;

import java.io.IOException;
import java.net.DatagramPacket;
import java.net.DatagramSocket;
import java.net.InetAddress;
import java.net.SocketException;
import java.net.UnknownHostException;
import static java.lang.System.out;
public class UdpClientThread extends Thread{

    //to send to server.
    byte _up;
    byte _down;
    byte _left;
    byte _right;
    byte _A;
    byte _B;
    byte _START;
    byte _SELECT;


    boolean ready = false;

    String dstAddress;
    int dstPort;
    private boolean running;
    Controller.UdpClientHandler handler;

    DatagramSocket socket;
    InetAddress address;

    public UdpClientThread(String addr, int port, Controller.UdpClientHandler handler ,
                           byte up,
                           byte down,
                           byte left,
                           byte right,
                           byte A,
                           byte B,
                           byte START,
                           byte SELECT) {
        super();
        dstAddress = addr;
        dstPort = port;
        this.handler = handler;

        _up = up;
        _down =down;
        _left = left;
        _right = right;
        _A = A;
        _B = B;
        _START = START;
        _SELECT = SELECT;


    }

    public void setRunning(boolean running){
        this.running = running;
    }

    private void sendState(String state){
        handler.sendMessage(
                Message.obtain(handler,
                        Controller.UdpClientHandler.UPDATE_STATE, state));
    }


    public void clicked(String addr, int port,
                        byte up,
                        byte down,
                        byte left,
                        byte right,
                        byte A,
                        byte B,
                        byte START,
                        byte SELECT){

        dstAddress = addr;
        dstPort = port;

        _up = up;
        _down =down;
        _left = left;
        _right = right;
        _A = A;
        _B = B;
        _START = START;
        _SELECT = SELECT;
        ready = true;

    }


    @Override
    public void run() {


        running = true;
        /*try {
            socket = new DatagramSocket();
        }
        catch (SocketException e){
            e.printStackTrace();
        }*/
        while(running) {
            if(ready) {
                try {
                    ready = false;
                    socket = new DatagramSocket();
                    address = InetAddress.getByName(dstAddress);

                    // send request
                    byte[] buf = new byte[256];

                    buf[0] = _up;
                    buf[1] = _down;
                    buf[2] = _left;
                    buf[3] = _right;


                    buf[4] = _A;
                    buf[5] = _B;
                    buf[6] = _START;
                    buf[7] = _SELECT;
                    DatagramPacket packet =
                            new DatagramPacket(buf, buf.length, address, dstPort);
                    socket.send(packet);



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
                }
            }
        }

        byte[] buf = new byte[8];

        buf[0] = 2;

        DatagramPacket packet =
                new DatagramPacket(buf, buf.length, address, dstPort);

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
