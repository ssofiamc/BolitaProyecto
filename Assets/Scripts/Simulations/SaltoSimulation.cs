using UnityEngine;

public class SaltoSimulation : MonoBehaviour
{
    [Header("Impulso Vertical")]
    public float fuerzaSalto = 20f;

    private void OnTriggerEnter(Collider other)
    {
        Bolita bolita =
            other.GetComponent<Bolita>();

        if (bolita == null)
            return;

        Vector3 impulso =
            Vector3.up *
            fuerzaSalto;

        bolita.AgregarImpulso(
            impulso
        );
        Debug.Log("JUMP PAD ACTIVADO");
    }
}
