package es.iqj.qr_reader;

import android.app.Activity;
import android.content.Intent;
import android.graphics.drawable.BitmapDrawable;
import android.os.Bundle;
import android.os.Handler;
import android.os.VibrationEffect;
import android.util.DisplayMetrics;
import android.view.MotionEvent;
import android.view.View;
import android.widget.RelativeLayout;
import android.os.Vibrator;




public class Controller extends Activity  {

    UdpClientHandler udpClientHandler;
    UdpClientThread udpClientThread;
    Receive_Image receive_image;
    //variables de botones
    float x =0;
    float y = 0;
    Vibrator v;
    String puerto = "";
    String ip = "";

    Intent activityThatCalled;

    RelativeLayout myLayout = null;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);

        // Set the layout for the layout we created
        setContentView(R.layout.second_layout);

        // Get the Intent that called for this Activity to open


        activityThatCalled = getIntent();

        // Get the data that was sent

        String port_ip = activityThatCalled.getExtras().getString("token");
        v = (Vibrator) getSystemService(this.VIBRATOR_SERVICE);

        char index;
        puerto = "";
        ip = "";
        boolean flag = false;

        int i = 0;
        while(i<port_ip.length()){
            index = port_ip.charAt(i);
            if(index == ':'){
                flag= true;
            }
            else if(flag){
                ip +=String.valueOf(index);
            }
            else  puerto +=String.valueOf(index);

            i++;

        }

        myLayout = findViewById(R.id.fondo);


        myLayout.setOnTouchListener(new View.OnTouchListener(){

            @Override
            public boolean onTouch(View v, MotionEvent event){
                if(event.getAction() == MotionEvent.ACTION_DOWN) {
                    //TODO: get x and y coordenates and calculate wich hotspot has been touched
                    x = event.getX();
                    y = event.getY();

                    if(udpClientThread == null) {
                        DisplayMetrics displayMetrics = new DisplayMetrics();
                        getWindowManager().getDefaultDisplay().getMetrics(displayMetrics);
                        int height = displayMetrics.heightPixels;
                        int width = displayMetrics.widthPixels;
                        udpClientThread = new UdpClientThread(
                                ip,
                                Integer.parseInt(puerto),
                                udpClientHandler,
                                (int)x,
                                (int)y,
                                width,
                                height);
                        udpClientThread.start();
                    }
                    else
                        udpClientThread.clicked((int)x,(int)y);
                }

                //LEVANTAR

                if(event.getAction() == MotionEvent.ACTION_UP) {

                    if(udpClientThread == null) {
                        DisplayMetrics displayMetrics = new DisplayMetrics();
                        getWindowManager().getDefaultDisplay().getMetrics(displayMetrics);
                        int height = displayMetrics.heightPixels;
                        int width = displayMetrics.widthPixels;
                        udpClientThread = new UdpClientThread(
                                ip,
                                Integer.parseInt(puerto),
                                udpClientHandler,
                                (int)x,
                                (int)y,
                                width,
                                height);
                        udpClientThread.start();
                    }
                    else
                        udpClientThread.clicked((int)x,(int)y);

                }
                return true;
            }
        });


        receive_image = new Receive_Image(ip,Integer.parseInt(puerto),this);
        receive_image.start();

        udpClientHandler = new UdpClientHandler(this);
    }


    public void onPause(){
        super.onPause();
        udpClientThread.setRunning(false);
        receive_image.setRunning(false);
        try {
            udpClientThread.join();
            receive_image.join();
        }
        catch (java.lang.InterruptedException a){

        }
        System.out.print("He terminado");
    }

    //cambiamos la img de id="fondo" en este metodo
    public void setByteMap(BitmapDrawable bit){
        myLayout.setBackground(bit);
    }

    public void VibrateTimer(int miliseconds){
        //deprecated in API 26
        v.vibrate(miliseconds);
    }



    public static class UdpClientHandler extends Handler {
        public static final int UPDATE_STATE = 0;
        public static final int UPDATE_MSG = 1;
        public static final int UPDATE_END = 2;
        private Controller parent;

        public UdpClientHandler(Controller parent) {
            super();
            this.parent = parent;
        }
    }

}
