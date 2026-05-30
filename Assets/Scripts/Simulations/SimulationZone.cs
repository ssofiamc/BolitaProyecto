using UnityEngine;

public class SimulationZone : MonoBehaviour
{
    [Header("Nombre de la Simulación")]
    [Tooltip("Texto que aparecerá en el HUD cuando la bolita entre a esta zona.")]
    public string nombreSimulacion = "Magnetismo";

    [Header("Objetos a Activar al Entrar")]
    [Tooltip("Objetos que se activan cuando la bolita entra (efectos, luces, etc.).")]
    public GameObject[] objetosAlEntrar;

    [Header("Objetos a Desactivar al Entrar")]
    public GameObject[] objetosAlSalir;

    // ─────────────────────────────────────────────
    // DETECCIÓN
    // ─────────────────────────────────────────────
    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent<Bolita>(out _)) return;

        BolitaHUD.zonaActual = nombreSimulacion;
        Debug.Log($"[Zona] Bolita entró a: {nombreSimulacion}");

        foreach (var obj in objetosAlEntrar)
            if (obj != null) obj.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.TryGetComponent<Bolita>(out _)) return;

        // Restaurar a zona general
        if (BolitaHUD.zonaActual == nombreSimulacion)
            BolitaHUD.zonaActual = "Tránsito";

        foreach (var obj in objetosAlSalir)
            if (obj != null) obj.SetActive(false);
    }
}
