package es.iqj.qr_reader;

import android.app.Activity;
import android.app.Application;
import android.content.Context;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.graphics.drawable.BitmapDrawable;
import android.media.Image;
import android.view.View;
import android.widget.ImageView;
import android.widget.RelativeLayout;
import java.io.IOException;
import java.net.DatagramPacket;
import java.net.DatagramSocket;
import java.net.InetAddress;
import java.net.ServerSocket;
import java.net.Socket;
import java.net.SocketException;

public class Receive_Image extends Thread{
    Context cont;
    DatagramSocket socket;
    InetAddress address;
    byte num;
    DatagramPacket packet;
    String dstAddress;
    int port;
    private boolean running;
    Controller control;
    BitmapDrawable bit = null;
    public Receive_Image(String addr, int port,Controller control ){
        dstAddress = addr;
        this.port = port;
        this.control = control;

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
                Bitmap bitmap;
                byte[] message = new byte[32768];
                DatagramPacket receivePacket = new DatagramPacket(message, message.length);
                serversocket.receive(receivePacket);

                bitmap = BitmapFactory.decodeByteArray(message,0,message.length);
                final BitmapDrawable bit = new BitmapDrawable(bitmap);

                //control.setByteMap(bit);//ESTO DA FALLO YA QUE SIGUES ESTANDO EN ESTE THREAD CUANDO TIENES QUE HACERLO EN EL MAIN THREAD
                control.runOnUiThread(new Runnable() {

                    @Override
                    public void run() {
                        control.setByteMap(bit);
                    }
                });


                System.out.println("VIVA EL VINO");

            } catch (IOException e) {
                e.printStackTrace();
            }
        }
    }

}
