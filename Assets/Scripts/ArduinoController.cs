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
    [SerializeField] Transform player1 = null;
    [SerializeField] Transform player2 = null;
    [SerializeField] Transform player3 = null;
    [SerializeField] Transform player4 = null;
    [SerializeField] bool isMenu;
    private Transform[] players = new Transform[4];
    private Key[] keyMap = { 
        Key.Space, Key.S, Key.W, Key.A, Key.D,
        Key.RightShift, Key.DownArrow, Key.UpArrow, Key.LeftArrow, Key.RightArrow,
        Key.Z, Key.U, Key.I, Key.O, Key.P,
        Key.X, Key.H, Key.J, Key.K, Key.L
    };

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
        players[0] = player1;
        players[1] = player2;
        players[2] = player3;
        players[3] = player4;
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
        if (stringArray.Length != 20)
        {
            return;
        }
        Debug.Log(data);
        List<Key> pressedKeys = new List<Key>();
        for (int i = 0; i < stringArray.Length; i++) {
            if (stringArray[i] == "1")
            {
                pressedKeys.Add(keyMap[i]);
            }
        }
        KeyboardState keyboardState = new KeyboardState(pressedKeys.ToArray());
        InputSystem.QueueStateEvent(keyboard, keyboardState);

        string result = $"{GetDashStatus(1)},{GetDashStatus(2)},{GetDashStatus(3)},{GetDashStatus(4)}";
        WriteSerialData(result);
    }

    private void WriteSerialData(string data)
    {
        if (serialPort != null && serialPort.IsOpen)
        {
            try
            {
                serialPort.WriteLine(data);
                // Debug.Log("writing: " + data);
            }
            catch (System.Exception e)
            {
                Debug.LogError("Error writing to serial port: " + e.Message);
            }
        }
    }

    private int GetDashStatus(int playerNo) {
        int index = playerNo - 1;
        if (players[index] == null) {
            return 0;
        }
        if (isMenu)
        {
            return 1;
        }
        if (players[index].GetComponent<PlayerMovement>().ableToBoost())
        {
            return 1;
        }
        else
        {
            return 0;
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
