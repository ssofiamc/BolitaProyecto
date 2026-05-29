using UnityEngine;
using System.IO.Ports;

public class MPU6050Reader : MonoBehaviour
{
    SerialPort serial = new SerialPort("COM3", 115200);

    // Valores públicos para usar en otros scripts
    public static float x;
    public static float y;
    public static float z;

    // Offsets iniciales
    private float offsetX;
    private float offsetY;
    private float offsetZ;

    // Suavizado
    private float smoothX;
    private float smoothY;
    private float smoothZ;

    // Configuración
    public float deadZone = 5f;
    public float smoothSpeed = 15f;

    private bool calibrated = false;

    void Start()
    {
        try
        {
            serial.ReadTimeout = 20;
            serial.Open();

            Debug.Log("Arduino conectado");

            // Esperar un poco antes de calibrar
            Invoke(nameof(CalibrateGyro), 2f);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error al conectar Arduino: " + e.Message);
        }
    }

    void CalibrateGyro()
    {
        float totalX = 0;
        float totalY = 0;
        float totalZ = 0;

        int samples = 100;

        for (int i = 0; i < samples; i++)
        {
            try
            {
                string data = serial.ReadLine();

                string[] values = data.Split(',');

                if (values.Length == 3)
                {
                    totalX += float.Parse(values[0]);
                    totalY += float.Parse(values[1]);
                    totalZ += float.Parse(values[2]);
                }
            }
            catch
            {

            }
        }

        offsetX = totalX / samples;
        offsetY = totalY / samples;
        offsetZ = totalZ / samples;

        calibrated = true;

        Debug.Log("MPU6050 calibrado");
    }

    void Update()
    {
        if (!calibrated)
            return;

        if (serial == null || !serial.IsOpen)
            return;

        try
        {
            string data = serial.ReadLine();

            string[] values = data.Split(',');

            if (values.Length == 3)
            {
                float rawX = float.Parse(values[0]) - offsetX;
                float rawY = float.Parse(values[1]) - offsetY;
                float rawZ = float.Parse(values[2]) - offsetZ;

                // Deadzone
                if (Mathf.Abs(rawX) < deadZone) rawX = 0;
                if (Mathf.Abs(rawY) < deadZone) rawY = 0;
                if (Mathf.Abs(rawZ) < deadZone) rawZ = 0;

                // Suavizado
                smoothX = Mathf.Lerp(smoothX, rawX, Time.deltaTime * smoothSpeed);
                smoothY = Mathf.Lerp(smoothY, rawY, Time.deltaTime * smoothSpeed);
                smoothZ = Mathf.Lerp(smoothZ, rawZ, Time.deltaTime * smoothSpeed);

                x = smoothX;
                y = smoothY;
                z = smoothZ;
            }
        }
        catch
        {

        }
    }

    private void OnApplicationQuit()
    {
        if (serial != null && serial.IsOpen)
        {
            serial.Close();
        }
    }
}