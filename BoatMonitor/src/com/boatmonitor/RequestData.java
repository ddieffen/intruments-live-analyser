package com.boatmonitor;

import com.example.boatmonitor.R;

import android.annotation.SuppressLint;
import android.app.Activity;
import android.content.Intent;
import android.os.Bundle;
import android.support.v4.app.NavUtils;
import android.view.Menu;
import android.view.MenuItem;
import android.view.View;
import android.widget.TextView;

public class RequestData extends Activity {
    private String serverIpAddress = ""; 
    private TextView tw;
 
	@SuppressLint("NewApi")
	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		setContentView(R.layout.activity_request_data);
		Intent intent = getIntent();
		String sip = intent.getStringExtra(MainActivity.IP_ADDRESS);
		//tw = (TextView)findViewById(R.id.main_view);
		//tw.setText(sip);
		// Show the Up button in the action bar.
		setupActionBar();
		serverIpAddress = sip;		
        if (!serverIpAddress.equals("")) {
        	ClientThread ct = new ClientThread();
        	ct.connected = false;
        	ct.serverIpAddress = sip;
            Thread cThread = new Thread(ct);
            cThread.start();
        }
        
	}

	/**
	 * Set up the {@link android.app.ActionBar}.
	 */
	private void setupActionBar() {

		getActionBar().setDisplayHomeAsUpEnabled(true);

	}

	@Override
	public boolean onCreateOptionsMenu(Menu menu) {
		// Inflate the menu; this adds items to the action bar if it is present.
		getMenuInflater().inflate(R.menu.request_data, menu);
		return true;
	}

	@Override
	public boolean onOptionsItemSelected(MenuItem item) {
		switch (item.getItemId()) {
		case android.R.id.home:
			// This ID represents the Home or Up button. In the case of this
			// activity, the Up button is shown. Use NavUtils to allow users
			// to navigate up one level in the application structure. For
			// more details, see the Navigation pattern on Android Design:
			//
			// http://developer.android.com/design/patterns/navigation.html#up-vs-back
			//
			NavUtils.navigateUpFromSameTask(this);
			return true;
		}
		return super.onOptionsItemSelected(item);
	}
	public void lineGraphHandler (View view){
    	LineGraph line = new LineGraph();
    	Intent lineIntent = line.getIntent(this);
        startActivity(lineIntent);		
	}
	public void pieGraphHandler (View view){}
	public void scatterGraphHandler (View view){}
	public void barGraphHandler (View view){}

}

