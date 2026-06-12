using UnityEngine;

public class BoostSimulation : MonoBehaviour
{
    [Header("Impulso")]
    public Vector3 direccionImpulso = Vector3.forward;
    public float fuerzaImpulso = 120f;
    public float tiempoRecarga = 0.5f;
    private float ultimoBoost = -999f;

    private void Update() // Se ejecuta cada frame
    {
        foreach (Bolita bolita in SimulationManager.Todas) // Itera sobre todas las bolitas registradas en el SimulationManager
        {
            float distancia =
                Vector3.Distance(
                    bolita.Posicion,
                    transform.position
                ); // Calcula la distancia entre la bolita y el centro del boost

            if (distancia > 1.5f) // Si la bolita está a más de 1.5 unidades de distancia, no se le aplica el impulso
                continue;

            if (Time.time <
                ultimoBoost + tiempoRecarga)
                continue; // Si no ha pasado el tiempo de recarga desde el último boost, no se le aplica el impulso

            ultimoBoost = Time.time; // Actualiza el tiempo del último boost al tiempo actual

            Vector3 impulso =
                direccionImpulso.normalized *
                fuerzaImpulso; // Calcula el vector de impulso normalizando la dirección y multiplicándola por la fuerza

            bolita.AgregarImpulso(impulso); // Aplica el impulso a la bolita utilizando su método AgregarImpulso
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        Gizmos.DrawWireSphere(
            transform.position,
            1.5f
        );

        Gizmos.DrawRay(
            transform.position,
            direccionImpulso.normalized * 3f
        );
    }
}
