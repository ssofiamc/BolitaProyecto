using UnityEngine;

public class BoostSimulation : MonoBehaviour
{
    [Header("Impulso")]
    public Vector3 direccionImpulso =
            Vector3.forward;

    public float fuerzaImpulso = 120f;

    public float tiempoRecarga = 0.5f;

    private float ultimoBoost = -999f;

    private void Update()
    {
        foreach (Bolita bolita in SimulationManager.Todas)
        {
            float distancia =
                Vector3.Distance(
                    bolita.Posicion,
                    transform.position
                );

            if (distancia > 1.5f)
                continue;

            if (Time.time <
                ultimoBoost + tiempoRecarga)
                continue;

            ultimoBoost = Time.time;

            Vector3 impulso =
                direccionImpulso.normalized *
                fuerzaImpulso;

            bolita.AgregarImpulso(
                impulso
            );

            Debug.Log(
                "[BOOST] Impulso aplicado"
            );
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
