using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.LowLevel;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;

public class ArduinoController1 : MonoBehaviour
{
    //private string portName = "COM3";
    //private string portName = "/dev/tty.usbmodem11401";
    private string portName = "/dev/tty.usbmodem1401";
    public int baudRate = 115200;

    private SerialPort serialPort;
    private Thread readThread;
    private bool isRunning = false;
    private string data;
    private object threadLock = new object();
    private Keyboard keyboard;

    // Define the indices from the 20-value input we care about and their corresponding keys.
    // Index 0: Space, Index 5: RightShift, Index 10: Z, Index 15: X.
    private readonly int[] activeIndices = { 0, 5, 10, 15 };
    private readonly Key[] activeKeys = { Key.Space, Key.RightShift, Key.Z, Key.X };

    void Start()
    {
        try
        {
            serialPort = new SerialPort(portName, baudRate)
            {
                ReadTimeout = 20,
                NewLine = "\n",
                DtrEnable = true,
                RtsEnable = true
            };
            serialPort.Open();
            isRunning = true;

            // Start a dedicated thread to read serial data.
            readThread = new Thread(ReadSerialData);
            readThread.Start();

            Debug.Log("Serial port opened successfully for End Screen");
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
                    serialPort.DiscardInBuffer();
                    data = serialPort.ReadLine();
                }
                else
                {
                    Debug.Log("Serial port closed");
                }
            }
            catch (System.TimeoutException) { } // Ignore timeout exceptions
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
        string currentData;
        lock (threadLock)
        {
            if (data == null)
                return;
            currentData = data;
        }

        // Split the incoming string by commas. We expect 20 comma-separated values.
        string[] stringArray = currentData.Split(',');
        if (stringArray.Length < 20)
            return;

        List<Key> pressedKeys = new List<Key>();

        // Only check the indices that map to our desired keys.
        for (int i = 0; i < activeIndices.Length; i++)
        {
            int index = activeIndices[i];
            if (stringArray[index] == "1")
                pressedKeys.Add(activeKeys[i]);
        }

        if (pressedKeys.Count > 0)
        {
            KeyboardState keyboardState = new KeyboardState(pressedKeys.ToArray());
            InputSystem.QueueStateEvent(keyboard, keyboardState);
        }
    }

    void OnDisable()
    {
        CloseSerialPort();
    }

    void OnApplicationQuit()
    {
        CloseSerialPort();
    }

    void CloseSerialPort()
    {
        isRunning = false;
        if (readThread != null && readThread.IsAlive)
        {
            readThread.Join(500);
        }
        if (serialPort != null && serialPort.IsOpen)
        {
            serialPort.Close();
            serialPort.Dispose();
        }
    }
}
