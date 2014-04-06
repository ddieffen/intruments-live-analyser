package com.boatmonitor;

import java.io.BufferedWriter;
import java.io.OutputStreamWriter;
import java.io.PrintWriter;
import java.net.InetAddress;
import java.net.Socket;
import android.util.Log;

public class ClientThread implements Runnable {
	//this class is based on 2 examples:
	//http://thinkandroid.wordpress.com/2010/03/27/incorporating-socket-programming-into-your-applications/
	//http://stackoverflow.com/questions/13649346/tcp-ip-communication-using-android-as-client-and-c-sharp-as-server
	public String serverIpAddress;
	public Boolean connected;
	@Override
	public void run() {
        try {
            InetAddress serverAddr = InetAddress.getByName(serverIpAddress);
            Log.d("ClientActivity", "C: Connecting...");
            Socket socket = new Socket(serverIpAddress, 11000);
            connected = true;
            while (connected) {
                try {
                    Log.d("ClientActivity", "C: Sending command.");
                    PrintWriter out = new PrintWriter(new BufferedWriter(new OutputStreamWriter(socket
                                .getOutputStream())), true);
                        // WHERE YOU ISSUE THE COMMANDS
                        out.println("Hey Server, Send me the Boat Data!<EOF>");
                        Log.d("ClientActivity", "C: Sent.");
                } catch (Exception e) {
                    Log.e("ClientActivity", "S: Error", e);
                }
            }
            socket.close();
            Log.d("ClientActivity", "C: Closed.");
        } catch (Exception e) {
            Log.e("ClientActivity", "C: Error", e);
            connected = false;
        }		

	}

}
