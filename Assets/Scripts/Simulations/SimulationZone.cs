using UnityEngine;

public class SimulationZone : MonoBehaviour
{
    [Header("Información")]
    public string nombreSimulacion = "Magnetismo";

    [Header("Radio de detección")]
    public float radioAccion = 10f;

    private void Update()
    {
        foreach (Bolita bolita in SimulationManager.Todas)
        {
            float distancia =
                Vector3.Distance(
                    transform.position,
                    bolita.Posicion
                );

            if (distancia <= radioAccion)
            {
                BolitaHUD.zonaActual =
                    nombreSimulacion;

                return;
            }
        }
    }
}