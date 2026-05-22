using UnityEngine;
using System.IO.Ports;
using System.Threading;

public class ArduinoSerialReader : MonoBehaviour
{
    public static ArduinoSerialReader Instance { get; private set; }

    [Header("Serial Configuration")]
    [SerializeField] private string portName = "COM4";
    [SerializeField] private int baudRate = 115200;
    [SerializeField] private int readTimeoutMs = 100;

    public int RawSpeed { get; private set; }
    public int RawSteering { get; private set; }
    public bool IsConnected { get; private set; }

    private SerialPort serialPort;
    private Thread readThread;
    private bool running;

    private readonly object dataLock = new object();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        ConnectTo(portName);
    }

    public void ConnectTo(string port)
    {
        try
        {
            serialPort = new SerialPort(port, baudRate)
            {
                ReadTimeout = readTimeoutMs,
                NewLine = "\n",
                DtrEnable = true,
                RtsEnable = true
            };

            serialPort.Open();

            // Esperar a que Arduino reinicie
            Thread.Sleep(2000);

            running = true;

            readThread = new Thread(ReadLoop);
            readThread.IsBackground = true;
            readThread.Start();

            IsConnected = true;

            Debug.Log("[Arduino] Connected to " + port);
        }
        catch (System.Exception e)
        {
            Debug.LogError("[Arduino] Connection Error: " + e.Message);
        }
    }

    void ReadLoop()
    {
        while (running)
        {
            try
            {
                string line = serialPort.ReadLine();
                ParseLine(line);
            }
            catch (System.TimeoutException)
            {
                // Ignorar timeout
            }
            catch (System.Exception e)
            {
                Debug.LogWarning("[Arduino] Read Error: " + e.Message);

                running = false;
                IsConnected = false;
            }
        }
    }

    void ParseLine(string line)
    {
        if (string.IsNullOrWhiteSpace(line))
            return;

        string[] parts = line.Trim().Split(',');

        if (parts.Length != 2)
            return;

        if (int.TryParse(parts[0], out int speed) &&
            int.TryParse(parts[1], out int steering))
        {
            lock (dataLock)
            {
                RawSpeed = speed;
                RawSteering = steering;
            }
        }
    }

    public void GetNormalizedValues(out float speed, out float steering)
    {
        lock (dataLock)
        {
            speed = (RawSpeed - 512) / 512f;
            steering = (RawSteering - 512) / 512f;
        }

        speed = Mathf.Clamp(speed, -1f, 1f);
        steering = Mathf.Clamp(steering, -1f, 1f);
    }

    void OnApplicationQuit()
    {
        Disconnect();
    }

    void OnDestroy()
    {
        Disconnect();
    }

    void Disconnect()
    {
        running = false;

        if (readThread != null && readThread.IsAlive)
        {
            readThread.Join(500);
        }

        if (serialPort != null && serialPort.IsOpen)
        {
            serialPort.Close();
        }

        IsConnected = false;

        Debug.Log("[Arduino] Disconnected");
    }
}