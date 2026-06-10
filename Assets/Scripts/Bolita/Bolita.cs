using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class Bolita : MonoBehaviour
{
    [Header("Condiciones Iniciales")]
    public Vector3 velocidadInicial = Vector3.zero;

    [Header("Parámetros Físicos")]
    [Tooltip("Gravedad manual (m/s²).")]
    public Vector3 gravedad = new Vector3(0f, -9.81f, 0f);

    [Tooltip("Factor de masa.")]
    [Min(0.0001f)]
    public float masaFactor = 1f;

    [Tooltip("Drag lineal.")]
    [Min(0f)]
    public float drag = 1f;

    [Tooltip("Restitución de colisiones.")]
    [Range(0f, 1f)]
    public float restitucion = 0.25f;

    [Tooltip("Fricción base.")]
    [Range(0f, 1f)]
    public float friccionBase = 0.3f;

    [Header("Colisiones")]
    public LayerMask mascaraColision;

    [Min(0f)]
    public float skin = 0.02f;

    [Header("Velocidad Máxima")]
    public float velocidadMaxima = 20f;

    // =========================
    // Estado físico interno
    // =========================

    private Vector3 posicion;
    private Vector3 velocidad;
    private Vector3 fuerzaAcumulada;

    private float masa;
    private float radio;

    private Vector3 posicionInicial;
    private float friccionActual;

    private bool enSuelo;

    // =========================
    // Propiedades públicas
    // =========================

    public float Masa => masa;

    public float Radio => radio;

    public float Restitucion => restitucion;

    public Vector3 Velocidad
    {
        get => velocidad;
        set => velocidad = value;
    }

    public Vector3 Posicion
    {
        get => posicion;
        set
        {
            posicion = value;
            transform.position = value;
        }
    }

    // =========================
    // Inicialización
    // =========================

    private void Awake()
    {
        posicionInicial = transform.position;

        CalcularRadio();
        CalcularMasa();

        friccionActual = friccionBase;
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

    // =========================
    // Detección de suelo
    // =========================

    private void DetectarSuelo()
    {
        enSuelo = Physics.Raycast(
            posicion,
            Vector3.down,
            radio + 0.05f,
            mascaraColision,
            QueryTriggerInteraction.Ignore
        );
    }

    // =========================
    // Integración numérica
    // =========================

    public void Integrar(float dt)
    {
        DetectarSuelo();

        Vector3 fuerzaDrag =
            -drag * velocidad;

        Vector3 aceleracion =
            (fuerzaAcumulada + fuerzaDrag) / masa;

        if (!enSuelo)
        {
            aceleracion += gravedad;
        }

        velocidad += aceleracion * dt;

        // Fricción de rodadura
        if (enSuelo)
        {
            float factorRodadura =
        Mathf.Clamp01(
            1f - friccionActual * dt * 10f
        );

            velocidad.x *= factorRodadura;
            velocidad.z *= factorRodadura;
        }

        if (velocidad.magnitude > velocidadMaxima)
        {
            velocidad =
                velocidad.normalized *
                velocidadMaxima;
        }

        fuerzaAcumulada = Vector3.zero;
    }

    // =========================
    // Movimiento
    // =========================

    public void Mover(float dt)
    {
        // ===================================
        // MOVIMIENTO HORIZONTAL
        // ===================================

        Vector3 movimientoHorizontal =
            new Vector3(
                velocidad.x,
                0f,
                velocidad.z
            ) * dt;

        float distanciaHorizontal =
            movimientoHorizontal.magnitude;

        if (distanciaHorizontal > Mathf.Epsilon)
        {
            Vector3 direccionHorizontal =
                movimientoHorizontal.normalized;

            if (
                Physics.SphereCast(
                    posicion,
                    radio,
                    direccionHorizontal,
                    out RaycastHit golpeHorizontal,
                    distanciaHorizontal + skin,
                    mascaraColision,
                    QueryTriggerInteraction.Ignore
                )
            )
            {
                // Ignorar suelo
                if (golpeHorizontal.normal.y < 0.7f)
                {
                    float recorrido =
                        Mathf.Max(
                            0f,
                            golpeHorizontal.distance - skin
                        );

                    posicion +=
                        direccionHorizontal * recorrido;

                    ReflejarVelocidad(
                        golpeHorizontal.normal
                    );
                }
                else
                {
                    posicion += movimientoHorizontal;
                }
            }
            else
            {
                posicion += movimientoHorizontal;
            }
        }

        // ===================================
        // MOVIMIENTO VERTICAL
        // ===================================

        float movimientoY =
            velocidad.y * dt;

        if (Mathf.Abs(movimientoY) > Mathf.Epsilon)
        {
            Vector3 direccionVertical =
                movimientoY > 0f
                ? Vector3.up
                : Vector3.down;

            if (
                Physics.SphereCast(
                    posicion,
                    radio,
                    direccionVertical,
                    out RaycastHit golpeVertical,
                    Mathf.Abs(movimientoY) + skin,
                    mascaraColision,
                    QueryTriggerInteraction.Ignore
                )
            )
            {
                float recorrido =
                    Mathf.Max(
                        0f,
                        golpeVertical.distance - skin
                    );

                posicion +=
                    direccionVertical * recorrido;

                // Rebote vertical
                velocidad.y =
                    -velocidad.y *
                    restitucion;
            }
            else
            {
                posicion +=
                    Vector3.up * movimientoY;
            }
        }
    }

    // =========================
    // Respuesta de colisión
    // =========================

    private void ReflejarVelocidad(
        Vector3 normal)
    {
        if (
            Vector3.Dot(
                velocidad,
                normal
            ) >= 0f
        )
        {
            return;
        }

        Vector3 vNormal =
            Vector3.Dot(
                velocidad,
                normal
            ) * normal;

        Vector3 vTangente =
            velocidad - vNormal;

        velocidad =
            vTangente *
            (1f - friccionActual)
            -
            vNormal *
            restitucion;
    }

    // =========================
    // Fuerzas
    // =========================

    public void AgregarFuerza(
        Vector3 fuerza)
    {
        fuerzaAcumulada += fuerza;
    }

    public void AgregarImpulso(
        Vector3 impulso)
    {
        velocidad += impulso / masa;
    }

    // =========================
    // Fricción por zonas
    // =========================

    public void SetFriccionZona(
        float coeficiente)
    {
        friccionActual =
            Mathf.Clamp01(
                coeficiente
            );
    }

    public void RestaurarFriccion()
    {
        friccionActual =
            friccionBase;
    }

    // =========================
    // Visuales
    // =========================

    public void ActualizarVisuales()
    {
        transform.position =
            posicion;
    }

    // =========================
    // Reset
    // =========================

    public void Resetear()
    {
        posicion =
            posicionInicial;

        velocidad =
            velocidadInicial;

        fuerzaAcumulada =
            Vector3.zero;

        friccionActual =
            friccionBase;

        transform.position =
            posicion;
    }

    // =========================
    // Masa
    // =========================

    private void CalcularMasa()
    {
        float volumen =
            (4f / 3f) *
            Mathf.PI *
            Mathf.Pow(radio, 3);

        masa =
            Mathf.Max(
                0.0001f,
                volumen * masaFactor
            );
    }

    // =========================
    // Radio
    // =========================

    private void CalcularRadio()
    {
        SphereCollider sc =
            GetComponent<SphereCollider>();

        radio =
            sc.radius *
            Mathf.Max(
                transform.lossyScale.x,
                transform.lossyScale.y,
                transform.lossyScale.z
            );
    }

    private void OnValidate()
    {
        CalcularRadio();
        CalcularMasa();
    }
}