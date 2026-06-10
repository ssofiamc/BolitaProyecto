using UnityEngine;

public class MagneticSimulation :
    MonoBehaviour,
    IForceGenerator
{
    [Header("Parámetros del Imán")]
    public float constanteK = 100f;

    public float distanciaMinima = 0.5f;

    public float radioAccion = 15f;

    public bool esRepulsivo = false;

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
        foreach (Bolita bolita in SimulationManager.Todas)
        {
            Vector3 delta =
                transform.position -
                bolita.Posicion;

            float distancia =
                delta.magnitude;

            if (distancia > radioAccion)
                continue;

            distancia =
                Mathf.Max(
                    distancia,
                    distanciaMinima
                );

            Vector3 direccion =
                delta.normalized;

            if (esRepulsivo)
                direccion *= -1f;

            float magnitud =
                constanteK *
                bolita.Masa /
                (distancia * distancia);

            Vector3 fuerza =
                direccion *
                magnitud;

            bolita.AgregarFuerza(
                fuerza
            );

            Debug.DrawLine(
                bolita.Posicion,
                transform.position,
                Color.magenta
            );
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color =
            esRepulsivo
            ? Color.red
            : Color.magenta;

        Gizmos.DrawWireSphere(
            transform.position,
            radioAccion
        );
    }
}