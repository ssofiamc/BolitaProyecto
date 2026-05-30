using UnityEngine;

public class PendulumSimulation : MonoBehaviour, IForceGenerator
{
    [Header("Configuración del Péndulo")]
    [Tooltip("Punto de pivote del péndulo (asignar un Transform vacío en la parte superior).")]
    public Transform pivote;

    [Tooltip("Longitud del péndulo (m).")]
    [Min(0.1f)] public float longitud = 4f;

    [Tooltip("Ángulo inicial (grados). 90 = posición horizontal.")]
    [Range(-90f, 90f)] public float anguloInicial = 45f;

    [Header("Física del Péndulo")]
    [Tooltip("Gravedad para el péndulo (puede diferir de la bolita).")]
    public float gravedad = 9.81f;

    [Tooltip("Amortiguamiento (0 = sin fricción, 1 = sobre-amortiguado).")]
    [Range(0f, 2f)] public float amortiguamiento = 0.1f;

    [Header("Modo")]
    [Tooltip("Si activo, este GameObject oscila como obstáculo.\n" +
             "Si inactivo, aplica fuerza oscilante sobre la bolita en zona.")]
    public bool moverObstaculo = true;

    [Tooltip("Fuerza lateral aplicada sobre la bolita (solo si moverObstaculo = false).")]
    public float fuerzaLateral = 40f;

    [Header("Debug")]
    public bool mostrarGizmos = true;

    // ─────────────────────────────────────────────
    // ESTADO DEL PÉNDULO
    // ─────────────────────────────────────────────
    private float angulo;        // θ en radianes
    private float velAngular;    // θ̇ en rad/s

    // Bolita en zona (para modo fuerza)
    private Bolita bolitaEnZona = null;

    // ─────────────────────────────────────────────
    // INICIO
    // ─────────────────────────────────────────────
    private void OnEnable() => SimulationManager.Registrar((IForceGenerator)this);
    private void OnDisable() => SimulationManager.Desregistrar((IForceGenerator)this);

    private void Start()
    {
        angulo = anguloInicial * Mathf.Deg2Rad;
        velAngular = 0f;
    }

    // ─────────────────────────────────────────────
    // GENERADOR DE FUERZA — integra el péndulo cada paso
    // ─────────────────────────────────────────────
    public void ApplyForces(float dt)
    {
        // ── Integración del péndulo (Euler) ──
        // θ̈ = -(g/L) × sin(θ) - b × θ̇
        float aceleracionAngular = -(gravedad / longitud) * Mathf.Sin(angulo)
                                   - amortiguamiento * velAngular;

        velAngular += aceleracionAngular * dt;
        angulo += velAngular * dt;

        // ── Modo obstáculo: mover este GameObject ──
        if (moverObstaculo && pivote != null)
        {
            // Posición en el espacio a partir del ángulo y la longitud
            Vector3 offset = new Vector3(
                Mathf.Sin(angulo) * longitud,
                -Mathf.Cos(angulo) * longitud,
                0f
            );
            transform.position = pivote.position + offset;
        }

        // ── Modo fuerza: empujar la bolita ──
        if (!moverObstaculo && bolitaEnZona != null)
        {
            // Fuerza proporcional al seno del ángulo (componente horizontal)
            float fuerzaX = Mathf.Sin(angulo) * fuerzaLateral;
            bolitaEnZona.AgregarFuerza(new Vector3(fuerzaX, 0f, 0f));
        }
    }

    // ─────────────────────────────────────────────
    // DETECCIÓN DE ZONA (solo modo fuerza)
    // ─────────────────────────────────────────────
    private void OnTriggerEnter(Collider other)
    {
        if (!moverObstaculo && other.TryGetComponent<Bolita>(out var bolita))
            bolitaEnZona = bolita;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!moverObstaculo && other.TryGetComponent<Bolita>(out _))
            bolitaEnZona = null;
    }

    // ─────────────────────────────────────────────
    // GIZMOS
    // ─────────────────────────────────────────────
    private void OnDrawGizmos()
    {
        if (!mostrarGizmos || pivote == null) return;

        Vector3 extremo = pivote.position + new Vector3(
            Mathf.Sin(Application.isPlaying ? angulo : anguloInicial * Mathf.Deg2Rad) * longitud,
            -Mathf.Cos(Application.isPlaying ? angulo : anguloInicial * Mathf.Deg2Rad) * longitud,
            0f
        );

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(pivote.position, extremo);
        Gizmos.DrawWireSphere(extremo, 0.3f);
        Gizmos.DrawWireSphere(pivote.position, 0.1f);
    }
}
