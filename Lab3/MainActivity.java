package com.example.lab3j;

import android.Manifest;
import android.app.Activity;
import android.content.Context;
import android.content.pm.PackageManager;
import android.location.Location;
import android.net.wifi.WifiInfo;
import android.net.wifi.WifiManager;
import android.os.AsyncTask;
import android.os.Build;
import android.os.Bundle;
import android.util.Log;
import android.widget.TextView;
import android.location.LocationManager;
import android.location.LocationListener;

import androidx.annotation.NonNull;
import androidx.annotation.RequiresApi;
import androidx.core.app.ActivityCompat;

import com.example.lab3j.databinding.ActivityMainBinding;

public class MainActivity extends Activity {

    private TextView mTextView, wifiTextView;
    private ActivityMainBinding binding;

    private LocationManager M;
    private LocationListener L;

    @RequiresApi(api = Build.VERSION_CODES.R)
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);

        binding = ActivityMainBinding.inflate(getLayoutInflater());
        setContentView(binding.getRoot());


        mTextView = binding.text;

        WorkThread thread = new WorkThread(this, binding.wifi);
        thread.execute();

        if (hasGps()) {
            mTextView.setText("gps");
            M = (LocationManager) this.getSystemService(Context.LOCATION_SERVICE);
            L = new LocationListener() {
                @Override
                public void onLocationChanged(@NonNull Location location) {
                    Log.e("GPS", "GPS Changed");
                    Log.e("GPS", location.toString());
                    mTextView.setText("Lat: "+
                            Double.valueOf(location.getLatitude()).toString() +
                            "\nLong: " +
                            Double.valueOf(location.getLongitude()).toString() +
                            "\nAlt: "+
                            Double.valueOf(location.getAltitude()).toString()
                    );

                }
            };

            if (ActivityCompat.checkSelfPermission(this, Manifest.permission.ACCESS_FINE_LOCATION) != PackageManager.PERMISSION_GRANTED && ActivityCompat.checkSelfPermission(this, Manifest.permission.ACCESS_COARSE_LOCATION) != PackageManager.PERMISSION_GRANTED) {
                // TODO: Consider calling
                //    ActivityCompat#requestPermissions
                // here to request the missing permissions, and then overriding
                //   public void onRequestPermissionsResult(int requestCode, String[] permissions,
                //                                          int[] grantResults)
                // to handle the case where the user grants the permission. See the documentation
                // for ActivityCompat#requestPermissions for more details.
                mTextView.setText("AHHHH");
                return;
            } else {
                M.requestLocationUpdates(LocationManager.GPS_PROVIDER, 1000, 1, L);
                Log.e("GPS", "Good");
            }

        } else {
            mTextView.setText("No gps");
        }
    }

    private boolean hasGps() {
        return getPackageManager().hasSystemFeature(PackageManager.FEATURE_LOCATION_GPS);
    }
}


class WorkThread extends AsyncTask<Integer, Double, Boolean> {
    Activity mAct;
    TextView wifi;
    WifiManager wifiManager;

    public WorkThread(Activity a, TextView w) {
        super();
        mAct = a;
        wifi = w;

        int counter = 0;

        wifiManager = (WifiManager) a.getSystemService(Context.WIFI_SERVICE);
        WifiInfo wifiInfo = wifiManager.getConnectionInfo();
        final int wifiLevel = wifiManager.calculateSignalLevel(wifiInfo.getRssi(), 100) + 1;

        w.setText("Wifi strength= " + Integer.toString(wifiLevel) + "/100\n" + Integer.toString(wifiInfo.getRssi()) + "dbm");
    }

    @Override
    protected void onProgressUpdate(Double... values) {
        WifiInfo wifiInfo = wifiManager.getConnectionInfo();
        final int rssi = wifiInfo.getRssi();
        final int wifiLevel = wifiManager.calculateSignalLevel(rssi, 100) + 1;

        String text = "Wifi strength= " + Integer.toString(wifiLevel) + "/100\n" + Integer.toString(rssi) + "dbm";
        // Log.e("WIFI", text);
        wifi.invalidate();
        wifi.setText(text);
        wifi.invalidate();
    }

    @Override
    protected Boolean doInBackground(Integer... i) {
        while (true) {
            try {
                Thread.sleep(2000);
            } catch (Exception e) {
                return true;
            }
            publishProgress();
        }
    }

    protected void onPostExecute(Boolean... value) {
        // idk
    }
}
