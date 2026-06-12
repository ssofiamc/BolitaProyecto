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

    private void OnEnable() //  Suscripción al simulador
    {
        SimulationManager.Registrar(this); // Registrar este generador de fuerzas en el SimulationManager para que se le llame cada paso de simulación.
    }

    private void OnDisable() // Desuscripción del simulador
    {
        SimulationManager.Desregistrar(this); // Desregistrar este generador de fuerzas del SimulationManager para que deje de recibir llamadas cada paso de simulación.
    }

    public void ApplyForces(float dt) // Método de la interfaz IForceGenerator que se llama cada paso de simulación para aplicar fuerzas a las bolitas.
    {
        foreach (Bolita bolita in SimulationManager.Todas) // Iterar sobre todas las bolitas registradas en el SimulationManager para aplicarles la fuerza magnética.
        {
            Vector3 delta =
                transform.position -
                bolita.Posicion; // Calcular el vector de diferencia entre la posición del imán (este objeto) y la posición de la bolita para determinar la dirección y distancia de la fuerza magnética.

            float distancia =
                delta.magnitude; // Calcular la distancia entre el imán y la bolita utilizando la magnitud del vector de diferencia.

            if (distancia > radioAccion)
                continue; // Si la distancia es mayor que el radio de acción del imán, no aplicar ninguna fuerza a esta bolita y pasar a la siguiente iteración del bucle.

            distancia =
                Mathf.Max(
                    distancia,
                    distanciaMinima
                ); // Asegurar que la distancia no sea menor que la distancia mínima para evitar fuerzas extremadamente grandes cuando la bolita esté muy cerca del imán.

            Vector3 direccion =
                delta.normalized; // Calcular la dirección de la fuerza magnética normalizando el vector de diferencia para obtener un vector unitario que apunte desde la bolita hacia el imán.

            if (esRepulsivo)
                direccion *= -1f; // Si el imán es repulsivo, invertir la dirección de la fuerza para que apunte desde el imán hacia la bolita en lugar de hacia el imán.

            float magnitud =
                constanteK *
                bolita.Masa /
                (distancia * distancia); // Calcular la magnitud de la fuerza magnética utilizando la fórmula F = k * (m1 * m2) / r^2, donde k es la constante del imán, m1 es la masa de la bolita, m2 se asume como 1 para simplificar, y r es la distancia entre el imán y la bolita.

            Vector3 fuerza =
                direccion *
                magnitud; // Calcular el vector de fuerza multiplicando la dirección por la magnitud para obtener la fuerza total que se aplicará a la bolita.

            bolita.AgregarFuerza(
                fuerza
            ); // Aplicar la fuerza calculada a la bolita utilizando su método AgregarFuerza para que esta fuerza afecte su movimiento en la simulación.

            Debug.DrawLine(
                bolita.Posicion,
                transform.position,
                Color.magenta
            ); // Dibujar una línea de depuración entre la posición de la bolita y la posición del imán para visualizar la dirección de la fuerza magnética en la escena durante la simulación.
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