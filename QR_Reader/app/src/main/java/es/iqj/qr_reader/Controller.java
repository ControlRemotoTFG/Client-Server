package es.iqj.qr_reader;

import android.app.Activity;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.os.Bundle;
import android.os.Handler;
import android.os.Message;
import android.view.MotionEvent;
import android.view.View;
import android.widget.Button;
import android.widget.EditText;
import android.widget.RelativeLayout;
import android.widget.TextView;




public class Controller extends Activity  {





    UdpClientHandler udpClientHandler;
    UdpClientThread udpClientThread;
    Receive_Image receive_image;
    //variables de botones
    float x =0;
    float y = 0;
    byte _up =0;
    byte _down =0;
    byte _left =0;
    byte _right =0;
    byte _A =0;
    byte _B =0;
    byte _START =0;
    byte _SELECT =0;

    String puerto = "";
    String ip = "";


    //rectangulos de botones
    Rect up = new Rect(201,52,171,114);
    Rect down = new Rect(201,288,171,114);
    Rect left = new Rect(55,175,171,114);
    Rect right = new Rect(363,166,171,114);

    Rect A = new Rect(480,348,358,260);
    Rect B = new Rect(611,689,347,239);
    Rect select = new Rect(13,818,446,74);
    Rect start = new Rect(40,626,401,93);

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





        myLayout = findViewById(R.id.myLayout);


        myLayout.setOnTouchListener(new View.OnTouchListener(){

            @Override
            public boolean onTouch(View v, MotionEvent event){
                if(event.getAction() == MotionEvent.ACTION_DOWN) {

                    //TODO: get x and y coordenates and calculate wich hotspot has been touched
                    x = event.getX();
                    y = event.getY();
                    System.out.println(x);
                    System.out.println(y);

                    //PULSAR

                    if(up.pulsado((int)x,(int)y))
                        _up = 1;
                    if(down.pulsado((int)x,(int)y))
                        _down = 1;
                    if(left.pulsado((int)x,(int)y))
                        _left = 1;
                    if(right.pulsado((int)x,(int)y))
                        _right = 1;
                    if(A.pulsado((int)x,(int)y))
                        _A = 1;
                    if(B.pulsado((int)x,(int)y))
                        _B = 1;
                    if(start.pulsado((int)x,(int)y))
                        _START = 1;
                    if(select.pulsado((int)x,(int)y))
                        _SELECT= 1;



                    if(udpClientThread == null) {
                        udpClientThread = new UdpClientThread(
                                ip,
                                Integer.parseInt(puerto),
                                udpClientHandler,
                                _up,
                                _down,
                                _left,
                                _right,
                                _A,
                                _B,
                                _START,
                                _SELECT);
                        udpClientThread.start();
                    }
                    else
                        udpClientThread.clicked(
                                ip,
                                Integer.parseInt(puerto),
                                _up,
                                _down,
                                _left,
                                _right,
                                _A,
                                _B,
                                _START,
                                _SELECT);
                }

                //LEVANTAR

                if(event.getAction() == MotionEvent.ACTION_UP) {


                    _up = 0;
                    _down = 0;
                    _left = 0;
                    _right = 0;
                    _A = 0;
                    _B = 0;
                    _START = 0;
                    _SELECT = 0;



                    if(udpClientThread == null) {
                        udpClientThread = new UdpClientThread(
                               ip,
                                Integer.parseInt(puerto),
                                udpClientHandler,
                                _up,
                                _down,
                                _left,
                                _right,
                                _A,
                                _B,
                                _START,
                                _SELECT);
                        udpClientThread.start();
                    }
                    else
                        udpClientThread.clicked(
                                ip,
                                Integer.parseInt(puerto),
                                _up,
                                _down,
                                _left,
                                _right,
                                _A,
                                _B,
                                _START,
                                _SELECT);

                }
                return true;
            }
        });


        receive_image = new Receive_Image(ip,Integer.parseInt(puerto));
        receive_image.start();

        udpClientHandler = new UdpClientHandler(this);
    }


    public void onDestroy(){
        super.onDestroy();
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




    // class Rect
    // Clase para los botones que vamos a pulsar, detecta si se ha pulsado dicho Rect.
    public class Rect{
        Rect(int x,int y, int ancho, int alto){
            _x=x;
            _y=y;
            _alto=alto;
            _ancho=ancho;

        }

        boolean pulsado(int x, int y){

            return x>=_x && x<=(_ancho + _x) && y>=_y && y<=(_alto + _y);
        }

        int _x;
        int _y;
        int _alto;
        int _ancho;
    }



}
