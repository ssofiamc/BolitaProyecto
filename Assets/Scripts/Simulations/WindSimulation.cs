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

    public float amplitudOscilacion = 20f;

    [Min(0.01f)]
    public float frecuenciaHz = 0.5f;

    [Header("Área de acción")]
    public float radioAccion = 12f;

    [Header("Debug")]
    public bool mostrarGizmos = true;

    private float tiempoInterno;

    private void OnEnable()
    {
        SimulationManager.Registrar(this);
    }

    private void OnDisable()
    {
        SimulationManager.Desregistrar(this);
    }

    public void ApplyForces(float dt)
    {
        tiempoInterno += dt;

        float intensidadActual =
            intensidadBase;

        if (esOscilante)
        {
            float omega =
                2f *
                Mathf.PI *
                frecuenciaHz;

            intensidadActual +=
                amplitudOscilacion *
                Mathf.Sin(
                    omega *
                    tiempoInterno
                );
        }

        Vector3 direccion =
            direccionViento.normalized;

        foreach (
            Bolita bolita
            in SimulationManager.Todas
        )
        {
            float distancia =
                Vector3.Distance(
                    transform.position,
                    bolita.Posicion
                );

            if (
                distancia >
                radioAccion
            )
            {
                continue;
            }

            Vector3 fuerzaViento =
                direccion *
                intensidadActual;

            bolita.AgregarFuerza(
                fuerzaViento
            );

            Debug.DrawRay(
                bolita.Posicion,
                direccion * 2f,
                Color.cyan
            );
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