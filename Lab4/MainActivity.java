package com.example.lab4;

import android.app.Activity;
import android.hardware.Sensor;
import android.hardware.SensorEvent;
import android.hardware.SensorEventListener;
import android.hardware.SensorManager;
import android.os.AsyncTask;
import android.os.Bundle;
import android.widget.TextView;

import com.example.lab4.databinding.ActivityMainBinding;

import org.w3c.dom.Text;

public class MainActivity extends Activity {

    private SensorManager sm;
    private Sensor heart, stepSensor;
    private TextView hTextView, sTextView;
    private ActivityMainBinding binding;
    private int index = 0, steps = 0;
    private boolean reset = true;

    private final int WINDOW = 6;
    private float rollingAverage = 0, rollingAverage2 = 0, previousPeak = -1, heartRate = -1;
    private float[] accel = new float[WINDOW];

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);

        binding = ActivityMainBinding.inflate(getLayoutInflater());
        setContentView(binding.getRoot());

        hTextView = binding.heart;
        sTextView = binding.step;
        sm = (SensorManager) getSystemService(this.SENSOR_SERVICE);

        heart = sm.getDefaultSensor(Sensor.TYPE_HEART_RATE);
        stepSensor = sm.getDefaultSensor(Sensor.TYPE_ACCELEROMETER);

        System.out.println(sm.registerListener(hSensorEventListener, heart, SensorManager.SENSOR_DELAY_NORMAL));
        System.out.println(sm.registerListener(stepSensorEventListener, stepSensor, SensorManager.SENSOR_DELAY_UI));
        System.out.println("--------------------------- RESET ---------------------------");

        WorkThread thread = new WorkThread(hTextView, sTextView);
        thread.execute();
    }

    @Override
    protected void onResume() {
        super.onResume();
        sm.registerListener(stepSensorEventListener, stepSensor, SensorManager.SENSOR_DELAY_NORMAL);
        sm.registerListener(hSensorEventListener, heart, SensorManager.SENSOR_DELAY_NORMAL);
        WorkThread thread = new WorkThread(hTextView, sTextView);
        thread.execute();
    }

    @Override
    protected void onPause() {
        super.onPause();
        sm.unregisterListener(stepSensorEventListener);
        sm.unregisterListener(hSensorEventListener);
    }

    private SensorEventListener hSensorEventListener = new SensorEventListener() {
        @Override
        public void onSensorChanged(SensorEvent sensorEvent) {
            System.out.print("Heart Sensor changed: ");
            heartRate = sensorEvent.values[0];
            System.out.println(sensorEvent.values[0]);
        }

        @Override
        public void onAccuracyChanged(Sensor sensor, int i) {

        }
    };

    private SensorEventListener stepSensorEventListener = new SensorEventListener() {

        @Override
        public void onSensorChanged(SensorEvent sensorEvent) {
            //System.out.print("Accelerometer Sensor change");

            /*
            Rolling average
            Compare new average to the 2 previous averages and if there is a peak.
            oldAvg2 < oldAvg  && oldAvr > newAvg
             */

            float previousValue = accel[index];
            accel[index] = Math.abs(sensorEvent.values[0]) + Math.abs(sensorEvent.values[1]) + Math.abs(sensorEvent.values[2]);
            float newAverage = rollingAverage - previousValue/WINDOW;
            newAverage += accel[index]/WINDOW;

            if (rollingAverage > rollingAverage2 && rollingAverage > newAverage && (rollingAverage-newAverage) > 0.08) {
                System.out.println(String.format("%f %f %f", rollingAverage2, rollingAverage, newAverage));
                steps++;
                if (steps % 2 == 0) {
                    System.out.println(steps / 2);
                }
            }

            rollingAverage2 = rollingAverage;
            rollingAverage = newAverage;
        }

        @Override
        public void onAccuracyChanged(Sensor sensor, int i) {

        }
    };

    class WorkThread extends AsyncTask<Integer, Double, Boolean> {
        TextView ss, hh;
        public WorkThread(TextView h_h, TextView s_s) {
            super();
            hh = h_h;
            ss = s_s;
        }
        @Override
        protected void onProgressUpdate(Double... values) {
            ss.setText("Steps: " + String.valueOf(steps/2));
            hh.setText("HeartRate: " + String.valueOf((int)heartRate));
        }

        @Override
        protected Boolean doInBackground(Integer... i) {
            while (true) {
                try {
                    Thread.sleep(500);
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
}


