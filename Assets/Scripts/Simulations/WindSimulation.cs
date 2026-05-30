using UnityEngine;

public class WindSimulation : MonoBehaviour, IForceGenerator
{
    [Header("Dirección del Viento")]
    [Tooltip("Dirección del viento en el mundo (se normaliza automáticamente).")]
    public Vector3 direccionViento = Vector3.right;

    [Header("Intensidad")]
    [Tooltip("Intensidad base del viento (N).")]
    public float intensidadBase = 30f;

    [Tooltip("Si activo, el viento oscila en intensidad (sinusoidal).")]
    public bool esOscilante = false;

    [Tooltip("Amplitud de la oscilación (sobre la intensidad base).")]
    public float amplitudOscilacion = 15f;

    [Tooltip("Frecuencia de oscilación en Hz (ciclos por segundo).")]
    [Min(0.01f)] public float frecuenciaHz = 0.5f;

    [Header("Coeficiente de Arrastre")]
    [Tooltip("Coeficiente de arrastre simplificado (Cd). 1 = igual a la intensidad base.")]
    [Range(0.1f, 5f)] public float coeficienteArrastre = 1f;

    [Header("Debug")]
    public bool mostrarGizmos = true;

    // Referencia a la bolita en zona
    private Bolita bolitaEnZona = null;

    // Tiempo interno para la oscilación
    private float tiempoInterno = 0f;

    // ─────────────────────────────────────────────
    // REGISTRO
    // ─────────────────────────────────────────────
    private void OnEnable() => SimulationManager.Registrar((IForceGenerator)this);
    private void OnDisable() => SimulationManager.Desregistrar((IForceGenerator)this);

    // ─────────────────────────────────────────────
    // GENERADOR DE FUERZA
    // ─────────────────────────────────────────────
    public void ApplyForces(float dt)
    {
        if (bolitaEnZona == null) return;

        tiempoInterno += dt;

        // Calcular intensidad actual
        float intensidadActual = intensidadBase;
        if (esOscilante)
        {
            // ω = 2π × f
            float omega = 2f * Mathf.PI * frecuenciaHz;
            intensidadActual += amplitudOscilacion * Mathf.Sin(omega * tiempoInterno);
        }

        // Dirección normalizada × coeficiente × intensidad
        Vector3 dirNorm = direccionViento.normalized;
        Vector3 fuerzaViento = dirNorm * intensidadActual * coeficienteArrastre;

        bolitaEnZona.AgregarFuerza(fuerzaViento);

        // Debug
        Debug.DrawRay(bolitaEnZona.Posicion, fuerzaViento.normalized * 2f, Color.cyan);
    }

    // ─────────────────────────────────────────────
    // DETECCIÓN DE ZONA
    // ─────────────────────────────────────────────
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Bolita>(out var bolita))
        {
            bolitaEnZona = bolita;
            Debug.Log("[Viento] Bolita entró a la zona de viento.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<Bolita>(out _))
            bolitaEnZona = null;
    }

    // ─────────────────────────────────────────────
    // GIZMOS
    // ─────────────────────────────────────────────
    private void OnDrawGizmos()
    {
        if (!mostrarGizmos) return;
        Gizmos.color = new Color(0f, 0.8f, 1f, 0.25f);

        // Dibuja el área del trigger si hay un collider
        if (TryGetComponent<Collider>(out var col))
        {
            Gizmos.DrawCube(col.bounds.center, col.bounds.size);
        }

        // Flecha de dirección del viento
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(transform.position, direccionViento.normalized * 3f);
    }
}
