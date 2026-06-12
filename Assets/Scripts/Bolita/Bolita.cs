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

    private Vector3 posicion;
    private Vector3 velocidad;
    private Vector3 fuerzaAcumulada;

    private float masa;
    private float radio;

    private Vector3 posicionInicial;
    private float friccionActual;

    private bool enSuelo;

    public float Masa => masa;

    public float Radio => radio;

    public float Restitucion => restitucion;

    public Vector3 Velocidad // Propiedad para acceder a la velocidad actual
    {
        get => velocidad;
        set => velocidad = value;
    } // Propiedad para acceder a la velocidad actual, permitiendo leer y modificar la velocidad desde otros componentes si es necesario.

    public Vector3 Posicion // Propiedad para acceder a la posición actual, actualizando la posición del transform cuando se asigna un nuevo valor
    {
        get => posicion;
        set
        {
            posicion = value;
            transform.position = value;
        }
    } // Propiedad para acceder a la posición actual, permitiendo leer y modificar la posición desde otros componentes si es necesario, y asegurando que el transform se actualice automáticamente cuando se asigna una nueva posición.

    private void Awake() // Inicialización de parámetros físicos y condiciones iniciales
    {
        posicionInicial = transform.position;

        CalcularRadio();
        CalcularMasa();

        friccionActual = friccionBase;
    } // Inicialización de parámetros físicos y condiciones iniciales, donde se guarda la posición inicial del transform para poder resetearla posteriormente, se calcula el radio y la masa basándose en el collider y el factor de masa, y se establece la fricción actual al valor base definido en el inspector.

    private void Start() // Registrar esta bolita en el SimulationManager para que sea incluida en la simulación y pueda recibir actualizaciones de posición, velocidad y fuerzas aplicadas por los generadores de fuerza registrados en el simulador.
    {
        Resetear();

        SimulationManager.Registrar(this);
    }

    private void OnDisable() // Desregistrar esta bolita del SimulationManager para que deje de ser incluida en la simulación y no reciba actualizaciones de posición, velocidad y fuerzas aplicadas por los generadores de fuerza registrados en el simulador cuando este objeto ya no esté activo o haya sido destruido.
    {
        SimulationManager.Desregistrar(this);
    }

    private void DetectarSuelo() // Detectar si la bolita está en contacto con el suelo utilizando un raycast hacia abajo desde la posición actual, con una distancia igual al radio de la bolita más un pequeño margen (skin) para evitar problemas de detección debido a pequeñas irregularidades en el terreno o colisiones cercanas.
    {
        enSuelo = Physics.Raycast(
            posicion,
            Vector3.down,
            radio + 0.05f,
            mascaraColision,
            QueryTriggerInteraction.Ignore
        );
    }

    public void Integrar(float dt) // Método de integración que se llama cada paso de simulación para actualizar la velocidad y posición de la bolita basándose en las fuerzas acumuladas, el drag, la gravedad y las colisiones detectadas. Se calcula la aceleración a partir de las fuerzas acumuladas y el drag, se aplica la gravedad si no está en el suelo, se actualiza la velocidad con la aceleración, se aplica fricción de rodadura si está en el suelo, se limita la velocidad máxima y se resetea la fuerza acumulada para el siguiente paso de simulación.
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

    public void Mover(float dt) // Método de movimiento que se llama cada paso de simulación para actualizar la posición de la bolita basándose en la velocidad actual y manejar las colisiones contra paredes u otros obstáculos utilizando SphereCast para detectar colisiones horizontales y verticales, ajustando la posición y reflejando la velocidad según sea necesario para simular rebotes y deslizamientos.
    {
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

    private void ReflejarVelocidad( // Método para reflejar la velocidad de la bolita al colisionar con una superficie, aplicando restitución y fricción según el ángulo de la colisión. Si la velocidad está dirigida hacia la superficie (dot product negativo), se calcula la componente normal y tangencial de la velocidad, se aplica fricción a la componente tangencial y restitución a la componente normal, y se actualiza la velocidad resultante.
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

    public void AgregarFuerza( // Método para agregar una fuerza a la bolita, acumulándola en el vector de fuerzaAcumulada para que sea aplicada en el siguiente paso de simulación durante la integración.
        Vector3 fuerza)
    {
        fuerzaAcumulada += fuerza;
    }

    public void AgregarImpulso( // Método para agregar un impulso a la bolita, modificando directamente su velocidad basándose en la magnitud del impulso y la masa de la bolita, lo que permite aplicar cambios instantáneos en la velocidad (como un golpe o un salto) sin necesidad de acumular fuerzas a lo largo del tiempo.
        Vector3 impulso)
    {
        velocidad += impulso / masa;
    }

    public void SetFriccionZona( // Método para establecer el coeficiente de fricción actual de la bolita al entrar en contacto con una zona de superficie específica, permitiendo que diferentes zonas apliquen diferentes niveles de fricción a la bolita mientras esté en contacto con ellas. El coeficiente se clampa entre 0 y 1 para asegurar valores válidos.
        float coeficiente)
    {
        friccionActual =
            Mathf.Clamp01(
                coeficiente
            );
    }

    public void RestaurarFriccion() // Método para restaurar el coeficiente de fricción actual de la bolita al valor base definido en el inspector, lo que se puede llamar cuando la bolita sale de una zona de superficie específica para volver a aplicar la fricción normal.
    {
        friccionActual =
            friccionBase;
    }

    public void ActualizarVisuales()
    {
        transform.position =
            posicion;
    }

    public void Resetear() // Método para resetear la bolita a sus condiciones iniciales, estableciendo la posición a la posición inicial guardada, la velocidad a la velocidad inicial definida en el inspector, reseteando la fuerza acumulada a cero, restaurando la fricción al valor base y actualizando la posición del transform para reflejar el reset.
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

    private void CalcularMasa() // Método para calcular la masa de la bolita basándose en su volumen (calculado a partir del radio) y el factor de masa definido en el inspector, asegurando que la masa mínima sea un valor pequeño para evitar divisiones por cero o masas extremadamente pequeñas que puedan causar problemas en la simulación.
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

    private void CalcularRadio()  // Método para calcular el radio de la bolita basándose en el radio del SphereCollider y la escala del transform, asegurando que el radio se ajuste correctamente si el objeto ha sido escalado en alguna dirección.
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

    private void OnValidate() // Método que se llama automáticamente en el editor de Unity cada vez que se modifica un valor en el inspector, lo que permite recalcular el radio y la masa de la bolita automáticamente para reflejar los cambios realizados en los parámetros físicos o en la escala del objeto.
    {
        CalcularRadio();
        CalcularMasa();
    }
}