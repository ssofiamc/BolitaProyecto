using TMPro;
using UnityEngine;

public class BolitaHUD : MonoBehaviour
{
    public static string zonaActual = "Exploración"; // Valor estático que se actualiza desde SimulationZone

    public TMP_Text textoZona; // Referencia al componente de texto en la UI para mostrar la zona actual

    private void Update() // Se ejecuta cada frame para actualizar el texto de la zona actual
    {
        textoZona.text =
            "Simulación: " +
            zonaActual; // Actualiza el texto del HUD para mostrar la zona actual de simulación, utilizando el valor estático que se actualiza desde SimulationZone
    }
}