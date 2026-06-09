using UnityEngine;

[RequireComponent(typeof(Bolita))]
public class BolitaInputForce : MonoBehaviour
{
    [Header("Control del Jugador")]
    [Tooltip("Magnitud de la fuerza aplicada al presionar WASD.")]
    public float fuerzaMovimiento = 25f;

    [Tooltip("Permite movimiento diagonal normalizado.")]
    public bool normalizarDiagonal = true;

    private Bolita bolita;

    private void Awake()
    {
        bolita = GetComponent<Bolita>();
    }

    private void Update()
    {
        float x = 0f;
        float z = 0f;

        // Horizontal
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            x = -1f;
        }

        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {   
            x = 1f;
        }

        // Vertical
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            z = 1f;
        }

        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            z = -1f;
        }

        Vector3 direccion = new Vector3(x, 0f, z);

        if (normalizarDiagonal && direccion.sqrMagnitude > 1f)
            direccion.Normalize();

        if (direccion.sqrMagnitude <= Mathf.Epsilon)
            return;

        Vector3 fuerza = direccion * fuerzaMovimiento;

        Debug.Log("FUERZA ENVIADA: " + fuerza);

        bolita.AgregarFuerza(fuerza);
    }
}