using UnityEngine;

public class SurfaceSimulation : MonoBehaviour, IForceGenerator
{
    public enum TipoSuperficie
    {
        SuperficieA,
        SuperficieB,
        SuperficieC
    }

    [Header("Tipo de Superficie")]
    public TipoSuperficie tipoSuperficie =
        TipoSuperficie.SuperficieA;

    [Header("Zona de Acción")]
    public float radioAccion = 5f;

    [Header("Coeficientes")]
    public float friccionA = 0.02f;
    public float friccionB = 0.30f;
    public float friccionC = 0.80f;

    private void OnEnable() // Suscripción al simulador
    {
        SimulationManager.Registrar(this); // Registrar este generador de fuerzas en el SimulationManager
    }

    private void OnDisable() // Desuscripción del simulador
    {
        SimulationManager.Desregistrar(this); // Desregistrar este generador de fuerzas del SimulationManager
    }

    public void ApplyForces(float dt) // Método de la interfaz IForceGenerator que se llama cada paso de simulación
    {
        foreach (Bolita bolita in SimulationManager.Todas) // Iterar sobre todas las bolitas registradas en el SimulationManager
        {
            float distancia =
                Vector3.Distance(
                    transform.position,
                    bolita.Posicion
                ); // Calcular la distancia entre la posición de esta superficie y la posición de la bolita

            if (distancia <= radioAccion) // Si la bolita está dentro del radio de acción de esta superficie
            {
                switch (tipoSuperficie) // Aplicar el coeficiente de fricción correspondiente según el tipo de superficie
                {
                    case TipoSuperficie.SuperficieA:

                        bolita.SetFriccionZona(
                            friccionA
                        );
                        break;

                    case TipoSuperficie.SuperficieB:

                        bolita.SetFriccionZona(
                            friccionB
                        );
                        break;

                    case TipoSuperficie.SuperficieC:

                        bolita.SetFriccionZona(
                            friccionC
                        );
                        break;
                }
            }
            else
            {
                bolita.RestaurarFriccion();
            }
        }
    }

    private void OnDrawGizmos()
    {
        switch (tipoSuperficie)
        {
            case TipoSuperficie.SuperficieA:
                Gizmos.color = Color.cyan;
                break;

            case TipoSuperficie.SuperficieB:
                Gizmos.color = Color.gray;
                break;

            case TipoSuperficie.SuperficieC:
                Gizmos.color = Color.yellow;
                break;
        }

        Gizmos.DrawWireSphere(
            transform.position,
            radioAccion
        );
    }
}
