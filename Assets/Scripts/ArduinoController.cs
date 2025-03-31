using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.LowLevel;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;

public class ArduinoController : MonoBehaviour
{
    public string portName = "COM3";
    public int baudRate = 115200;

    private SerialPort serialPort;
    private Thread readThread;
    private bool isRunning = false;
    private string data;
    private object threadLock = new object();
    private Keyboard keyboard;

    void Start()
    {
        try
        {
            serialPort = new SerialPort(portName, baudRate);
            serialPort.ReadTimeout = 20;

            serialPort.DtrEnable = true;
            serialPort.RtsEnable = true;
            serialPort.NewLine = "\n";

            serialPort.Open();
            isRunning = true;

            readThread = new Thread(ReadSerialData);
            readThread.Start();

            Debug.Log("Serial port opened successfully");
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error opening serial port: " + e.Message);
        }
        keyboard = InputSystem.GetDevice<Keyboard>();
    }

    void ReadSerialData()
    {
        while (isRunning)
        {
            try
            {
                if (serialPort.IsOpen)
                {
                    serialPort.DiscardInBuffer(); // Clear any backlogged data
                    data = serialPort.ReadLine();
                }
                else
                {
                    Debug.Log("serial port closed");
                }
            }
            catch (System.TimeoutException) { } // Ignore timeouts
            catch (System.Exception e)
            {
                Debug.LogError("Error reading serial data: " + e.Message);
                isRunning = false;
            }

            Thread.Sleep(10); // Prevent tight looping
        }
    }

    void Update()
    {
        string[] stringArray;
        lock (threadLock)
        {
            if (data == null) {
                return;
            }
            stringArray = data.Split(',');
        }
        if (stringArray.Length != 5)
        {
            return;
        }
        for (int i = 0; i < stringArray.Length; i++) {
            List<Key> pressedKeys = new List<Key>();
            if (stringArray[0] == "1")
            {
                pressedKeys.Add(Key.Space);
            }
            if (stringArray[1] == "1")
            {
                pressedKeys.Add(Key.S);
            }
            if(stringArray[2] == "1")
            {
                pressedKeys.Add(Key.W);
            }
            if(stringArray[3] == "1")
            {
                pressedKeys.Add(Key.A);
            }
            if(stringArray[4] == "1")
            {
                pressedKeys.Add(Key.D);
            }
            KeyboardState keyboardState = new KeyboardState(pressedKeys.ToArray());
            InputSystem.QueueStateEvent(keyboard, keyboardState);
        }

    }

    void OnDisable()
    {
        CloseSerialPort();
    }

    void CloseSerialPort()
    {
        isRunning = false;
        if (readThread != null && readThread.IsAlive)
        {
            readThread.Join(500); // Wait for thread to finish
        }
        if (serialPort != null && serialPort.IsOpen)
        {
            serialPort.Close();
            serialPort.Dispose();
        }
    }

    void OnApplicationQuit()
    {
        CloseSerialPort();
    }

}
