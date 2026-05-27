using UnityEngine;

public class MenuPausa : MonoBehaviour
{
    // Arrastra aquí tu panel de pausa desde el Inspector
    public GameObject panelPausa;
    private bool juegoPausado = false;

    // Esta función la llamará tu botón de Pausa
    public void Pausar()
    {
        panelPausa.SetActive(true);    // Muestra el menú de pausa
        Time.timeScale = 0f;          // Congela el juego
        juegoPausado = true;
    }

    // Esta función la llamará tu botón de Reanudar/Despausa
    public void Reanudar()
    {
        panelPausa.SetActive(false);   // Esconde el menú de pausa
        Time.timeScale = 1f;          // Descongela el juego
        juegoPausado = false;
    }
}