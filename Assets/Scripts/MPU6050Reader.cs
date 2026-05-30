using UnityEngine;
using System.IO.Ports;
using System.Collections;

public class MPU6050Reader : MonoBehaviour
{
    [Header("Configuración Serial")]
    [Tooltip("Puerto COM del Arduino. En Windows: COM3, COM4, etc. En Mac/Linux: /dev/tty...")]
    public string puerto = "COM3";

    [Tooltip("Baud rate del Arduino. Debe coincidir con el Serial.begin() del sketch.")]
    public int baudRate = 115200;

    [Header("Calibración")]
    [Tooltip("Zona muerta: valores menores a esto se tratan como 0 (elimina ruido en reposo).")]
    public float deadZone = 5f;

    [Tooltip("Número de muestras para calcular el offset en calibración.")]
    public int muestrasCalibracion = 50;

    [Header("Suavizado")]
    [Tooltip("Velocidad de suavizado. Mayor = más rápido pero más tembloroso.")]
    public float velocidadSuavizado = 15f;

    // ─────────────────────────────────────────────
    // VALORES PÚBLICOS ESTÁTICOS (accesibles desde cualquier script)
    // ─────────────────────────────────────────────
    /// <summary>Inclinación X calibrada y suavizada (positivo = derecha).</summary>
    public static float x = 0f;
    /// <summary>Inclinación Y calibrada y suavizada (positivo = adelante).</summary>
    public static float y = 0f;
    /// <summary>Inclinación Z calibrada y suavizada (rotación).</summary>
    public static float z = 0f;

    /// <summary>True si el Arduino está conectado y calibrado.</summary>
    public static bool conectado = false;

    // ─────────────────────────────────────────────
    // ESTADO INTERNO
    // ─────────────────────────────────────────────
    private SerialPort serial;
    private bool calibrado = false;
    private float offsetX, offsetY, offsetZ;
    private float smoothX, smoothY, smoothZ;

    // ─────────────────────────────────────────────
    // INICIO
    // ─────────────────────────────────────────────
    private void Start()
    {
        try
        {
            serial = new SerialPort(puerto, baudRate)
            {
                ReadTimeout = 30,
                WriteTimeout = 100
            };
            serial.Open();
            conectado = true;
            Debug.Log($"[MPU6050] Arduino conectado en {puerto} a {baudRate} bps.");

            // Esperar 2s para que el Arduino se estabilice, luego calibrar
            StartCoroutine(EsperarYCalibrar());
        }
        catch (System.Exception e)
        {
            conectado = false;
            Debug.LogWarning($"[MPU6050] No se pudo conectar al Arduino: {e.Message}\n" +
                             "Modo teclado activado como respaldo.");
        }
    }

    // ─────────────────────────────────────────────
    // CALIBRACIÓN (corrutina para no bloquear Unity)
    // Toma N muestras en reposo y calcula el offset promedio.
    // Offset = valor promedio en reposo (inclinación de la mesa, etc.)
    // ─────────────────────────────────────────────
    private IEnumerator EsperarYCalibrar()
    {
        yield return new WaitForSeconds(2f);

        float totalX = 0, totalY = 0, totalZ = 0;
        int muestrasValidas = 0;

        for (int i = 0; i < muestrasCalibracion; i++)
        {
            if (LeerLinea(out float rx, out float ry, out float rz))
            {
                totalX += rx;
                totalY += ry;
                totalZ += rz;
                muestrasValidas++;
            }
            yield return null; // Esperar un frame entre muestras
        }

        if (muestrasValidas > 0)
        {
            offsetX = totalX / muestrasValidas;
            offsetY = totalY / muestrasValidas;
            offsetZ = totalZ / muestrasValidas;
        }

        calibrado = true;
        Debug.Log($"[MPU6050] Calibración completada. Offset: ({offsetX:F2}, {offsetY:F2}, {offsetZ:F2})");
    }

    // ─────────────────────────────────────────────
    // UPDATE — lee y procesa un dato por frame
    // ─────────────────────────────────────────────
    private void Update()
    {
        if (!calibrado || serial == null || !serial.IsOpen) return;

        if (!LeerLinea(out float rx, out float ry, out float rz)) return;

        // Restar offset de calibración
        float cx = rx - offsetX;
        float cy = ry - offsetY;
        float cz = rz - offsetZ;

        // Aplicar zona muerta (elimina ruido pequeño en reposo)
        if (Mathf.Abs(cx) < deadZone) cx = 0f;
        if (Mathf.Abs(cy) < deadZone) cy = 0f;
        if (Mathf.Abs(cz) < deadZone) cz = 0f;

        // Suavizado tipo filtro pasa-bajos (Lerp)
        // Reduce saltos bruscos del sensor
        smoothX = Mathf.Lerp(smoothX, cx, Time.deltaTime * velocidadSuavizado);
        smoothY = Mathf.Lerp(smoothY, cy, Time.deltaTime * velocidadSuavizado);
        smoothZ = Mathf.Lerp(smoothZ, cz, Time.deltaTime * velocidadSuavizado);

        // Publicar valores estáticos
        x = smoothX;
        y = smoothY;
        z = smoothZ;
    }

    // ─────────────────────────────────────────────
    // LECTURA DE UNA LÍNEA SERIAL
    // Formato esperado del Arduino: "valorX,valorY,valorZ\n"
    // ─────────────────────────────────────────────
    private bool LeerLinea(out float rx, out float ry, out float rz)
    {
        rx = ry = rz = 0f;
        try
        {
            string linea = serial.ReadLine().Trim();
            string[] partes = linea.Split(',');
            if (partes.Length >= 3)
            {
                rx = float.Parse(partes[0], System.Globalization.CultureInfo.InvariantCulture);
                ry = float.Parse(partes[1], System.Globalization.CultureInfo.InvariantCulture);
                rz = float.Parse(partes[2], System.Globalization.CultureInfo.InvariantCulture);
                return true;
            }
        }
        catch { /* Timeout o línea corrupta: se ignora silenciosamente */ }
        return false;
    }

    // ─────────────────────────────────────────────
    // CIERRE LIMPIO DEL PUERTO SERIAL
    // ─────────────────────────────────────────────
    private void OnApplicationQuit()
    {
        if (serial != null && serial.IsOpen)
        {
            serial.Close();
            Debug.Log("[MPU6050] Puerto serial cerrado.");
        }
    }

    private void OnDestroy()
    {
        if (serial != null && serial.IsOpen)
            serial.Close();
    }
}