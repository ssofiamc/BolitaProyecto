using TMPro;
using UnityEngine;

public class BolitaHUD : MonoBehaviour
{
    public static string zonaActual =
        "Exploración";

    public TMP_Text textoZona;

    private void Update()
    {
        textoZona.text =
            "Simulación: " +
            zonaActual;
    }
}