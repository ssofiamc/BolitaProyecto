using UnityEngine;
using System.IO.Ports;

public class MPU6050Reader : MonoBehaviour
{
    SerialPort serial = new SerialPort(“COM3”, 9600);

    public Transform objeto;

    void Start()
    {
        serial.Open();
        serial.ReadTimeout = 20;
    }

    void Update()
    {
        try
        {
            string data = serial.ReadLine();

            string[] values = data.Split(‘,’);

            if (values.Length == 3)
            {
                float x = float.Parse(values[0]);
                float y = float.Parse(values[1]);
                float z = float.Parse(values[2]);

                objeto.rotation = Quaternion.Euler(x, y, z);
            }
        }
        catch
        {

        }
    }

    private void OnApplicationQuit()
    {
        if (serial.IsOpen)
        {
            serial.Close();
        }
    }
}