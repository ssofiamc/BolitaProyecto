using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPausa : MonoBehaviour
{
    private bool juegoPausado = false;

    public void Pausar()
    {
        Time.timeScale = 0f;          // Congela el juego
        juegoPausado = true;
    }

    public void Reanudar()
    {
        Time.timeScale = 1f;          // Descongela el juego
        juegoPausado = false;
    }

    public void VolverAlJuego()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("GameScene"); 
    }
}