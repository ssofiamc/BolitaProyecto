using UnityEngine;

public class JumpSimulation : MonoBehaviour
{
    [Header("Impulso Vertical")]
    [Tooltip("Impulso aplicado hacia arriba (N·s).")]
    public float impulsoVertical = 60f;

    [Tooltip("Evita múltiples activaciones seguidas.")]
    public float tiempoRecarga = 0.5f;

    private float ultimoSalto = -999f;

    private void Update() // Se ejecuta cada frame
    {
        foreach (Bolita bolita in SimulationManager.Todas) // Itera sobre todas las bolitas registradas en el SimulationManager
        {
            float distancia =
                Vector3.Distance(
                    bolita.Posicion,
                    transform.position
                ); // Calcula la distancia entre la bolita y el trampolín

            if (distancia > 1.5f)
                continue; // Si la bolita está demasiado lejos, no se aplica el impulso

            if (
                Time.time <
                ultimoSalto + tiempoRecarga
            ) // Si no ha pasado suficiente tiempo desde el último salto, se evita aplicar otro impulso
                continue;

            ultimoSalto = Time.time; // Actualiza el tiempo del último salto para controlar la recarga

            Vector3 impulso =
                Vector3.up *
                impulsoVertical; // Crea un vector de impulso hacia arriba con la magnitud definida por impulsoVertical

            bolita.AgregarImpulso(impulso); // Aplica el impulso a la bolita, lo que hará que salte hacia arriba
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
