using UnityEngine;

public class MagneticSimulation : MonoBehaviour, IForceGenerator
{
    [Header("Parámetros del Imán")]
    [Tooltip("Constante k del imán. Mayor = más fuerte.")]
    public float constanteK = 200f;

    [Tooltip("Distancia mínima para evitar fuerza infinita.")]
    [Min(0.1f)] public float distanciaMinima = 0.5f;

    [Tooltip("Radio máximo de acción del imán (debe coincidir con el SphereCollider Trigger).")]
    public float radioAccion = 8f;

    [Tooltip("Si es verdadero, repele en lugar de atraer.")]
    public bool esRepulsivo = false;

    [Header("Debug")]
    public bool mostrarGizmos = true;

    // ─────────────────────────────────────────────
    // Referencia a la bolita (se asigna cuando entra al trigger)
    // ─────────────────────────────────────────────
    private Bolita bolitaEnZona = null;

    // ─────────────────────────────────────────────
    // REGISTRO EN EL MUNDO
    // ─────────────────────────────────────────────
    private void OnEnable() => SimulationManager.Registrar((IForceGenerator)this);
    private void OnDisable() => SimulationManager.Desregistrar((IForceGenerator)this);

    // ─────────────────────────────────────────────
    // GENERADOR DE FUERZA
    // ─────────────────────────────────────────────
    public void ApplyForces(float dt)
    {
        if (bolitaEnZona == null) return;

        // Vector de la bolita hacia el imán
        Vector3 delta = transform.position - bolitaEnZona.Posicion;
        float distancia = delta.magnitude;

        // Clamp de distancia mínima (evita fuerza infinita)
        distancia = Mathf.Max(distancia, distanciaMinima);

        // Dirección normalizada
        Vector3 direccion = delta / distancia;

        // F = k × m / d²
        // La fuerza decrece con el cuadrado de la distancia
        float magnitud = constanteK * bolitaEnZona.Masa / (distancia * distancia);

        // Si es repulsivo, invertir dirección
        if (esRepulsivo) direccion = -direccion;

        Vector3 fuerzaMagnetica = direccion * magnitud;
        bolitaEnZona.AgregarFuerza(fuerzaMagnetica);

        // Debug visual
        Debug.DrawLine(bolitaEnZona.Posicion, transform.position, Color.magenta);
    }

    // ─────────────────────────────────────────────
    // DETECCIÓN DE ZONA (Trigger)
    // ─────────────────────────────────────────────
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Bolita>(out var bolita))
        {
            bolitaEnZona = bolita;
            Debug.Log("[Magnetismo] Bolita entró al campo magnético.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<Bolita>(out _))
        {
            bolitaEnZona = null;
        }
    }

    // ─────────────────────────────────────────────
    // VISUALIZACIÓN EN EDITOR
    // ─────────────────────────────────────────────
    private void OnDrawGizmos()
    {
        if (!mostrarGizmos) return;
        Gizmos.color = esRepulsivo ? new Color(1f, 0.5f, 0f, 0.3f) : new Color(1f, 0f, 1f, 0.3f);
        Gizmos.DrawWireSphere(transform.position, radioAccion);
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, distanciaMinima);
    }
}
