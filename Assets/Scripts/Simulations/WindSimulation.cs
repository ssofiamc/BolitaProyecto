using UnityEngine;

public class WindSimulation :
    MonoBehaviour,
    IForceGenerator
{
    [Header("Dirección del viento")]
    public Vector3 direccionViento =
        Vector3.right;

    [Header("Intensidad")]
    public float intensidadBase = 40f;

    public bool esOscilante = false;

    public float amplitudOscilacion = 40f;

    [Min(0.01f)]
    public float frecuenciaHz = 3f;

    [Header("Área de acción")]
    public float radioAccion = 12f;

    [Header("Debug")]
    public bool mostrarGizmos = true;

    private float tiempoInterno;

    private void OnEnable() // Suscripción al simulador
    {
        SimulationManager.Registrar(this); // Registrar este generador de fuerzas en el manager para que se llame a ApplyForces cada paso de simulación
    }

    private void OnDisable() // Desuscripción del simulador
    {
        SimulationManager.Desregistrar(this); // Desregistrar este generador de fuerzas del manager para que deje de llamarse a ApplyForces cuando este objeto se desactive o destruya
    }

    public void ApplyForces(float dt) // Método de la interfaz IForceGenerator que se llama cada paso de simulación para aplicar fuerzas a las bolitas
    {
        tiempoInterno += dt; // Incrementamos el tiempo interno con el delta time para usarlo en el cálculo de la oscilación del viento, lo que nos permite crear un efecto de viento que sube y baja con el tiempo si esOscilante es verdadero

        float intensidadActual =
            intensidadBase; // Comenzamos con la intensidad base del viento, que es el valor inicial de la fuerza que se aplicará a las bolitas, y luego le sumaremos una componente oscilante si esOscilante es verdadero para crear un efecto de viento dinámico que varía con el tiempo

        if (esOscilante) // Si el viento es oscilante, calculamos la intensidad actual sumando una componente sinusoidal a la intensidad base, usando la amplitud y frecuencia definidas, y el tiempo interno para crear un efecto de viento que sube y baja con el tiempo
        {
            float omega =
                2f *
                Mathf.PI *
                frecuenciaHz; // La frecuencia angular se calcula multiplicando 2π por la frecuencia en Hz, lo que nos da la velocidad a la que oscila el viento

            intensidadActual +=
                amplitudOscilacion *
                Mathf.Sin(
                    omega *
                    tiempoInterno
                ); // La intensidad actual se calcula sumando a la intensidad base una componente sinusoidal que varía entre -amplitudOscilacion y +amplitudOscilacion, dependiendo del tiempo interno y la frecuencia, lo que crea un efecto de viento que sube y baja con el tiempo
        }

        Vector3 direccion =
            direccionViento.normalized; // Normalizamos la dirección del viento para asegurarnos de que tenga una magnitud de 1, lo que nos permite escalarla correctamente por la intensidad actual sin afectar su dirección

        foreach (
            Bolita bolita
            in SimulationManager.Todas
        ) // Iteramos sobre todas las bolitas registradas en el SimulationManager para aplicarles la fuerza del viento si están dentro del área de acción definida por el radioAccion
        {
            float distancia =
                Vector3.Distance(
                    transform.position,
                    bolita.Posicion
                ); // Calculamos la distancia entre la posición del viento (este objeto) y la posición de la bolita para determinar si está dentro del área de acción del viento

            if (
                distancia >
                radioAccion
            ) // Si la distancia es mayor que el radio de acción, significa que la bolita está fuera del alcance del viento, por lo que no aplicamos ninguna fuerza y continuamos con la siguiente bolita en el bucle
            {
                continue;
            }

            Vector3 fuerzaViento =
                direccion *
                intensidadActual; // Calculamos la fuerza del viento multiplicando la dirección normalizada del viento por la intensidad actual, lo que nos da un vector de fuerza que apunta en la dirección del viento y tiene una magnitud proporcional a su intensidad

            bolita.AgregarFuerza(
                fuerzaViento
            ); // Aplicamos la fuerza del viento a la bolita utilizando el método AgregarFuerza de la clase Bolita, lo que hará que la bolita acelere en la dirección del viento según su masa y otras propiedades físicas

            Debug.DrawRay(
                bolita.Posicion,
                direccion * 2f,
                Color.cyan
            ); // Dibujamos un rayo desde la posición de la bolita en la dirección del viento para visualizar la fuerza aplicada, con una longitud de 2 unidades y color cian, lo que nos ayuda a entender cómo el viento afecta a cada bolita dentro de su área de acción
        }
    }

    private void OnDrawGizmos()
    {
        if (!mostrarGizmos)
            return;

        Gizmos.color =
            new Color(
                0f,
                0.8f,
                1f,
                0.25f
            );

        Gizmos.DrawWireSphere(
            transform.position,
            radioAccion
        );

        Gizmos.color =
            Color.cyan;

        Gizmos.DrawRay(
            transform.position,
            direccionViento.normalized *
            4f
        );
    }
}