using UnityEngine;
using System.Collections;

public class TemporizadorImagenUI : MonoBehaviour
{
    public float tiempoVisible = 5f;

    // Esta función especial de Unity se ejecuta AUTOMÁTICAMENTE 
    // cada vez que el objeto se activa (SetActive(true))
    private void OnEnable()
    {
        // Detiene cualquier conteo previo para evitar que se pisen entre sí
        StopAllCoroutines();
        // Inicia la cuenta regresiva de 5 segundos
        StartCoroutine(ContadorOcultar());
    }

    private IEnumerator ContadorOcultar()
    {
        // Espera los 5 segundos en pantalla
        yield return new WaitForSeconds(tiempoVisible);

        // Se apaga a sí mismo automáticamente
        gameObject.SetActive(false);
    }
}