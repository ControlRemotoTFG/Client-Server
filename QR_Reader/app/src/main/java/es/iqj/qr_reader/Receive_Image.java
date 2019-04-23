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
import com.google.android.gms.common.images.internal.ImageUtils;
import java.io.IOException;
import java.net.DatagramPacket;
import java.net.DatagramSocket;
import java.net.InetAddress;
import java.net.ServerSocket;
import java.net.Socket;
import java.net.SocketException;


public class Receive_Image extends Thread{

    //Reguladores de la conexión
    String dstAddress;
    int port;

    private boolean running;//boolean de control del thread
    Controller control;//recibimos la activity


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
                //el tamaño maximo que podemos recibir
                byte[] message = new byte[1900];//entre 1900-1700
                DatagramPacket receivePacket = new DatagramPacket(message, message.length);
                //recibimos la img del pc
                serversocket.receive(receivePacket);
                //creamos el bitmap desde el byteArray que recibimos
                long statTime = System.nanoTime();
                bitmap = BitmapFactory.decodeByteArray(message,0,message.length);

                //creamos el bitmapDrawable que luego se pasara a la mainThread
                //para poder modificar la img que creamos en el manifest
                final BitmapDrawable bit = new BitmapDrawable(bitmap);
                control.runOnUiThread(new Runnable() {

                    @Override
                    public void run() {
                        control.setByteMap(bit);
                    }
                });
                long endTime = System.nanoTime();
                long timeElapsed = endTime - statTime;
                System.out.println((timeElapsed/1000000) + " ms time descompressing img");
                //TOTAL = 13-15 MS MEDIA
            } catch (IOException e) {
                e.printStackTrace();
            }
        }
    }

}
