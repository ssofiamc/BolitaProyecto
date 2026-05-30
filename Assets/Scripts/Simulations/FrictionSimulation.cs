using UnityEngine;

public class FrictionSimulation : MonoBehaviour, IForceGenerator
{
    public enum TipoSuperficie { Hielo, Normal, Barro, Arena, Personalizada }

    [Tooltip("Selecciona el tipo de superficie para cargar valores predefinidos.")]
    public TipoSuperficie tipo = TipoSuperficie.Barro;

    [Tooltip("Coeficiente de fricción μk. Se usa solo si tipo = Personalizada.")]
    [Range(0f, 1f)] public float coeficienteFriccionPersonalizado = 0.4f;

    [Header("Resistencia Activa")]
    [Tooltip("Si activo, aplica una fuerza de frenado activa (drag de superficie).")]
    public bool aplicarDragActivo = true;

    [Tooltip("Multiplicador del drag activo.")]
    [Range(0f, 3f)] public float multiplicadorDrag = 1f;

    [Header("Debug")]
    public bool mostrarGizmos = true;

    // ─────────────────────────────────────────────
    // TABLA DE COEFICIENTES
    // ─────────────────────────────────────────────
    private static readonly float[] coeficientesPorTipo = {
        0.03f,  // Hielo
        0.3f,   // Normal
        0.65f,  // Barro
        0.50f,  // Arena
        0f      // Personalizada (se usa coeficienteFriccionPersonalizado)
    };

    private float CoeficienteActual => tipo == TipoSuperficie.Personalizada
        ? coeficienteFriccionPersonalizado
        : coeficientesPorTipo[(int)tipo];

    // Bolita en zona
    private Bolita bolitaEnZona = null;

    // ─────────────────────────────────────────────
    // REGISTRO
    // ─────────────────────────────────────────────
    private void OnEnable() => SimulationManager.Registrar((IForceGenerator)this);
    private void OnDisable() => SimulationManager.Desregistrar((IForceGenerator)this);

    // ─────────────────────────────────────────────
    // GENERADOR DE FUERZA
    // Aplica drag activo en la dirección opuesta al movimiento:
    //   F_drag = -μk × m × g × v̂   (proporcional a la velocidad normalizada)
    // ─────────────────────────────────────────────
    public void ApplyForces(float dt)
    {
        if (!aplicarDragActivo || bolitaEnZona == null) return;

        Vector3 vel = bolitaEnZona.Velocidad;
        float speed = vel.magnitude;

        if (speed < 0.01f) return;  // No aplicar si está quieta

        // F_drag_superficie = -μk × m × g × dirección_velocidad
        float magnitud = CoeficienteActual * bolitaEnZona.Masa * 9.81f * multiplicadorDrag;
        Vector3 fuerzaFriccion = -(vel / speed) * magnitud;

        bolitaEnZona.AgregarFuerza(fuerzaFriccion);
    }

    // ─────────────────────────────────────────────
    // DETECCIÓN DE ZONA: modificar fricción de rebote
    // ─────────────────────────────────────────────
    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent<Bolita>(out var bolita)) return;
        bolitaEnZona = bolita;

        // Cambiar la fricción interna de la bolita para los rebotes
        bolita.SetFriccionZona(CoeficienteActual);
        Debug.Log($"[Fricción] Superficie: {tipo} — μk = {CoeficienteActual:F2}");
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.TryGetComponent<Bolita>(out var bolita)) return;
        bolitaEnZona = null;

        // Restaurar fricción base de la bolita
        bolita.RestaurarFriccion();
        Debug.Log($"[Fricción] Bolita salió de la superficie {tipo}. Fricción restaurada.");
    }

    // ─────────────────────────────────────────────
    // GIZMOS
    // ─────────────────────────────────────────────
    private void OnDrawGizmos()
    {
        if (!mostrarGizmos) return;

        Color col = tipo switch
        {
            TipoSuperficie.Hielo => new Color(0.6f, 0.9f, 1f, 0.3f),
            TipoSuperficie.Barro => new Color(0.5f, 0.3f, 0.1f, 0.3f),
            TipoSuperficie.Arena => new Color(1f, 0.9f, 0.4f, 0.3f),
            TipoSuperficie.Normal => new Color(0.5f, 0.8f, 0.5f, 0.2f),
            _ => new Color(1f, 1f, 1f, 0.2f)
        };

        Gizmos.color = col;
        if (TryGetComponent<Collider>(out var col3D))
            Gizmos.DrawCube(col3D.bounds.center, col3D.bounds.size);
    }
}
