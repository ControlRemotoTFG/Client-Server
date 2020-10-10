package es.iqj.qr_reader;


import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.graphics.drawable.BitmapDrawable;
import java.io.IOException;
import java.net.DatagramPacket;
import java.net.DatagramSocket;




public class Receive_Image extends Thread{

    //Reguladores de la conexión
    String dstAddress;
    int port;
    private boolean running;//boolean de control del thread
    Controller control;//recibimos la activity
    private int [] timePerImage;

    public Receive_Image(String addr, int port,Controller control ){
        dstAddress = addr;
        timePerImage = new int[31];
        for(int i = 0; i < timePerImage.length; i++)
            timePerImage[i] = 0;
        this.port = port;
        this.control = control;
    }

    public int[] getTimePerImage() {
        return timePerImage;
    }

    public void setRunning(boolean running){
        this.running = running;
    }
    @Override
    public void run(){
        running = true;
        DatagramSocket serversocket=null;
        int vibrateTime = 0;
        try {
             serversocket = new DatagramSocket(port);
             System.out.println("Holiwi__" + serversocket.getLocalSocketAddress() + "_____" + serversocket.getLocalPort());
             System.out.println(port);
        }
        catch (IOException e)
        {
            e.printStackTrace();
        }
        // RECIVIMOS EL TIMEPO DE VIBRACION DESDE EL SERVER DEL PC
        byte[] vibrationTimerMessage = new byte[4];//entre 1900-1700
        DatagramPacket receiveTime = new DatagramPacket(vibrationTimerMessage, vibrationTimerMessage.length);
        try {
            serversocket.receive(receiveTime);
        } catch (IOException e) {
            e.printStackTrace();
        }
        // DESCOMPONEMOS EN INT EL TIEMPO DADO POR EL PC Y LO ASIGNAMOS A VIBRATIONTIME
        int a = vibrationTimerMessage[0];
        int b = (vibrationTimerMessage[1] << 4);
        int c = (vibrationTimerMessage[2] << 8);
        int d = (vibrationTimerMessage[3] << 16);
        vibrateTime = a + b + c + d;


        while(running) {
            try {
                Bitmap bitmap;
                //el tamaño maximo que podemos recibir
                byte[] message = new byte[64000];//entre 1900-1700
                DatagramPacket receivePacket = new DatagramPacket(message, message.length);
                //recibimos la img del pc
                serversocket.receive(receivePacket);
                if(message[0] == 0 && message[1] == 0 && message[2] == 0 && message[3] == 0){//tenemos q activar vibracion pq hemos pulsado bien
                    control.VibrateTimer(vibrateTime);
                }
                else { // es imagen
                    //creamos el bitmap desde el byteArray que recibimos
                    long statTime = System.nanoTime();
                    bitmap = BitmapFactory.decodeByteArray(message, 0, message.length);
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
                    if((int)(timeElapsed / 1000000) < timePerImage.length && timeElapsed >= 0)
                        timePerImage[(int)(timeElapsed / 1000000)] += 1;
                    //System.out.println((timeElapsed / 1000000) + " ms time descompressing img");
                    //TOTAL = 13-15 MS MEDIA
                }
            } catch (IOException e) {
                e.printStackTrace();
            }
        }
    }

}
