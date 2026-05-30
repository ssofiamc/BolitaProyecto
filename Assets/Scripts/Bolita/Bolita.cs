using UnityEngine;

public class Bolita : MonoBehaviour
{
    [Header("Condiciones Iniciales")]
    public Vector3 velocidadInicial = Vector3.zero;

    [Header("Parámetros Físicos")]
    [Tooltip("Gravedad manual (m/s²). No usa la de Unity.")]
    public Vector3 gravedad = new Vector3(0f, -9.81f, 0f);

    [Tooltip("Factor de masa. La masa real = volumen × masaFactor.")]
    [Min(0.0001f)] public float masaFactor = 1f;

    [Tooltip("Coeficiente de drag lineal (amortiguamiento aerodinámico).")]
    [Min(0f)] public float drag = 0.5f;

    [Tooltip("Restitución al rebotar (0 = sin rebote, 1 = elástico puro).")]
    [Range(0f, 1f)] public float restitucion = 0.4f;

    [Tooltip("Fricción base contra superficies.")]
    [Range(0f, 1f)] public float friccionBase = 0.3f;

    [Header("Colisiones")]
    [Tooltip("Capas contra las que colisiona (asigna la capa 'Pared').")]
    public LayerMask mascaraColision;

    [Tooltip("Margen de piel para el SphereCast (evita interpenetración).")]
    [Min(0f)] public float skin = 0.02f;

    [Header("Velocidad máxima")]
    [Tooltip("Limita la velocidad para evitar tunelización.")]
    public float velocidadMaxima = 20f;

    private Vector3 posicion;
    private Vector3 velocidad;
    private Vector3 fuerzaAcumulada;
    private float masa;
    private float radio;
    private Vector3 posicionInicial;
    private float friccionActual;

    public Vector3 Posicion
    {
        get => posicion;
        set { posicion = value; transform.position = value; }
    }
    public Vector3 Velocidad
    {
        get => velocidad;
        set => velocidad = value;
    }
    public float Masa => masa;
    public float Radio => radio;
    public float Restitucion => restitucion;

    private void Awake()
    {
        posicionInicial = transform.position;
        CalcularMasa();
        CalcularRadio();
    }

    private void Start()
    {
        Resetear();
        SimulationManager.Registrar(this);
    }

    private void OnDisable()
    {
        SimulationManager.Desregistrar(this);
    }

    // ─────────────────────────────────────────────
    // PASO 1: INTEGRACIÓN — acumula fuerzas → velocidad
    // F_total = F_ext + F_drag + F_gravedad
    // a = F_total / m
    // v(t+dt) = v(t) + a * dt          (Euler explícito)
    // ─────────────────────────────────────────────
    public void Integrar(float dt)
    {
        // Drag lineal: F_drag = -b * v
        Vector3 fDrag = -drag * velocidad;

        // Aceleración total: a = (F_ext + F_drag) / m + g
        Vector3 aceleracion = gravedad + (fuerzaAcumulada + fDrag) / masa;

        // Integración de Euler
        velocidad += aceleracion * dt;

        // Limitar velocidad máxima (evita tunelización en colisiones)
        if (velocidad.magnitude > velocidadMaxima)
            velocidad = velocidad.normalized * velocidadMaxima;

        // Limpiar fuerzas acumuladas al final del paso
        fuerzaAcumulada = Vector3.zero;
    }

    // ─────────────────────────────────────────────
    // PASO 2: MOVIMIENTO CON DETECCIÓN DE COLISIONES
    // Usa SphereCast para detectar obstáculos antes de moverse.
    // Si hay colisión: se detiene en el punto de contacto
    // y se refleja la velocidad (restitución + fricción).
    // ─────────────────────────────────────────────
    public void Mover(float dt)
    {
        Vector3 desplazamiento = velocidad * dt;
        float distancia = desplazamiento.magnitude;

        if (distancia <= Mathf.Epsilon) return;

        Vector3 direccion = desplazamiento / distancia;

        // Origen del cast retrocedido por skin (evita empezar dentro de una pared)
        Vector3 origenCast = posicion - direccion * skin;
        float distanciaCast = distancia + skin;

        if (Physics.SphereCast(origenCast, radio, direccion,
                               out RaycastHit golpe, distanciaCast,
                               mascaraColision, QueryTriggerInteraction.Ignore))
        {
            // Avanzar hasta el punto de contacto (sin entrar a la pared)
            float recorrido = Mathf.Max(0f, golpe.distance - skin);
            posicion += direccion * recorrido;

            // Reflexión de velocidad con restitución y fricción
            ReflejarVelocidad(golpe.normal);

            // Debug visual del punto de contacto
            Debug.DrawRay(golpe.point, golpe.normal * 0.3f, Color.yellow, 0.1f);
        }
        else
        {
            posicion += desplazamiento;
        }
    }

    // ─────────────────────────────────────────────
    // REFLEJO DE VELOCIDAD (colisión con pared)
    // Se descompone v en componente normal y tangencial.
    // v_normal → restitución (rebote)
    // v_tangencial → fricción (deslizamiento)
    // ─────────────────────────────────────────────
    public void ReflejarVelocidad(Vector3 normal)
    {
        Vector3 vNormal = Vector3.Dot(velocidad, normal) * normal;   // componente perpendicular
        Vector3 vTangente = velocidad - vNormal;                      // componente paralela

        // v' = tangente*(1-friccion) - normal*restitucion
        velocidad = vTangente * (1f - friccionActual) - vNormal * restitucion;
    }

    // Sincronizar Transform con la posición calculada
    public void ActualizarVisuales()
    {
        transform.position = posicion;
    }

    // ─────────────────────────────────────────────
    // API PÚBLICA: fuerzas e impulsos
    // ─────────────────────────────────────────────

    /// <summary>Añade una fuerza al acumulador. Se aplicará en el siguiente Integrar().</summary>
    public void AgregarFuerza(Vector3 fuerza) => fuerzaAcumulada += fuerza;

    /// <summary>
    /// Aplica un impulso instantáneo (J = m * Δv → Δv = J/m).
    /// No depende del dt; cambia la velocidad de inmediato.
    /// </summary>
    public void AgregarImpulso(Vector3 impulso) => velocidad += impulso / masa;

    /// <summary>Permite a zonas especiales cambiar la fricción temporalmente.</summary>
    public void SetFriccionZona(float coef) => friccionActual = Mathf.Clamp01(coef);

    /// <summary>Restaura la fricción base.</summary>
    public void RestaurarFriccion() => friccionActual = friccionBase;

    // ─────────────────────────────────────────────
    // RESET
    // ─────────────────────────────────────────────
    public void Resetear()
    {
        CalcularMasa();
        CalcularRadio();
        posicion = posicionInicial;
        velocidad = velocidadInicial;
        fuerzaAcumulada = Vector3.zero;
        friccionActual = friccionBase;
        transform.position = posicion;
    }

    // ─────────────────────────────────────────────
    // CÁLCULOS INTERNOS
    // m = volumen × masaFactor  (esfera: V = 4/3 π r³)
    // ─────────────────────────────────────────────
    private void CalcularMasa()
    {
        float r = transform.localScale.x * 0.5f;
        float volumen = (4f / 3f) * Mathf.PI * r * r * r;
        masa = Mathf.Max(0.0001f, volumen * masaFactor);
    }

    private void CalcularRadio()
    {
        float maxEscala = Mathf.Max(transform.localScale.x,
                                    transform.localScale.y,
                                    transform.localScale.z);
        radio = maxEscala * 0.5f;
    }

    private void OnValidate()
    {
        CalcularMasa();
        CalcularRadio();
    }
}
