package com.boatmonitor;

import java.io.BufferedWriter;
import java.io.IOException;
import java.io.OutputStreamWriter;
import java.io.PrintWriter;
import java.net.Socket;
import java.net.UnknownHostException;

import com.example.boatmonitor.R;

import android.os.Bundle;
import android.app.Activity;
import android.content.Intent;
import android.view.Menu;
import android.view.View;
import android.widget.EditText;

public class MainActivity extends Activity {
	public final static String IP_ADDRESS = "com.example.myfirstapp.IP";

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);
    	EditText t = (EditText)findViewById(R.id.server_ip);
    	t.setText("192.168.1.218");        
    }


    @Override
    public boolean onCreateOptionsMenu(Menu menu) {
        // Inflate the menu; this adds items to the action bar if it is present.
        getMenuInflater().inflate(R.menu.main, menu);
        return true;
    }
    public void startListening(View view)
    {
    	Intent intent = new Intent(this, RequestData.class);
    	EditText t = (EditText)findViewById(R.id.server_ip);
    	t.refreshDrawableState();
    	String sip = t.getText().toString();
    	intent.putExtra(IP_ADDRESS,  sip);
    	startActivity(intent);    	
    	
    }
    
}
