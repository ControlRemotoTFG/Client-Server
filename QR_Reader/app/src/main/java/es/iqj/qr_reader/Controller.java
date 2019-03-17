package es.iqj.qr_reader;

import android.app.Activity;
import android.content.Intent;
import android.os.Bundle;


public class Controller extends Activity {

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);

        // Set the layout for the layout we created
        setContentView(R.layout.second_layout);

        // Get the Intent that called for this Activity to open

        Intent activityThatCalled = getIntent();

        // Get the data that was sent

        String port_ip = activityThatCalled.getExtras().getString("token");

    }

}
