using UnityEngine;

public class SimulationZone : MonoBehaviour
{
    [Header("Información")]
    public string nombreSimulacion = "Magnetismo";

    [Header("Radio de detección")]
    public float radioAccion = 10f;

    private void Update() // Se ejecuta cada frame
    {
        foreach (Bolita bolita in SimulationManager.Todas) // Recorre todas las bolitas registradas en el SimulationManager
        {
            float distancia =
                Vector3.Distance(
                    transform.position,
                    bolita.Posicion
                ); // Calcula la distancia entre la posición de esta zona de simulación y la posición de la bolita

            if (distancia <= radioAccion)
            {
                BolitaHUD.zonaActual =
                    nombreSimulacion;

                return; // Si la bolita está dentro del radio de acción, actualiza el HUD con el nombre de la simulación y sale del método
            }
        }
    }
}