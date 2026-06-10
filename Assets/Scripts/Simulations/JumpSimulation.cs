using UnityEngine;

public class JumpSimulation : MonoBehaviour
{
    [Header("Impulso Vertical")]
    [Tooltip("Impulso aplicado hacia arriba (N·s).")]
    public float impulsoVertical = 60f;

    [Tooltip("Evita múltiples activaciones seguidas.")]
    public float tiempoRecarga = 0.5f;

    private float ultimoSalto = -999f;

    private void Update()
    {
        foreach (Bolita bolita in SimulationManager.Todas)
        {
            float distancia =
                Vector3.Distance(
                    bolita.Posicion,
                    transform.position
                );

            // Radio del trampolín
            if (distancia > 1.5f)
                continue;

            if (
                Time.time <
                ultimoSalto + tiempoRecarga
            )
                continue;

            ultimoSalto = Time.time;

            Vector3 impulso =
                Vector3.up *
                impulsoVertical;

            bolita.AgregarImpulso(
                impulso
            );

            Debug.Log(
                "[SALTO] Impulso aplicado"
            );
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color =
            Color.green;

        Gizmos.DrawWireSphere(
            transform.position,
            1.5f
        );

        Gizmos.DrawRay(
            transform.position,
            Vector3.up * 2f
        );
    }
}
