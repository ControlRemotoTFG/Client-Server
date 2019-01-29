package client.tfg.clienteudp;

import android.os.Handler;
import android.os.Message;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.view.MotionEvent;
import android.view.View;
import android.widget.Button;
import android.widget.EditText;
import android.widget.RelativeLayout;
import android.widget.TextView;



public class MainActivity extends AppCompatActivity {

    EditText editTextAddress, editTextPort;
    Button buttonConnect;
    TextView textViewState, textViewRx;

    UdpClientHandler udpClientHandler;
    UdpClientThread udpClientThread;

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

    //rectangulos de botones
    Rect up = new Rect(201,52,171,114);
    Rect down = new Rect(201,288,171,114);
    Rect left = new Rect(55,175,171,114);
    Rect right = new Rect(363,166,171,114);

    Rect A = new Rect(480,348,358,260);
    Rect B = new Rect(611,689,347,239);
    Rect select = new Rect(13,818,446,74);
    Rect start = new Rect(40,626,401,93);


    RelativeLayout myLayout = null;


    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);
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

                     _up =0;
                     _down =0;
                     _left =0;
                     _right =0;
                     _A =0;
                     _B =0;
                     _START =0;
                     _SELECT =0;

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
                                editTextAddress.getText().toString(),
                                Integer.parseInt(editTextPort.getText().toString()),
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
                                editTextAddress.getText().toString(),
                                Integer.parseInt(editTextPort.getText().toString()),
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
                                editTextAddress.getText().toString(),
                                Integer.parseInt(editTextPort.getText().toString()),
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
                                editTextAddress.getText().toString(),
                                Integer.parseInt(editTextPort.getText().toString()),
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

        editTextAddress = (EditText) findViewById(R.id.address);
        editTextPort = (EditText) findViewById(R.id.port);
        buttonConnect = (Button) findViewById(R.id.connect);
        textViewState = (TextView)findViewById(R.id.state);
        textViewRx = (TextView)findViewById(R.id.received);

        buttonConnect.setOnClickListener(buttonConnectOnClickListener);

        udpClientHandler = new UdpClientHandler(this);
    }

    View.OnClickListener buttonConnectOnClickListener =
            new View.OnClickListener() {

                @Override
                public void onClick(View arg0) {

                    udpClientThread = new UdpClientThread(
                            editTextAddress.getText().toString(),
                            Integer.parseInt(editTextPort.getText().toString()),
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


                    buttonConnect.setEnabled(false);
                }
            };

    private void updateState(String state){
        textViewState.setText(state);
    }

    private void updateRxMsg(String rxmsg){
        textViewRx.append(rxmsg + "\n");
    }

    private void clientEnd(){
        udpClientThread = null;
        textViewState.setText("clientEnd()");
        buttonConnect.setEnabled(true);

    }

    public static class UdpClientHandler extends Handler {
        public static final int UPDATE_STATE = 0;
        public static final int UPDATE_MSG = 1;
        public static final int UPDATE_END = 2;
        private MainActivity parent;

        public UdpClientHandler(MainActivity parent) {
            super();
            this.parent = parent;
        }

        @Override
        public void handleMessage(Message msg) {

            switch (msg.what){
                case UPDATE_STATE:
                    parent.updateState((String)msg.obj);
                    break;
                case UPDATE_MSG:
                    parent.updateRxMsg((String)msg.obj);
                    break;
                case UPDATE_END:
                    parent.clientEnd();
                    break;
                default:
                    super.handleMessage(msg);
            }

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


