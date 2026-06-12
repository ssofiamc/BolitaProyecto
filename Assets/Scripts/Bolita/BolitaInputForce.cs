using UnityEngine;

[RequireComponent(typeof(Bolita))]
public class BolitaInputForce : MonoBehaviour
{
    [Header("Control del Jugador")]
    [Tooltip("Magnitud de la fuerza aplicada al presionar WASD.")]
    public float fuerzaMovimiento = 50f;

    [Tooltip("Permite movimiento diagonal normalizado.")]
    public bool normalizarDiagonal = true;

    private Bolita bolita;

    private void Awake()
    {
        bolita = GetComponent<Bolita>();
    }

    private void Update()
    {
        // Ejes del teclado + gamepad
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        // Mantener compatibilidad total con WASD y flechas
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            x = -1f;

        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            x = 1f;

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            z = 1f;

        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            z = -1f;

        Vector3 direccion =
            new Vector3(x, 0f, z); // Dirección del movimiento en el plano XZ

        if (
            normalizarDiagonal &&
            direccion.sqrMagnitude > 1f
        ) // Si se permite normalizar diagonal y la magnitud de la dirección es mayor que 1 (movimiento diagonal), normalizar el vector para evitar que sea más rápido en diagonal
        {
            direccion.Normalize();
        }

        if (
            direccion.sqrMagnitude <=
            Mathf.Epsilon
        ) // Si no hay entrada de movimiento (el vector es prácticamente cero), no aplicar fuerza
        {
            return;
        }

        Vector3 fuerza =
            direccion *
            fuerzaMovimiento; // Calcular la fuerza a aplicar multiplicando la dirección por la magnitud de la fuerza definida

        bolita.AgregarFuerza(fuerza); // Aplicar la fuerza a la bolita utilizando su método AgregarFuerza, lo que hará que se mueva en la dirección deseada
    }
}