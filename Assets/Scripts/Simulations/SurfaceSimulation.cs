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
            float distancia =
                Vector3.Distance(
                    transform.position,
                    bolita.Posicion
                );

            if (distancia <= radioAccion)
            {
                switch (tipoSuperficie)
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
