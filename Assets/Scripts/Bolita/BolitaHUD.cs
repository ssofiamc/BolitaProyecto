using UnityEngine;
using TMPro;

public class BolitaHUD : MonoBehaviour
{
    [Header("Texto de UI")]
    [Tooltip("TMP_Text donde se mostrará la información física.")]
    public TMP_Text textoEstado;

    [Header("Referencia a la Bolita")]
    public Bolita bolita;

    [Header("Altura de referencia para Ep")]
    [Tooltip("Altura Y desde la cual se calcula la energía potencial.")]
    public float alturaReferencia = 0f;

    [Header("Zona Activa")]
    [Tooltip("Texto que indica la simulación activa (set desde otros scripts).")]
    public static string zonaActual = "—";

    // ─────────────────────────────────────────────
    // SUSCRIPCIÓN AL SIMULADOR
    // ─────────────────────────────────────────────
    private void OnEnable()
    {
        if (BolitaSimulator.Instancia != null)
            BolitaSimulator.Instancia.OnPaso += Actualizar;
    }

    private void OnDisable()
    {
        if (BolitaSimulator.Instancia != null)
            BolitaSimulator.Instancia.OnPaso -= Actualizar;
    }

    // ─────────────────────────────────────────────
    // ACTUALIZAR HUD cada paso de simulación
    // ─────────────────────────────────────────────
    private void Actualizar(float dt)
    {
        if (textoEstado == null || bolita == null) return;

        var sim = BolitaSimulator.Instancia;

        // Cálculo de energías
        float v2 = bolita.Velocidad.sqrMagnitude;
        float ek = 0.5f * bolita.Masa * v2;                              // ½mv²
        float h = bolita.Posicion.y - alturaReferencia;
        float ep = bolita.Masa * 9.81f * h;                              // mgh
        float et = ek + ep;

        string estado = sim.estaPausada ? "⏸ PAUSADO" : "▶ CORRIENDO";
        string arduino = MPU6050Reader.conectado ? "✓ Arduino" : "⌨ Teclado";

        textoEstado.text =
            $"<b>{estado}</b>\n" +
            $"t = {sim.TiempoSimulacion:F2}s  |  paso #{sim.ContadorPasos}\n" +
            $"Δt = {sim.updateTime * 1000f:F0} ms\n" +
            $"─────────────────\n" +
            $"v  = {Mathf.Sqrt(v2):F2} m/s\n" +
            $"Ek = {ek:F2} J\n" +
            $"Ep = {ep:F2} J\n" +
            $"Et = {et:F2} J\n" +
            $"─────────────────\n" +
            $"Zona: {zonaActual}\n" +
            $"Entrada: {arduino}";
    }
}
